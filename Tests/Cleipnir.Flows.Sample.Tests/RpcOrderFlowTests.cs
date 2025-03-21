using Cleipnir.Flows.Sample.Flows.Ordering;
using Cleipnir.Flows.Sample.Flows.Ordering.Rpc;
using Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;
using Cleipnir.ResilientFunctions.Domain;
using Cleipnir.ResilientFunctions.Domain.Exceptions.Commands;

namespace Cleipnir.Flows.Sample.Tests;

[TestClass]
public sealed class RpcOrderFlowTests
{
    [TestMethod]
    public async Task OrderFlowUsesTheSameTransactionIdAfterRestart()
    {
        using var container = FlowsContainer.Create();

        var transactionIds = new List<Guid>();
        var flow = new OrderFlow(
            PaymentProviderClientTestStub.Create(reserve: (transactionId, _, _) =>
            {
                transactionIds.Add(transactionId);
                throw new SuspendInvocationException();
            }),
            EmailClientStub.Instance,
            LogisticsClientStub.Instance
        );
        var flows = container.RegisterAnonymousFlow<OrderFlow, Order>(
            flowFactory: () => flow
        );

        var testOrder = new Order("MK-54321", CustomerId: Guid.NewGuid(), ProductIds: [Guid.NewGuid()], TotalPrice: 120);
        await flows.Schedule("SomeInstance", testOrder);

        var controlPanel = await flows.ControlPanel("SomeInstance");
        Assert.IsNotNull(controlPanel);
        
        await controlPanel.BusyWaitUntil(c => c.Status == Status.Suspended);
        await controlPanel.ScheduleRestart();
        await controlPanel.Refresh();
        await controlPanel.BusyWaitUntil(c => c.Status == Status.Suspended);

        Assert.AreEqual(2, transactionIds.Count);
        Assert.AreEqual(1, transactionIds.Distinct().Count());
    }
}