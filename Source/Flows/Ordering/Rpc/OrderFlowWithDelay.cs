using Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;
using Cleipnir.ResilientFunctions.Domain;

namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc;

[GenerateFlows]
public class OrderFlowWithDelay(
    IPaymentProviderClient paymentProviderClient,
    IEmailClient emailClient,
    ILogisticsClient logisticsClient
) : Flow<Order>
{
    public override async Task Run(Order order)
    {
        await Effect.Capture(() => ProcessOrder(order));

        await Delay(TimeSpan.FromDays(1));

        await emailClient.SendFollowUpEmail(order.CustomerId);
    }

    private async Task ProcessOrder(Order order)
    {
        var transactionId = await Effect.Capture("TransactionId", Guid.NewGuid);
        
        await Capture(() => paymentProviderClient.Reserve(transactionId, order.CustomerId, order.TotalPrice));
        var trackAndTrace = await Capture(() => logisticsClient.ShipProducts(order.CustomerId, order.ProductIds));
        await Capture(() => paymentProviderClient.Capture(transactionId));
        await Capture(() => emailClient.SendOrderConfirmation(order.CustomerId, trackAndTrace, order.ProductIds));
    }
}