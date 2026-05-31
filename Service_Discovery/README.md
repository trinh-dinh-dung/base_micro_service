# Service Discovery — Consul

Consul is used as the service registry for the microservice cluster. Services register themselves on startup and the Gateway can query Consul to resolve destination addresses.

## Start Consul

```bash
docker-compose up -d
```

Consul UI: http://localhost:8500/ui

## Service Registration

Each .NET service that wants to register with Consul should add the following code.

### 1. Install NuGet package

```
dotnet add package Consul
```

### 2. Register on startup (Program.cs)

```csharp
builder.Services.AddSingleton<IConsulClient>(_ =>
    new ConsulClient(cfg => cfg.Address = new Uri("http://localhost:8500")));

// After app.Build():
var consul = app.Services.GetRequiredService<IConsulClient>();
var registration = new AgentServiceRegistration
{
    ID   = "service-upload-1",
    Name = "service-upload",          // matches Gateway cluster ID
    Address = "host.docker.internal",
    Port = 5001,
    Check = new AgentServiceCheck
    {
        HTTP     = "http://host.docker.internal:5001/health",
        Interval = TimeSpan.FromSeconds(15),
        Timeout  = TimeSpan.FromSeconds(5)
    }
};
await consul.Agent.ServiceRegister(registration);

// Deregister on shutdown
app.Lifetime.ApplicationStopping.Register(() =>
    consul.Agent.ServiceDeregister(registration.ID).Wait());
```

### 3. Enable in Gateway

Set `ServiceDiscovery:UseConsul = true` in `Gateway/appsettings.json`:

```json
{
  "ServiceDiscovery": {
    "UseConsul": true,
    "ConsulAddress": "http://localhost:8500"
  }
}
```

When `UseConsul = false` (default), the Gateway uses the static `ReverseProxy.Clusters` addresses defined in its appsettings. This lets you run locally without Docker.

## Ports Reference

| Service           | Port  |
|-------------------|-------|
| Consul UI/API     | 8500  |
| Gateway (YARP)    | 5050  |
| AuthServer (SSO)  | 5010  |
| Service_Base      | 5000  |
| Service_Upload    | 5001  |
| React_App (dev)   | 5173  |
