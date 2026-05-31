using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.IServices.DataConfig;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRabbitMQClient, RabbitMQClient>();
            services.AddScoped<IDataConfig, DataConfigService>();
            services.AddSingleton<IDbConnectionFactory, DapperConnectionFactory>();

            return services;
        }
    }
}
