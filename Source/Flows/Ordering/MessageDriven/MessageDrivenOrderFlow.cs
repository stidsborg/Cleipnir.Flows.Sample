using Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven.Clients;
using MassTransit;

namespace Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven;

[GenerateFlows]
public class MessageDrivenOrderFlow(IBus bus) : Flow<Order>
{
    public override async Task Run(Order order)
    {
        var transactionId = await Capture(Guid.NewGuid);

        await ReserveFunds(order, transactionId);
        await Message<FundsReserved>();
        
        await ShipProducts(order);
        var productsShipped = await Message<ProductsShipped>();
        var trackAndTraceNumber = productsShipped.TrackAndTraceNumber;
        
        await CaptureFunds(order, transactionId);
        await Message<FundsCaptured>();
        
        await SendOrderConfirmationEmail(order, trackAndTraceNumber);
        await Message<OrderConfirmationEmailSent>();
    }
    
    #region MessagePublishers
    
    private Task ReserveFunds(Order order, Guid transactionId) 
        => Capture(() => bus.Publish(new ReserveFunds(order.OrderId, order.TotalPrice, transactionId, order.CustomerId)));
    private Task ShipProducts(Order order)
        => Capture(() => bus.Publish(new ShipProducts(order.OrderId, order.CustomerId, order.ProductIds)));
    private Task CaptureFunds(Order order, Guid transactionId)
        => Capture(() => bus.Publish(new CaptureFunds(order.OrderId, order.CustomerId, transactionId)));
    private Task SendOrderConfirmationEmail(Order order, string trackAndTraceNumber)
        => Capture(() => bus.Publish(new SendOrderConfirmationEmail(order.OrderId, order.CustomerId, trackAndTraceNumber)));
    private Task CancelProductsShipment(Order order)
        => Capture(() => bus.Publish(new CancelProductsShipment(order.OrderId)));
    private Task CancelFundsReservation(Order order, Guid transactionId)
        => Capture(() => bus.Publish(new CancelFundsReservation(order.OrderId, transactionId)));
    private Task ReversePayment(Order order, Guid transactionId)
        => Capture(() => bus.Publish(new ReverseTransaction(order.OrderId, transactionId)));
    
    #endregion
    
    #region CleanUp

    private async Task CleanUp(FailedAt failedAt, Order order, Guid transactionId)
    {
        switch (failedAt) 
        {
            case FailedAt.FundsReserved:
                break;
            case FailedAt.ProductsShipped:
                await CancelFundsReservation(order, transactionId);
                break;
            case FailedAt.FundsCaptured:
                await CancelFundsReservation(order, transactionId);
                await CancelProductsShipment(order);
                break;
            case FailedAt.OrderConfirmationEmailSent:
                await ReversePayment(order, transactionId);
                await CancelProductsShipment(order);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(failedAt), failedAt, null);
        }

        throw new OrderProcessingException($"Order processing failed at: '{failedAt}'");
    }
    
    private enum FailedAt
    {
        FundsReserved,
        ProductsShipped,
        FundsCaptured,
        OrderConfirmationEmailSent,
    }

    #endregion
}
