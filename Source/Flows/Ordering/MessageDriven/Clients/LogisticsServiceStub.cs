using MassTransit;

namespace Cleipnir.Flows.Sample.Flows.Ordering.MessageDriven.Clients;

public class LogisticsServiceStub(LogisticsServiceFlows flows) : IConsumer<ShipProducts>
{
    public async Task Consume(ConsumeContext<ShipProducts> context)
    {
        var command = context.Message;
        await flows.Schedule(command.OrderId, command);
    }
}

[GenerateFlows]
public class LogisticsServiceFlow(IBus bus) : Flow<ShipProducts>
{
    public override async Task Run(ShipProducts command)
    {
        await Delay(TimeSpan.FromSeconds(1));
        await bus.Publish(new ProductsShipped(command.OrderId, TrackAndTraceNumber: Guid.NewGuid().ToString("N")));
    }
}