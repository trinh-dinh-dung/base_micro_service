using Elastic.Apm.NetCoreAll;
using Gateway.ServiceDiscovery;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var useConsul = builder.Configuration.GetValue<bool>("ServiceDiscovery:UseConsul");

if (useConsul)
{
    builder.Services.AddSingleton<IConsulServiceResolver, ConsulServiceResolver>();
    builder.Services.AddHostedService<ConsulClusterUpdater>();
}

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ── Elastic APM ──
builder.Services.AddAllElasticApm();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Middleware: log traceparent để trace request từ FE qua Gateway đến service
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var traceparent = context.Request.Headers["traceparent"].FirstOrDefault();
    if (!string.IsNullOrEmpty(traceparent))
    {
        logger.LogInformation("[Gateway] {Method} {Path} traceparent={Traceparent}",
            context.Request.Method, context.Request.Path, traceparent);
    }
    await next();
});

// Health check endpoint — dùng cho Docker healthcheck và load balancer
app.MapHealthChecks("/health");

// Gateway info endpoint
app.MapGet("/", () => Results.Ok(new
{
    service = "API Gateway",
    status = "running",
    timestamp = DateTime.UtcNow
}));

app.MapReverseProxy();

app.Run();
