using Application.Abstractions.Services;
using Application.IServices;
using Application.Services.Upload;
using Consul;
using Refit;

namespace Evo.Mes.Template.Api.HttpClients;

public static class RefitClientExtensions
{
    /// <summary>
    /// Đăng ký tất cả Refit HTTP clients cho Service_Upload.
    ///
    /// Chế độ địa chỉ:
    ///   UseConsul=false → ServiceAddresses:ServiceBase (static, default)
    ///   UseConsul=true  → query Consul health API để resolve địa chỉ động
    /// </summary>
    public static IServiceCollection AddRefitClients(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddTransient<ForwardAuthorizationHandler>();
        services.AddSingleton<IServiceAddressResolver, ConsulAddressResolver>();

        if (config.GetValue<bool>("ServiceDiscovery:UseConsul"))
        {
            var consulAddress = config["ServiceDiscovery:ConsulAddress"] ?? "http://localhost:8500";
            services.AddSingleton<IConsulClient>(_ =>
                new ConsulClient(cfg => cfg.Address = new Uri(consulAddress)));
        }

        var baseServiceUrl = config["ServiceAddresses:ServiceBase"] ?? "http://localhost:5000";

        var baseBuilder = services
            .AddRefitClient<IBaseServiceClient>(new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer()
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseServiceUrl))
            .AddHttpMessageHandler<ForwardAuthorizationHandler>();

        if (config.GetValue<bool>("ServiceDiscovery:UseConsul"))
        {
            baseBuilder.AddHttpMessageHandler(sp =>
                new ConsulDynamicBaseUrlHandler<IBaseServiceClient>(
                    sp.GetRequiredService<IServiceAddressResolver>(),
                    serviceName: "service-base",
                    configKey: "ServiceAddresses:ServiceBase"));
        }

        // ── Adapter: IBaseDataService → BaseDataService (dùng Refit) ─────
        services.AddScoped<IBaseDataService, BaseDataService>();

        // ── Application service dùng IBaseDataService ─────────────────────
        services.AddScoped<IFileMetadataService, FileMetadataService>();

        return services;
    }
}
