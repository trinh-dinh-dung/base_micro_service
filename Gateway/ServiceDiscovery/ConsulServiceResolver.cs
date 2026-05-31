using Consul;

namespace Gateway.ServiceDiscovery;

/// <summary>
/// Resolves service addresses from Consul when UseConsul = true.
/// Falls back to static ReverseProxy config when Consul is unavailable.
/// </summary>
public class ConsulServiceResolver : IConsulServiceResolver, IDisposable
{
    private readonly IConsulClient _consul;
    private readonly ILogger<ConsulServiceResolver> _logger;

    public ConsulServiceResolver(IConfiguration configuration, ILogger<ConsulServiceResolver> logger)
    {
        _logger = logger;
        var address = configuration["ServiceDiscovery:ConsulAddress"] ?? "http://localhost:8500";
        _consul = new ConsulClient(cfg => cfg.Address = new Uri(address));
    }

    public async Task<string?> ResolveAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _consul.Health.Service(serviceName, tag: null, passingOnly: true, cancellationToken);
            var service = result.Response.FirstOrDefault();
            if (service == null) return null;

            var address = string.IsNullOrEmpty(service.Service.Address)
                ? service.Node.Address
                : service.Service.Address;
            return $"http://{address}:{service.Service.Port}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Consul resolution failed for service {ServiceName}", serviceName);
            return null;
        }
    }

    public void Dispose() => _consul.Dispose();
}
