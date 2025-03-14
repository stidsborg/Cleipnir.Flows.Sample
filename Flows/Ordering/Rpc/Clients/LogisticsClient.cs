namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;

public interface ILogisticsClient
{
    Task<TrackAndTrace> ShipProducts(Guid customerId, IEnumerable<Guid> productIds);
    Task CancelShipment(TrackAndTrace trackAndTrace);
}

public record TrackAndTrace(string Value);

public class LogisticsClientStub(ILogger<LogisticsClientStub> logger) : ILogisticsClient
{
    public Task<TrackAndTrace> ShipProducts(Guid customerId, IEnumerable<Guid> productIds)
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ =>
            {
                logger.LogInformation("LOGISTICS_SERVER: Products shipped");
                return new TrackAndTrace(Guid.NewGuid().ToString());
            }
        );
    
    public Task CancelShipment(TrackAndTrace trackAndTrace)
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ =>
            {
                logger.LogInformation("LOGISTICS_SERVER: Products shipment cancelled");
                return new TrackAndTrace(Guid.NewGuid().ToString());
            }
        );
}