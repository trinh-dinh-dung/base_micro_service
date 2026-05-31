namespace Evo.Mes.Template.Api.HttpClients;

/// <summary>
/// DelegatingHandler: ghi đè BaseAddress của mỗi HTTP request bằng địa chỉ
/// được resolve từ Consul (Round-Robin) hoặc config tĩnh.
///
/// Chỉ active khi ServiceDiscovery:UseConsul = true.
/// Khi UseConsul=false, Refit dùng BaseAddress tĩnh từ RefitClientExtensions.
///
/// Lợi ích khi UseConsul=true:
///   - Load balancing: mỗi request có thể đến instance khác nhau
///   - Không cần khai báo địa chỉ từng service trong config
///   - Tự động failover: instance unhealthy bị Consul loại ra
/// </summary>
public class ConsulDynamicBaseUrlHandler<TClient> : DelegatingHandler
{
    private readonly IServiceAddressResolver _resolver;
    private readonly string _serviceName;
    private readonly string _configKey;

    public ConsulDynamicBaseUrlHandler(
        IServiceAddressResolver resolver,
        string serviceName,
        string configKey)
    {
        _resolver = resolver;
        _serviceName = serviceName;
        _configKey = configKey;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Resolve địa chỉ mỗi request → Round-Robin tự động phân phối
        var baseUrl = await _resolver.ResolveAsync(_serviceName, _configKey, cancellationToken);
        var baseUri = new Uri(baseUrl.TrimEnd('/') + "/");

        if (request.RequestUri != null)
        {
            // Giữ nguyên path + query, chỉ thay host:port
            var relativeUri = baseUri.MakeRelativeUri(request.RequestUri).ToString();
            if (Uri.TryCreate(baseUri, relativeUri, out var newUri))
                request.RequestUri = newUri;
            else
                request.RequestUri = new Uri(baseUri, request.RequestUri.PathAndQuery.TrimStart('/'));
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
