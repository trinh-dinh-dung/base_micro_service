using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Gateway.ServiceDiscovery;

/// <summary>
/// YARP destination resolver that queries Consul for healthy service instances.
/// Registered via AddServiceDiscoveryDestinationResolver().
/// </summary>
public static class ConsulDestinationResolverExtensions
{
    public static IReverseProxyBuilder AddServiceDiscoveryDestinationResolver(
        this IReverseProxyBuilder builder)
    {
        builder.Services.AddSingleton<IConsulProxyConfigFilter, ConsulProxyConfigFilter>();
        return builder;
    }
}

public interface IConsulProxyConfigFilter
{
    Task<IReadOnlyList<DestinationConfig>> ResolveDestinationsAsync(
        string clusterId, IReadOnlyDictionary<string, DestinationConfig> destinations,
        CancellationToken cancellationToken);
}

public class ConsulProxyConfigFilter : IConsulProxyConfigFilter
{
    private readonly IConsulServiceResolver _resolver;
    private readonly ILogger<ConsulProxyConfigFilter> _logger;

    public ConsulProxyConfigFilter(IConsulServiceResolver resolver, ILogger<ConsulProxyConfigFilter> logger)
    {
        _resolver = resolver;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DestinationConfig>> ResolveDestinationsAsync(
        string clusterId,
        IReadOnlyDictionary<string, DestinationConfig> destinations,
        CancellationToken cancellationToken)
    {
        var consulAddress = await _resolver.ResolveAsync(clusterId, cancellationToken);
        if (consulAddress == null)
        {
            _logger.LogWarning("Consul returned no address for {ClusterId}. Using static config.", clusterId);
            return destinations.Values.ToList();
        }

        return new List<DestinationConfig>
        {
            new() { Address = consulAddress }
        };
    }
}
