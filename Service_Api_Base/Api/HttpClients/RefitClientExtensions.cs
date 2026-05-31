using Application.Abstractions.Services;
using Application.IServices;
using Application.Services.Document;
using Consul;
using Refit;

namespace Evo.Mes.Template.Api.HttpClients;

public static class RefitClientExtensions
{
    /// <summary>
    /// Đăng ký tất cả Refit HTTP clients cho Service_Base.
    ///
    /// Chế độ địa chỉ:
    ///   UseConsul=false → ServiceAddresses:ServiceUpload (static, default)
    ///   UseConsul=true  → query Consul health API để resolve địa chỉ động
    ///                     + fallback về static nếu Consul không có instance healthy
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

        // ── IUploadServiceClient ────────────────────────────────
        var uploadBaseUrl = config["ServiceAddresses:ServiceUpload"] ?? "http://localhost:5001";

        var uploadBuilder = services
            .AddRefitClient<IUploadServiceClient>(new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer()
            })
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(uploadBaseUrl))
            .AddHttpMessageHandler<ForwardAuthorizationHandler>();

        if (config.GetValue<bool>("ServiceDiscovery:UseConsul"))
        {
            uploadBuilder.AddHttpMessageHandler(sp =>
                new ConsulDynamicBaseUrlHandler<IUploadServiceClient>(
                    sp.GetRequiredService<IServiceAddressResolver>(),
                    serviceName: "service-upload",
                    configKey: "ServiceAddresses:ServiceUpload"));
        }

        // ── Adapter: IFileStorageService → FileStorageService (dùng Refit) ──
        // Application services inject IFileStorageService, không biết Refit.
        services.AddScoped<IFileStorageService, FileStorageService>();

        // ── Application service dùng IFileStorageService ──────────────────
        services.AddScoped<IDocumentService, DocumentService>();

        return services;
    }
}
