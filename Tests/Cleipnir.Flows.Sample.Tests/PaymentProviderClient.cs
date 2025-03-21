using Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;

namespace Cleipnir.Flows.Sample.Tests;

public delegate Task Reserve(Guid transactionId, Guid customerId, decimal amount);
public delegate Task Capture(Guid transactionId);
public delegate Task CancelReservation(Guid transactionId);
public delegate Task Reverse(Guid transactionId);

public class PaymentProviderClientTestStub(
    Reserve reserve,
    Capture capture,
    CancelReservation cancelReservation,
    Reverse reverse
    ) : IPaymentProviderClient
{
    public Task Reserve(Guid transactionId, Guid customerId, decimal amount) => reserve(transactionId, customerId, amount);
    public Task Capture(Guid transactionId) => capture(transactionId);
    public Task CancelReservation(Guid transactionId) => cancelReservation(transactionId);
    public Task Reverse(Guid transactionId) => reverse(transactionId);

    public static PaymentProviderClientTestStub Create(
        Reserve? reserve = null,
        Capture? capture = null,
        CancelReservation? cancelReservation = null,
        Reverse? reverse = null
    ) => new(
        reserve ?? ((_, _, _) => Task.CompletedTask),
        capture ?? (_ => Task.CompletedTask),
        cancelReservation ?? (_ => Task.CompletedTask),
        reverse ?? (_ => Task.CompletedTask)
    );
}