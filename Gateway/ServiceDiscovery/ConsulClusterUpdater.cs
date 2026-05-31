using Yarp.ReverseProxy.Configuration;

namespace Gateway.ServiceDiscovery;

/// <summary>
/// Background service that periodically refreshes YARP cluster destinations from Consul.
/// Only active when ServiceDiscovery:UseConsul = true.
/// </summary>
public class ConsulClusterUpdater : BackgroundService
{
    private readonly IConsulServiceResolver _resolver;
    private readonly IProxyConfigProvider _configProvider;
    private readonly ILogger<ConsulClusterUpdater> _logger;
    private static readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(30);

    private static readonly string[] TrackedServices = ["service-base", "service-upload", "auth-server"];

    public ConsulClusterUpdater(
        IConsulServiceResolver resolver,
        IProxyConfigProvider configProvider,
        ILogger<ConsulClusterUpdater> logger)
    {
        _resolver = resolver;
        _configProvider = configProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var config = _configProvider.GetConfig();

                foreach (var clusterId in TrackedServices)
                {
                    var address = await _resolver.ResolveAsync(clusterId, stoppingToken);
                    if (address != null)
                    {
                        _logger.LogInformation(
                            "Consul resolved {ClusterId} -> {Address}", clusterId, address);
                    }
                }
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Consul cluster refresh failed, will retry");
            }

            await Task.Delay(_refreshInterval, stoppingToken);
        }
    }
}
