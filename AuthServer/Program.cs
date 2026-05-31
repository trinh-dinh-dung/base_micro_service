using AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("AuthServerDb");
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        // Issuer cố định = URL browser dùng đăng nhập (token iss khớp khi service validate)
        var issuer = builder.Configuration["Auth:Issuer"] ?? "http://localhost:5010";
        options.SetIssuer(new Uri(issuer.TrimEnd('/') + "/"));

        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetTokenEndpointUris("/connect/token")
               .SetEndSessionEndpointUris("/connect/logout");

        options.AllowAuthorizationCodeFlow()
               .RequireProofKeyForCodeExchange();

        options.AllowRefreshTokenFlow();

        // Đăng ký scopes server hỗ trợ — phải khớp với scope React app yêu cầu
        options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "api");

        // API (Service_Base) validate JWT bằng JWKS — không giải mã JWE
        options.DisableAccessTokenEncryption();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough()
               .DisableTransportSecurityRequirement();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/account/login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddAuthorization();

var app = builder.Build();

await SeedDataAsync(app);

app.UseCors("AllowReactApp");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();

static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();

    var manager = scope.ServiceProvider.GetRequiredService<OpenIddict.Abstractions.IOpenIddictApplicationManager>();

    if (await manager.FindByClientIdAsync("react-app") == null)
    {
        await manager.CreateAsync(new OpenIddict.Abstractions.OpenIddictApplicationDescriptor
        {
            ClientId = "react-app",
            ClientType = OpenIddict.Abstractions.OpenIddictConstants.ClientTypes.Public,
            DisplayName = "React Application",
            ConsentType = OpenIddict.Abstractions.OpenIddictConstants.ConsentTypes.Implicit,
            RedirectUris =
            {
                new Uri("http://localhost:5173/callback"),
                new Uri("http://localhost:3000/callback")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:5173"),
                new Uri("http://localhost:3000")
            },
            Permissions =
            {
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddict.Abstractions.OpenIddictConstants.Permissions.Scopes.Roles,
                "scp:api"
            }
        });
    }
}
