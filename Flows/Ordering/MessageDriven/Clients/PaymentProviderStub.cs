using MassTransit;

namespace Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven.Clients;

public class PaymentProviderStub(PaymentProviderFlows flows) : 
    IConsumer<ReserveFunds>,
    IConsumer<CaptureFunds>,
    IConsumer<CancelFundsReservation>
{
    private async Task MessageHandler(EventsAndCommands message, string orderId)
    {
        await flows.SendMessage(orderId, message);
    }

    public Task Handle(ReserveFunds message) => MessageHandler(message, message.OrderId);
    public Task Handle(CaptureFunds message) => MessageHandler(message, message.OrderId);
    public Task Handle(CancelFundsReservation message) => MessageHandler(message, message.OrderId);
    public Task Consume(ConsumeContext<ReserveFunds> context) => Handle(context.Message);
    public Task Consume(ConsumeContext<CaptureFunds> context) => Handle(context.Message);
    public Task Consume(ConsumeContext<CancelFundsReservation> context) => Handle(context.Message);
}

[GenerateFlows]
public class PaymentProviderFlow(IBus bus) : Flow
{
    public override async Task Run()
    {
        await foreach (var msg in Messages)
        {
            await (msg switch
            {
                CaptureFunds captureFunds => Capture(() => bus.Publish(new FundsCaptured(captureFunds.OrderId))),
                ReserveFunds reserveFunds => Capture(() => bus.Publish(new FundsReserved(reserveFunds.OrderId))),
                CancelFundsReservation cancelFundsReservation => Capture(() => bus.Publish(new FundsReservationCancelled(cancelFundsReservation.OrderId))),
                _ => Task.CompletedTask
            });
        }
    }
}