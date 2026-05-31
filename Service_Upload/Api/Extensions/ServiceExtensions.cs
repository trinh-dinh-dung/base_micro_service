using Application.Common.Converters;
using Application.IServices;
using Application.Services;
using AutoMapper;
using Evo.Mes.Template.Api.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDepartmentDapperService, DepartmentDapperService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IHomeService, HomeService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

        public static IServiceCollection AddRefitHttpClients(
            this IServiceCollection services,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            return services.AddRefitClients(configuration);
        }

        public static void ConfigureMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<DateTime?, int?>().ConvertUsing(new DateTimeTypeConverter());
                cfg.CreateMap<DateTime, int>().ConvertUsing(new DateTimeTypeConverter());
            }, AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
