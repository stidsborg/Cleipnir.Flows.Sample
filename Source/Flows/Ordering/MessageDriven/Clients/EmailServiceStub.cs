using MassTransit;

namespace Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven.Clients;

public class EmailServiceStub(EmailServiceFlows flows) : IConsumer<SendOrderConfirmationEmail>
{
    public async Task Consume(ConsumeContext<SendOrderConfirmationEmail> context)
    {
        await flows.Schedule(context.Message.OrderId, context.Message);
    }
}

[GenerateFlows]
public class EmailServiceFlow(IBus bus) : Flow<SendOrderConfirmationEmail>
{
    public override async Task Run(SendOrderConfirmationEmail command)
    {
        await Delay(TimeSpan.FromSeconds(1));
        await bus.Publish(new OrderConfirmationEmailSent(command.OrderId, command.CustomerId));
    }
}