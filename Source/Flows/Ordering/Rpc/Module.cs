using Cleipnir.Flows.Sample.Flows.Ordering.Rpc.Clients;

namespace Cleipnir.Flows.Sample.Flows.Ordering.Rpc;

public static class Module
{
    public static IServiceCollection AddRpcOrderFlows(this IServiceCollection services)
    {
        services.AddSingleton<IEmailClient, EmailClientStub>();
        services.AddSingleton<ILogisticsClient, LogisticsClientStub>();
        services.AddSingleton<IPaymentProviderClient, PaymentProviderClientStub>();
        
        return services;
    } 
}