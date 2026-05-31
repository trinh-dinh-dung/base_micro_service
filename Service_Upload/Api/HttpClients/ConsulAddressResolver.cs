using Consul;
using System.Collections.Concurrent;

namespace Evo.Mes.Template.Api.HttpClients;

public interface IServiceAddressResolver
{
    /// <summary>
    /// Trả về base URL của một instance healthy.
    /// Khi UseConsul=true: lấy tất cả instance healthy từ Consul rồi chọn theo
    /// thuật toán Round-Robin → mỗi lần gọi sẽ chọn instance khác nhau.
    /// Khi UseConsul=false: đọc địa chỉ tĩnh từ appsettings (configKey).
    /// </summary>
    Task<string> ResolveAsync(string serviceName, string configKey, CancellationToken ct = default);
}

/// <summary>
/// Resolve địa chỉ service với Round-Robin load balancing qua Consul.
///
/// Khi UseConsul=true và Consul có nhiều instance của cùng 1 service:
///   - Lấy tất cả instance passing health check
///   - Chọn theo Round-Robin (counter per serviceName, thread-safe)
///   - Fallback về config tĩnh nếu Consul không có instance nào healthy
///
/// Khi UseConsul=false:
///   - Dùng địa chỉ tĩnh trong appsettings (ServiceAddresses:xxx)
///   - Không cần Consul đang chạy
/// </summary>
public class ConsulAddressResolver : IServiceAddressResolver
{
    private readonly IConfiguration _config;
    private readonly ILogger<ConsulAddressResolver> _logger;
    private readonly IConsulClient? _consul;
    private readonly bool _useConsul;

    // Counter thread-safe cho Round-Robin, mỗi serviceName có counter riêng
    private readonly ConcurrentDictionary<string, int> _roundRobinCounters = new();

    public ConsulAddressResolver(IConfiguration config, ILogger<ConsulAddressResolver> logger)
    {
        _config = config;
        _logger = logger;
        _useConsul = config.GetValue<bool>("ServiceDiscovery:UseConsul");

        if (_useConsul)
        {
            var consulAddress = config["ServiceDiscovery:ConsulAddress"] ?? "http://localhost:8500";
            _consul = new ConsulClient(cfg => cfg.Address = new Uri(consulAddress));
            _logger.LogInformation("Consul mode enabled → {Address}", consulAddress);
        }
    }

    public async Task<string> ResolveAsync(string serviceName, string configKey, CancellationToken ct = default)
    {
        if (_useConsul && _consul != null)
        {
            try
            {
                // Lấy TẤT CẢ instances healthy của service
                var result = await _consul.Health.Service(
                    serviceName, tag: null, passingOnly: true, ct);

                var instances = result.Response;

                if (instances.Length > 0)
                {
                    // Round-Robin: lấy counter hiện tại rồi tăng lên
                    var counter = _roundRobinCounters.AddOrUpdate(
                        serviceName,
                        addValue: 0,
                        updateValueFactory: (_, current) => (current + 1) % instances.Length);

                    var selected = instances[counter];
                    var addr = string.IsNullOrEmpty(selected.Service.Address)
                        ? selected.Node.Address
                        : selected.Service.Address;
                    var url = $"http://{addr}:{selected.Service.Port}";

                    _logger.LogDebug(
                        "Consul Round-Robin [{Index}/{Total}] {Service} → {Url}",
                        counter + 1, instances.Length, serviceName, url);

                    return url;
                }

                _logger.LogWarning(
                    "Consul: 0 healthy instances for '{Service}', fallback to config", serviceName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Consul lookup failed for '{Service}', fallback to config", serviceName);
            }
        }

        // Fallback: đọc từ appsettings
        var staticUrl = _config[configKey];
        if (string.IsNullOrEmpty(staticUrl))
            throw new InvalidOperationException(
                $"Cannot resolve '{serviceName}'. " +
                $"Set '{configKey}' in appsettings or register service in Consul.");

        return staticUrl;
    }
}
