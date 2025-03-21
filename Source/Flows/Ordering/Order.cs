namespace Cleipnir.Flows.Sample.Flows.Ordering;

public record Order(string OrderId, Guid CustomerId, IEnumerable<Guid> ProductIds, decimal TotalPrice);