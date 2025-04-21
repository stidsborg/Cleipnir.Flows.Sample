namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;

public interface IEmailClient
{
    Task SendOrderConfirmation(Guid customerId, TrackAndTrace trackAndTrace, IEnumerable<Guid> productIds);
    Task SendFollowUpEmail(Guid customerId);
}

public class EmailClientStub(ILogger<EmailClientStub>? logger) : IEmailClient
{
    public static readonly EmailClientStub Instance = new(null);
    
    public Task SendOrderConfirmation(Guid customerId, TrackAndTrace trackAndTrace, IEnumerable<Guid> productIds)
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ =>
            logger?.LogInformation("EMAIL_SERVER: Order confirmation emailed")
        );
    
    public Task SendFollowUpEmail(Guid customerId)
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ =>
            logger?.LogInformation("EMAIL_SERVER: Order confirmation emailed")
        );
}