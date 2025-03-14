namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;

public interface IPaymentProviderClient
{
    Task Reserve(Guid transactionId, Guid customerId, decimal amount);
    Task<TrackAndTrace> Capture(Guid transactionId);
    Task CancelReservation(Guid transactionId);
    Task Reverse(Guid transactionId);
}

public class PaymentProviderClientStub(ILogger<PaymentProviderClientStub> logger) : IPaymentProviderClient
{
    public Task Reserve(Guid transactionId, Guid customerId, decimal amount)
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ =>
            logger.LogInformation($"PAYMENT_PROVIDER: Reserved '{amount}'")
        );
    
    public Task<TrackAndTrace> Capture(Guid transactionId) 
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ =>
            {
                logger.LogInformation("PAYMENT_PROVIDER: Reserved amount captured");
                return new TrackAndTrace(Guid.NewGuid().ToString());
            }
        );
    public Task CancelReservation(Guid transactionId) 
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ => 
            logger.LogInformation("PAYMENT_PROVIDER: Reservation cancelled")
        );

    public Task Reverse(Guid transactionId)
        => Task.Delay(ClientSettings.Delay).ContinueWith(_ => 
            logger.LogInformation("PAYMENT_PROVIDER: Reservation reversed")
        );
}