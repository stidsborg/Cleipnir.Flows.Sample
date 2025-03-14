using Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven.Clients;
using MassTransit;

namespace Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven;

public class OrderHandler(MessageDrivenOrderFlows flows) : 
    IConsumer<FundsReserved>,
    IConsumer<FundsReservationFailed>,
    IConsumer<FundsCaptured>,
    IConsumer<FundsCaptureFailed>,
    IConsumer<ProductsShipped>,
    IConsumer<ProductsShipmentFailed>,
    IConsumer<OrderConfirmationEmailSent>,
    IConsumer<OrderConfirmationEmailFailed>
{
    public Task Consume(ConsumeContext<FundsReserved> context) => flows.SendMessage(context.Message.OrderId, context.Message);
    public Task Consume(ConsumeContext<FundsReservationFailed> context) => flows.SendMessage(context.Message.OrderId, context.Message);
    public Task Consume(ConsumeContext<FundsCaptured> context) => flows.SendMessage(context.Message.OrderId, context.Message);
    public Task Consume(ConsumeContext<FundsCaptureFailed> context) => flows.SendMessage(context.Message.OrderId, context.Message);
    public Task Consume(ConsumeContext<ProductsShipped> context) => flows.SendMessage(context.Message.OrderId, context.Message);
    public Task Consume(ConsumeContext<ProductsShipmentFailed> context) => flows.SendMessage(context.Message.OrderId, context.Message);
    public Task Consume(ConsumeContext<OrderConfirmationEmailSent> context) => flows.SendMessage(context.Message.OrderId, context.Message);
    public Task Consume(ConsumeContext<OrderConfirmationEmailFailed> context) => flows.SendMessage(context.Message.OrderId, context.Message);
}