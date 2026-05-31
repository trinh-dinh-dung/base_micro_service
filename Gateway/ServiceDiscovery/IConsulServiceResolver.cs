namespace Gateway.ServiceDiscovery;

public interface IConsulServiceResolver
{
    Task<string?> ResolveAsync(string serviceName, CancellationToken cancellationToken = default);
}
