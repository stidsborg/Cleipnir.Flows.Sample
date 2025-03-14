using Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;
using Cleipnir.ResilientFunctions.Domain;

namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc.FlowWithErrorHandling;

[GenerateFlows]
public class OrderFlowWithErrorHandling(
    IPaymentProviderClient paymentProviderClient,
    IEmailClient emailClient,
    ILogisticsClient logisticsClient)
    : Flow<Order>
{
   public override async Task Run(Order order)
    {
        var transactionId = await Capture(Guid.NewGuid);

        try { await paymentProviderClient.Reserve(transactionId, order.CustomerId, order.TotalPrice); }
        catch (FatalWorkflowException) { await CleanUp(FailedAt.FundsReserved, transactionId, trackAndTrace: null); throw; }

        TrackAndTrace? trackAndTrace;
        try { trackAndTrace = await logisticsClient.ShipProducts(order.CustomerId, order.ProductIds); }
        catch (FatalWorkflowException) { await CleanUp(FailedAt.FundsReserved, transactionId, trackAndTrace: null); throw; }
        
        try { await paymentProviderClient.Capture(transactionId); }
        catch (FatalWorkflowException) { await CleanUp(FailedAt.FundsReserved, transactionId, trackAndTrace: null); throw; }
        
        try { await emailClient.SendOrderConfirmation(order.CustomerId, trackAndTrace!, order.ProductIds); }
        catch (FatalWorkflowException) { await CleanUp(FailedAt.FundsReserved, transactionId, trackAndTrace: null); throw; }
    }

    private async Task CleanUp(FailedAt failedAt, Guid transactionId, TrackAndTrace? trackAndTrace)
    {
        switch (failedAt) 
        {
            case FailedAt.FundsReserved:
                break;
            case FailedAt.ProductsShipped:
                await paymentProviderClient.CancelReservation(transactionId);
                break;
            case FailedAt.FundsCaptured:
                await paymentProviderClient.CancelReservation(transactionId);
                await logisticsClient.CancelShipment(trackAndTrace!);
                break;
            case FailedAt.OrderConfirmationEmailSent:
                await paymentProviderClient.Reverse(transactionId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(failedAt), failedAt, null);
        }
    }
    
    private enum FailedAt
    {
        FundsReserved,
        ProductsShipped,
        FundsCaptured,
        OrderConfirmationEmailSent,
    }
}