using Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;
using Cleipnir.ResilientFunctions.Domain;

namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc;

[GenerateFlows]
public class OrderFlow(
    IPaymentProviderClient paymentProviderClient,
    IEmailClient emailClient,
    ILogisticsClient logisticsClient
) : Flow<Order>
{
    public override async Task Run(Order order)
    {
        var transactionId = await Capture(Guid.NewGuid);
        
        await paymentProviderClient.Reserve(order.CustomerId, transactionId, order.TotalPrice);

        var trackAndTrace = await Capture(
            () => logisticsClient.ShipProducts(order.CustomerId, order.ProductIds),
            ResiliencyLevel.AtMostOnce
        );

        await paymentProviderClient.Capture(transactionId);

        await emailClient.SendOrderConfirmation(order.CustomerId, trackAndTrace, order.ProductIds);
    }
}