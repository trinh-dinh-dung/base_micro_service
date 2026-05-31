# BASE — Microservice Architecture

> Tài liệu này mô tả toàn bộ kiến trúc, cấu trúc code, luồng hoạt động và quy tắc phát triển.
> Dùng làm tài liệu tham chiếu cho **developer** và **AI agent** khi làm việc trong repo này.

---

## Mục lục

1. [Tổng quan kiến trúc](#1-tổng-quan-kiến-trúc)
2. [Danh sách projects](#2-danh-sách-projects)
3. [Cấu trúc thư mục](#3-cấu-trúc-thư-mục)
4. [Luồng SSO & Auth](#4-luồng-sso--auth)
5. [API Gateway — YARP](#5-api-gateway--yarp)
6. [Service Discovery — Consul](#6-service-discovery--consul)
7. [Inter-Service Communication — Refit](#7-inter-service-communication--refit)
8. [Clean Architecture — Quy tắc tầng](#8-clean-architecture--quy-tắc-tầng)
9. [API Endpoints](#9-api-endpoints)
10. [Chạy local](#10-chạy-local)
11. [Chạy Docker Compose](#11-chạy-docker-compose)
12. [Tài khoản demo](#12-tài-khoản-demo)

---

## 1. Tổng quan kiến trúc

```
   ┌──────────────────────────────────────────────────────────────────┐
   │  Browser / React_App (:5173)                                     │
   │  oidc-client-ts: PKCE login → access_token (JWT)                 │
   └─────────┬──────────────────────────────┬────────────────────────┘
             │ 1. redirect login             │ 3. Bearer token + API call
             ▼                              ▼
   ┌─────────────────┐           ╔══════════════════════════╗
   │  AuthServer     │           ║  Gateway (:5050) — YARP  ║
   │  (:5010)        │           ║  - Verify JWT            ║
   │  OpenIddict SSO │           ║  - /api/base/*  → :5000  ║
   │  PKCE flow      │           ║  - /api/upload/*→ :5001  ║
   └────────┬────────┘           ╚═══════════╤══════════════╝
            │ 2. access_token                │
            └────────────────────────────────┘
                                             │ proxy (EXTERNAL traffic)
                              ┌──────────────┴──────────────┐
                              │                             |
                              ▼                             ▼
                   ┌──────────────────┐       ┌──────────────────────┐
                   │  Service_Base    │       │  Service_Upload      │
                   │  (:5000)         │       │  (:5001)             │
                   │  Clean Arch      │       │  Clean Arch + Upload │
                   └────────┬─────────┘       └──────────┬───────────┘
                            │                            │
                            │   Refit TRỰC TIẾP          │
                            │   (INTERNAL traffic,       │
                            │    KHÔNG qua Gateway)      │
                            └─────────────┬──────────────┘
                                          │
                                          │ query healthy instances
                                          ▼
                               ┌──────────────────────┐
                               │  Consul (:8500)      │
                               │  Service Registry    │
                               │  Round-Robin LB      │
                               │  Auto failover       │
                               └──────────────────────┘
```

### ⚡ Quy tắc quan trọng nhất

```
╔══════════════════════════════════════════════════════════════╗
║  Client bên ngoài  →  BẮT BUỘC qua Gateway (:5050)         ║
║  Service ↔ Service →  TRỰC TIẾP qua Refit + Consul         ║
╚══════════════════════════════════════════════════════════════╝
```

| Loại traffic | Đi qua | Lý do |
|---|---|---|
| React App → API | **Gateway** | Verify JWT user, single entry point, rate limit |
| Postman / bên ngoài → API | **Gateway** | Như trên |
| Service_Base → Service_Upload | **Trực tiếp** | Giảm latency, tránh phụ thuộc Gateway, Bearer token forward tự động |
| Service_Upload → Service_Base | **Trực tiếp** | Như trên |

**Tại sao KHÔNG đi qua Gateway cho internal?**
- Gateway verify JWT của **user** (issued cho React App) — Service không có token này
- Thêm hop không cần thiết → tăng latency
- Gateway down → toàn bộ service-to-service call chết
- Tạo circular dependency: `ServiceA → Gateway → ServiceB → Gateway → ServiceA`

**Luồng đầy đủ:**
1. React App gửi user đến AuthServer login (PKCE)
2. AuthServer trả `access_token` (JWT)
3. React App gọi **Gateway** với `Authorization: Bearer <token>`
4. Gateway verify JWT, proxy đến Service tương ứng
5. Service gọi chéo nhau **trực tiếp** qua Refit — `ForwardAuthorizationHandler` tự copy Bearer token
6. Consul cung cấp địa chỉ + Round-Robin load balancing khi `UseConsul=true`

---

## 2. Danh sách projects

| Thư mục | Công nghệ | Port local | Port Docker | Mô tả |
|---------|-----------|-----------|------------|-------|
| `Service_Base` | .NET 10, Clean Architecture | 5000 | 5000 | Backend API chính |
| `Service_Upload` | .NET 10, Clean Architecture | 5001 | 5001 | File upload/download service |
| `Gateway` | .NET 10, YARP 2.3 | 5050 | 5050 | API Gateway + JWT validation |
| `AuthServer` | .NET 10, OpenIddict 7.5 | 5010 | 5010 | SSO Authorization Server |
| `Service_Discovery` | Consul 1.18 (Go/Docker) | — | 8500 | Service registry |
| `React_App` | React 19, Vite, TypeScript | 5173 | 3000 | Frontend SPA |

---

## 3. Cấu trúc thư mục

### Service_Base & Service_Upload (Clean Architecture)

```
Api/                            ← Presentation layer
  Controllers/
    FileBridgeController.cs     ← Inject IDocumentService (không inject Refit trực tiếp)
    BaseBridgeController.cs     ← (Service_Upload) inject IFileMetadataService
  HttpClients/                  ← Infrastructure HTTP (chỉ Api layer biết Refit)
    IUploadServiceClient.cs     ← Refit interface → Service_Upload
    IBaseServiceClient.cs       ← Refit interface → Service_Base  (Service_Upload)
    FileStorageService.cs       ← Implements IFileStorageService bằng Refit
    BaseDataService.cs          ← Implements IBaseDataService bằng Refit
    ForwardAuthorizationHandler.cs  ← Tự động forward Bearer token ra ngoài
    ConsulAddressResolver.cs    ← Resolve địa chỉ từ Consul hoặc config
    ConsulDynamicBaseUrlHandler.cs  ← Ghi đè BaseAddress mỗi request (khi UseConsul=true)
    RefitClientExtensions.cs    ← DI registration cho toàn bộ Refit + adapters
  Extensions/
    ServiceExtensions.cs        ← AddApplicationServices, AddRefitHttpClients

Application/                    ← Use-case / Business Logic layer
  Abstractions/Services/
    IFileStorageService.cs      ← Interface thuần — Application KHÔNG biết Refit
    IBaseDataService.cs         ← Interface thuần — Application KHÔNG biết HTTP
  IServices/
    IDocumentService.cs         ← Interface nghiệp vụ tài liệu
    IFileMetadataService.cs     ← Interface nghiệp vụ metadata upload
  Services/
    Document/DocumentService.cs ← Nghiệp vụ: validate + upload + metadata, inject IFileStorageService
    Upload/FileMetadataService.cs ← Nghiệp vụ: gọi Service_Base 2 lần song song
  HttpClients/Models/
    UploadModels.cs             ← DTOs trả về từ Service_Upload
    BaseServiceModels.cs        ← DTOs trả về từ Service_Base

Domain/                         ← Entities, ValueObjects, Domain exceptions
Infrastructure/                 ← EF DbContext, Repositories, RabbitMQ, Redis
```

### Gateway

```
Gateway/
  Program.cs                    ← AddReverseProxy, JWT Bearer, optional Consul
  appsettings.json              ← Routes, Clusters, ServiceDiscovery config
  ServiceDiscovery/
    IConsulServiceResolver.cs   ← Interface resolve Consul
    ConsulServiceResolver.cs    ← Implement dùng Consul client
    ConsulClusterUpdater.cs     ← BackgroundService refresh YARP cluster từ Consul
    ConsulDestinationResolver.cs ← Extension helpers
```

### AuthServer

```
AuthServer/
  Program.cs                    ← OpenIddict config, seed client react-app
  Controllers/
    AuthorizationController.cs  ← /connect/authorize, /connect/token, /connect/logout
    AccountController.cs        ← /account/login (cookie-based login form)
  Views/Account/Login.cshtml    ← Login page UI
  Data/ApplicationDbContext.cs  ← EF InMemory DB cho OpenIddict entities
```

### React_App

```
React_App/src/
  auth/oidcConfig.ts            ← OIDC config (authority, client_id, redirect_uri...)
  api/gatewayClient.ts          ← Axios instance trỏ vào Gateway, setAuthToken()
  pages/
    HomePage.tsx                ← Dashboard: login/logout, ping services, show token
    CallbackPage.tsx            ← Xử lý redirect sau login
  App.tsx                       ← AuthProvider + BrowserRouter
  index.css                     ← Styles
.env                            ← VITE_AUTH_AUTHORITY, VITE_GATEWAY_URL, v.v.
```

---

## 4. Luồng SSO & Auth

### Authorization Code Flow + PKCE

```
1. React        GET  /connect/authorize?response_type=code&code_challenge=...
                     &client_id=react-app&redirect_uri=.../callback

2. AuthServer   → Chưa login? Redirect → /account/login
                → Login thành công → Set cookie session
                → Redirect về React: /callback?code=AUTH_CODE

3. React        POST /connect/token  { code, code_verifier, ... }
                ← { access_token, refresh_token, expires_in }

4. React        GET  /api/base/...
                Header: Authorization: Bearer <access_token>

5. Gateway      Verify JWT (Authority=AuthServer, ValidateAudience=false)
                → Proxy đến Service_Base
```

### JWT Claims mẫu (access_token)

```json
{
  "sub": "admin",
  "name": "admin",
  "email": "admin@example.com",
  "role": "Admin",
  "oi_tkn_id": "...",
  "iss": "http://localhost:5010"
}
```

---

## 5. API Gateway — YARP

### Route mapping

| Client gọi | Gateway route | Chuyển đến |
|---|---|---|
| `/api/base/{**path}` | service-base-route | `http://service-base:5000/{path}` |
| `/api/upload/{**path}` | service-upload-route | `http://service-upload:5001/{path}` |
| `/auth/{**path}` | auth-route | `http://auth-server:5010/{path}` |

### Bật/tắt Consul

```jsonc
// Gateway/appsettings.json

// Chế độ tĩnh (mặc định, không cần Consul)
"ServiceDiscovery": { "UseConsul": false },
"ReverseProxy": {
  "Clusters": {
    "service-base":   { "Destinations": { "primary": { "Address": "http://localhost:5000" } } },
    "service-upload": { "Destinations": { "primary": { "Address": "http://localhost:5001" } } }
  }
}

// Chế độ động (cần Consul đang chạy)
"ServiceDiscovery": { "UseConsul": true, "ConsulAddress": "http://localhost:8500" }
// → ConsulClusterUpdater (BackgroundService) refresh địa chỉ mỗi 30 giây
```

---

## 6. Service Discovery — Consul

Consul **không phải .NET** — là binary Go của HashiCorp, chạy qua Docker.

```bash
cd Service_Discovery
docker-compose up -d
# Consul UI: http://localhost:8500/ui
```

### Đăng ký service .NET vào Consul

Thêm vào `Program.cs` của từng service (Service_Base, Service_Upload):

```csharp
var consul = new ConsulClient(cfg => cfg.Address = new Uri("http://localhost:8500"));
await consul.Agent.ServiceRegister(new AgentServiceRegistration
{
    ID      = "service-base-1",
    Name    = "service-base",       // phải khớp với Cluster ID trong Gateway
    Address = "host.docker.internal",
    Port    = 5000,
    Check   = new AgentServiceCheck
    {
        HTTP     = "http://host.docker.internal:5000/health",
        Interval = TimeSpan.FromSeconds(15)
    }
});
```

> Xem chi tiết: `Service_Discovery/README.md`

---

## 7. Inter-Service Communication — Refit

### Refit gọi TRỰC TIẾP — KHÔNG qua Gateway

Đây là thiết kế có chủ đích. Hệ thống có **2 loại traffic tách biệt**:

```
EXTERNAL (từ client vào)          INTERNAL (service gọi service)
─────────────────────────         ──────────────────────────────
React / Postman / Browser         Service_Base  ←──Refit──►  Service_Upload
        │                                │                        │
        │  BẮT BUỘC qua Gateway          │   TRỰC TIẾP           │
        ▼                                │   (bypass Gateway)     │
     Gateway (:5050)                     ▼                        ▼
     - Verify JWT                  localhost:5000           localhost:5001
     - Single entry point          hoặc Consul-resolved     hoặc Consul-resolved
        │           │
        ▼           ▼
  Service_Base  Service_Upload
```

**Lý do KHÔNG đi qua Gateway:**

| Vấn đề | Nếu Internal đi qua Gateway |
|--------|----------------------------|
| **Latency** | Tăng 1 hop không cần thiết |
| **Availability** | Gateway down → toàn bộ internal call chết |
| **Auth vòng tròn** | Gateway verify JWT của user (issued cho React App), không phải của service → cần tạo thêm M2M token |
| **Circular dependency** | `Service_Base → Gateway → Service_Upload → Gateway → Service_Base` |

**Vì vậy:** External client **phải** qua Gateway, Internal Refit **bỏ qua** Gateway.

---

### Load Balancing & Service Registry

Đây là lý do **Consul giải quyết được cả 2 vấn đề** mà direct call có thể gặp:

#### Vấn đề 1: Load Balancing

```
Không có Consul (UseConsul=false):
  Service_Base → service-upload:5001  (1 địa chỉ cố định, không load balance)

Có Consul (UseConsul=true):
  Consul biết TẤT CẢ instances healthy:
    service-upload @ 10.0.0.1:5001  ← instance 1
    service-upload @ 10.0.0.2:5001  ← instance 2
    service-upload @ 10.0.0.3:5001  ← instance 3

  ConsulAddressResolver dùng Round-Robin:
    Request 1  → 10.0.0.1:5001
    Request 2  → 10.0.0.2:5001
    Request 3  → 10.0.0.3:5001
    Request 4  → 10.0.0.1:5001  (quay vòng)
    ...
    Instance 2 crash → Consul loại ra → chỉ còn 1 và 3
    Request 5  → 10.0.0.1:5001
    Request 6  → 10.0.0.3:5001  (tự failover)
```

Thuật toán hiện tại: **Round-Robin** (thread-safe, per service name).
Có thể đổi thành Random hoặc Least-Connection trong `ConsulAddressResolver.cs`.

#### Vấn đề 2: Không cần khai báo nhiều địa chỉ

```
UseConsul=false → Phải khai báo từng service:
  "ServiceAddresses": {
    "ServiceUpload":   "http://...",   ← cần biết địa chỉ
    "ServiceOrder":    "http://...",   ← cần biết địa chỉ
    "ServicePayment":  "http://...",   ← cần biết địa chỉ
    "ServiceNotify":   "http://...",   ← cần biết địa chỉ
    ...                                 vài chục service = vài chục dòng
  }

UseConsul=true → Chỉ cần tên service (đã đăng ký khi khởi động):
  // KHÔNG CẦN ServiceAddresses nữa
  // Service_Base chỉ cần biết tên "service-upload"
  // Consul tự trả về IP:Port hiện tại
```

#### Sơ đồ Consul Registry

```
Khi mỗi service khởi động, tự đăng ký vào Consul:

  Service_Base  khởi động → đăng ký "service-base"  @ :5000
  Service_Upload khởi động → đăng ký "service-upload" @ :5001
  ...thêm service mới...   → đăng ký "service-order"  @ :5002

Consul Registry:
  ┌─────────────────────────────────────────────┐
  │  service-base    [healthy] 10.0.0.1:5000    │
  │  service-upload  [healthy] 10.0.0.1:5001    │
  │                  [healthy] 10.0.0.2:5001    │  ← scale 2 instances
  │  service-order   [healthy] 10.0.0.1:5002    │
  └─────────────────────────────────────────────┘

Khi Service_Base cần gọi Service_Upload:
  → hỏi Consul: "service-upload healthy instances?"
  → Consul trả: [10.0.0.1:5001, 10.0.0.2:5001]
  → Round-Robin chọn 1 trong 2
  → Gọi trực tiếp (không qua Gateway)
```

#### Config trong `appsettings.json`

```jsonc
// Service_Base/Api/appsettings.json

"ServiceDiscovery": {
  "UseConsul": false,                       // true = bật Consul
  "ConsulAddress": "http://localhost:8500"
},

// Chỉ cần khi UseConsul=false (static fallback)
"ServiceAddresses": {
  "ServiceUpload": "http://localhost:5001"
  // Thêm service mới: "ServiceOrder": "http://localhost:5002"
}
// Khi UseConsul=true: ServiceAddresses chỉ là fallback khi Consul down
```

---

### Nguyên tắc kiến trúc Refit

```
Controller
  │
  ├─► inject IDocumentService      ← nghiệp vụ phức tạp (validate + upload + DB)
  │          │
  │          └─► inject IFileStorageService   ← Application interface (không biết HTTP)
  │                         │
  │                         └─► FileStorageService  ← Api adapter
  │                                    │
  │                                    └─► IUploadServiceClient (Refit)
  │                                              │ HTTP TRỰC TIẾP
  │                                              ▼
  │                                        Service_Upload (:5001)
  │
  └─► inject IFileStorageService trực tiếp  ← thao tác đơn giản, không cần biz logic
```

**Quy tắc bắt buộc:**
- `Application` layer **KHÔNG** được import `Refit` — chỉ dùng interface thuần
- `Api` layer chứa toàn bộ code liên quan Refit (interface, implementation, DI)
- `IFileStorageService`, `IBaseDataService` là **cầu nối** giữa Application và Refit

### Khi nào Controller inject gì

```csharp
// ✅ Controller cần nghiệp vụ (validate + upload + lưu DB + xử lý lỗi biz)
public class FileBridgeController(IDocumentService documentService) { ... }

// ✅ Controller chỉ cần 1 thao tác đơn giản, không có biz logic
public class QuickController(IFileStorageService fileStorage) { ... }

// ❌ Sai — Controller không được inject Refit trực tiếp
public class WrongController(IUploadServiceClient refitClient) { ... }
```

### Forward Bearer Token tự động

`ForwardAuthorizationHandler` (DelegatingHandler) tự động copy header
`Authorization: Bearer <token>` từ request gốc (từ React qua Gateway) vào mọi
HTTP call Refit gửi đến service khác. Service nhận có thể dùng token đó để
biết danh tính người dùng gốc.

```
React → Gateway → Service_Base                Service_Upload
              Bearer: <token_A>         Refit ──────────────►
                                        Header tự động:
                                        Bearer: <token_A>  (forward)
```

### Gọi service nhiều lần trong một nghiệp vụ

```csharp
// Lần lượt (khi lần 2 phụ thuộc kết quả lần 1)
public async Task RemoveFileAsync(string fileName, CancellationToken ct)
{
    var files = await _fileStorage.GetFilesAsync(ct);   // lần 1: kiểm tra tồn tại
    if (!files.Any(f => f.Name == fileName))
        throw new AppException("File không tồn tại.");

    await _fileStorage.DeleteAsync(fileName, ct);        // lần 2: xóa
}

// Song song (khi 2 lần gọi độc lập nhau)
public async Task<FileWithMetadata> UploadWithMetadataAsync(...)
{
    var userTask = _baseData.GetUserAsync(uploaderId, ct);     // lần 1 ─┐ song
    var deptTask = _baseData.GetDepartmentsAsync(ct);          // lần 2 ─┘ song
    await Task.WhenAll(userTask, deptTask);                    // đợi cả 2
}
```

---

## 8. Clean Architecture — Quy tắc tầng

```
Api (Presentation)          ← Controllers, Startup, Middleware, Refit adapters
    ↓ depends on
Application (Use Cases)     ← IServices, Services, Abstractions, DTOs, Exceptions
    ↓ depends on
Domain (Core)               ← Entities, ValueObjects, Domain exceptions
    ↑ depends on (ngược chiều)
Infrastructure (Adapters)   ← EF DbContext, Repositories, RabbitMQ, Redis
```

**Dependency rule:** tầng trong KHÔNG được import tầng ngoài.

| Tầng | Được import | KHÔNG được import |
|------|------------|-----------------|
| Domain | (không ai) | Application, Infrastructure, Api |
| Application | Domain | Infrastructure, Api, Refit, EF |
| Infrastructure | Application, Domain | Api |
| Api | Application, Infrastructure, Domain | (tất cả đều OK) |

---

## 9. API Endpoints

### Qua Gateway (:5050)

#### Service_Base (`/api/base/...`)

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| GET | `/api/base/api/Home` | Health check | No |
| GET | `/api/base/api/Department` | Danh sách phòng ban | No |
| POST | `/api/base/api/Department` | Tạo phòng ban | Yes |
| GET | `/api/base/api/User` | Danh sách user | No |
| POST | `/api/base/api/FileBridge/attach` | Attach file vào entity (proxy → Upload) | No |
| GET | `/api/base/api/FileBridge/files` | File của entity (proxy → Upload) | No |
| DELETE | `/api/base/api/FileBridge/files/{name}` | Xóa file (proxy → Upload) | Yes |

#### Service_Upload (`/api/upload/...`)

| Method | Path | Mô tả | Auth |
|--------|------|-------|------|
| POST | `/api/upload/api/FileUpload/upload` | Upload file (multipart/form-data) | No |
| GET | `/api/upload/api/FileUpload/list` | Danh sách file | No |
| DELETE | `/api/upload/api/FileUpload/{name}` | Xóa file | Yes |
| POST | `/api/upload/api/BaseBridge/upload-with-meta` | Upload + enrich từ Service_Base | No |

### Trực tiếp (không qua Gateway)

| Service | URL | Ghi chú |
|---------|-----|---------|
| AuthServer | `http://localhost:5010/.well-known/openid-configuration` | OIDC discovery |
| AuthServer | `http://localhost:5010/account/login` | Login page |
| Service_Base | `http://localhost:5000/swagger` | Swagger UI |
| Service_Upload | `http://localhost:5001/swagger` | Swagger UI |
| Consul | `http://localhost:8500/ui` | Service registry UI |

---

## 10. Chạy local

> Chạy theo thứ tự: AuthServer → Gateway → Service_Base → Service_Upload → React_App

```bash
# 1. AuthServer (SSO)
cd AuthServer && dotnet run
# http://localhost:5010

# 2. Gateway (YARP)
cd Gateway && dotnet run
# http://localhost:5050

# 3. Service_Base
cd Service_Base && dotnet run --project Api
# http://localhost:5000/swagger

# 4. Service_Upload
cd Service_Upload && dotnet run --project Api
# http://localhost:5001/swagger

# 5. React App
cd React_App && npm run dev
# http://localhost:5173

# 6. (Optional) Consul
cd Service_Discovery && docker-compose up -d
# http://localhost:8500/ui
```

### Environment variables (local)

Các service đọc config từ `appsettings.json` và `appsettings.Development.json`.
Tạo `.env` ở root để override khi chạy Docker:

```bash
cp .env.example .env
# Chỉnh sửa DB connection string trong .env
```

---

## 11. Chạy Docker Compose

```bash
# Toàn bộ stack (build + run)
docker-compose up -d --build

# Xem log
docker-compose logs -f
docker-compose logs -f gateway

# Dừng
docker-compose down

# Dừng + xóa volume
docker-compose down -v
```

### Ports sau khi chạy Docker

| URL | Service |
|-----|---------|
| `http://localhost:3000` | React App |
| `http://localhost:5010` | AuthServer |
| `http://localhost:5050` | Gateway |
| `http://localhost:5000/swagger` | Service_Base |
| `http://localhost:5001/swagger` | Service_Upload |
| `http://localhost:8500/ui` | Consul |

### Bật Consul trong Docker

Trong `docker-compose.yml`, đổi:
```yaml
- ServiceDiscovery__UseConsul=false
```
thành:
```yaml
- ServiceDiscovery__UseConsul=true
```

---

## 12. Tài khoản demo

### AuthServer (OpenIddict)

| Username | Password | Role |
|----------|----------|------|
| `admin` | `admin123` | Admin |
| `user` | `user123` | User |

### OIDC Client (React App)

| Field | Value |
|-------|-------|
| `client_id` | `react-app` |
| `client_type` | Public (no secret) |
| `grant_type` | Authorization Code + PKCE |
| `redirect_uri` | `http://localhost:5173/callback` |
| `scopes` | `openid profile email api` |

---

## Ghi chú cho AI Agent

Khi làm việc trong repo này, cần nắm:

- **Thêm nghiệp vụ mới** liên quan cross-service: thêm method vào `IFileStorageService` hoặc `IBaseDataService` (Application), implement trong `FileStorageService`/`BaseDataService` (Api/HttpClients), thêm method vào Refit interface tương ứng.
- **Thêm endpoint mới** trong Service_Base: tạo controller, inject `IDocumentService` (hoặc Application service phù hợp), KHÔNG inject Refit interface trực tiếp.
- **Thêm service mới**: tạo thư mục mới (copy structure từ Service_Base), thêm Refit interface trong service cần gọi, tạo adapter implements Application interface, đăng ký trong `RefitClientExtensions.cs`, thêm route trong `Gateway/appsettings.json`.
- **Consul không phải .NET**: chỉ cần chạy Docker. Code .NET dùng NuGet `Consul` để query/register.
- **Bearer token**: `ForwardAuthorizationHandler` xử lý tự động — không cần truyền token thủ công khi dùng Refit client.
