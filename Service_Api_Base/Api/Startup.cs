using Api.Base;
using Api.Extensions;
using Application.Common.Appsetting;
using Application.GetMap;
using Application.IServices.DataConfig;
using Infrastructure.DataContext;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using static Application.Common.Status.TenantCommon;
using Microsoft.IdentityModel.Logging;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.ResponseCompression;
using Api.Base.JWT;
using Api.Base.Audit;
using Elastic.Apm.NetCoreAll;
using Serilog;

namespace Evo.Mes.Template.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>(); 
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
            });


            IdentityModelEventSource.ShowPII = true;
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            var configSettings = Configuration.GetSection("Appsettings").Get<Appsettings>();
            services.Configure<Appsettings>(Configuration.GetSection("Appsettings"));

            services.AddAuthorization(
            );

            services.AddInfrastructure();
            services.AddApplicationServices();
            services.AddRefitHttpClients(Configuration);

            // ── Elastic APM ──
            services.AddAllElasticApm();
            services.AddScoped<AuditInterceptor>();

            services.AddDbContext<PVIContext>((serviceProvider, dbContextBuilder) =>
            {
                var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
                dbContextBuilder.AddInterceptors(auditInterceptor);

                if (Environment.EnvironmentName == "Production_Custom")
                {
                    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    var TenantName = "";
                    if (string.IsNullOrEmpty(httpContextAccessor?.HttpContext?.Request?.Headers?["Authorization"]))
                        return;
                    else
                    {
                        var jwt = httpContextAccessor?.HttpContext?.Request?.Headers?["Authorization"].ToString();
                        var handler = new JwtSecurityTokenHandler();
                        var tokenS = handler.ReadJwtToken(jwt["Bearer ".Length..]);
                        //var tokenS = handler.ReadJwtToken(jwt.Substring("Bearer ".Length));
                        if (!string.IsNullOrEmpty(tokenS?.ToString()))
                            TenantName = tokenS.Claims.First(claim => claim.Type == "TenantName")?.Value;
                    }

                    if (!string.IsNullOrEmpty(TenantName))
                    {
                        var db_connect = GetDbByTenant(serviceProvider, TenantName);
                        if (!string.IsNullOrEmpty(db_connect))
                        {
                            dbContextBuilder.UseSqlServer(db_connect);
                        }
                    }
                }
                else
                {

                    dbContextBuilder.UseSqlServer(configSettings.DefaultConnection
                   //, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                   );
                }

            }, ServiceLifetime.Scoped);

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddJWTTokenServices(Configuration);
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                //options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var modelState = actionContext.ModelState;
                    //return new BadRequestObjectResult(string.Join(',', modelState.Values.SelectMany(x => x.Errors)
                    //    .Select(x => x.ErrorMessage)));
                    return Ok(new ResponseApi(true, true, Mess: string.Join(',', modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))));
                    //throw new AppException("Trạng thái không được null");
                };
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            }).AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Token Authentication API",
                    Description = "ASP.NET Core 5.0 Web API"
                });
                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
                //swagger.EnableAnnotations();
                // Include the XML comments
                swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Application.xml"));
            });
            services.ConfigureMapper();
            services.AddSingleton(Configuration);
            // add job service
            //services.AddHostedService<ConsumeScopedServiceHostedService>();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "vi", "en" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.LegacyRowVersionNullBehavior", false);
        }
        private IActionResult Ok(ResponseApi responseApi)
        {
            throw new NotImplementedException();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var configSettings = Configuration.GetSection("Appsettings").Get<Appsettings>();

            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Evo.Mes.Template.Api v1"));
            }
            app.UseResponseCompression();

            app.UsePathBase("/e-invoice-ho");
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Evo.Mes.Template.Api v1"));

            app.UseCors("MyPolicy");
            app.UseStaticFiles();
            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.Use((httpContext, next) =>
            {
                httpContext.Request.Scheme = "https";
                return next();
            });

            app.UseHttpsRedirection();

            var supportedCultures = new[] { "vi", "en" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseRequestCulture();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected string GetDbByTenant(IServiceProvider serviceProvider, string tenantName)
        {
            var dataConnectString = (IDataConfig)serviceProvider.GetService(typeof(IDataConfig));
            return dataConnectString.GetConnectStringByConnectName(string.Format("{0}_{1}", tenantName, TypeTenantConection.KeyNameDB_EVOMES_PRODUCTION_MANAGEMENT.ToLower()));
        }
    }

}



