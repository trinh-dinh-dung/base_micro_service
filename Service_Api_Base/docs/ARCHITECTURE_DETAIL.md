# TÀI LIỆU KIẾN TRÚC DỰ ÁN BE-EF

> **Phiên bản:** 1.0  
> **Ngày cập nhật:** 28/05/2026  
> **Framework:** .NET 10 — Clean Architecture  
> **Database:** PostgreSQL  

---

## MỤC LỤC

1. [Tổng quan dự án](#1-tổng-quan-dự-án)
2. [Kiến trúc tổng thể](#2-kiến-trúc-tổng-thể)
3. [Cấu trúc Solution](#3-cấu-trúc-solution)
4. [Tầng Domain](#4-tầng-domain)
5. [Tầng Application](#5-tầng-application)
6. [Tầng Infrastructure](#6-tầng-infrastructure)
7. [Tầng API (Presentation)](#7-tầng-api-presentation)
8. [Cơ sở dữ liệu](#8-cơ-sở-dữ-liệu)
9. [Xác thực & Phân quyền](#9-xác-thực--phân-quyền)
10. [Middleware & Cross-Cutting Concerns](#10-middleware--cross-cutting-concerns)
11. [Hệ thống Messaging (RabbitMQ)](#11-hệ-thống-messaging-rabbitmq)
12. [Caching (Redis)](#12-caching-redis)
13. [Logging & Monitoring](#13-logging--monitoring)
14. [API Endpoints](#14-api-endpoints)
15. [Cấu hình hệ thống](#15-cấu-hình-hệ-thống)
16. [DevOps & Triển khai](#16-devops--triển-khai)
17. [Quy trình phát triển](#17-quy-trình-phát-triển)
18. [Phụ lục](#18-phụ-lục)

---

## 1. TỔNG QUAN DỰ ÁN

### 1.1 Mô tả

**BE-EF** là một dự án backend API xây dựng trên nền tảng .NET 10, áp dụng kiến trúc **Clean Architecture** thực dụng kết hợp **DB-first**. Dự án phục vụ như một template/framework chuẩn cho việc phát triển các microservice trong hệ thống.

### 1.2 Công nghệ sử dụng

| Thành phần | Công nghệ | Phiên bản |
|------------|-----------|-----------|
| Framework | .NET | 10.0 |
| SDK | .NET SDK | 10.0.200 |
| ORM (chính) | Entity Framework Core | 10.0.8 |
| ORM (phụ) | Dapper | 2.1.66 |
| Database | PostgreSQL (Npgsql) | 10.0.1 / 10.0.2 |
| Authentication | JWT Bearer + Keycloak | 10.0.8 / 3.0.0 |
| Messaging | RabbitMQ.Client | 6.8.1 |
| Cache | StackExchange.Redis | 2.8.16 |
| Logging | Serilog + Elasticsearch | 10.0.0 |
| API Documentation | Swashbuckle (Swagger) | 10.1.7 |
| Object Mapping | AutoMapper | 16.1.1 |
| Excel Export | ClosedXML / EPPlus | 0.104.2 / 7.0.2 |
| HTTP Client | RestSharp | 106.12.0 |
| Serialization | Newtonsoft.Json | 13.0.4 |

### 1.3 Mục tiêu kiến trúc

- **Tách biệt rõ ràng** giữa các tầng (Separation of Concerns)
- **Phát triển nhanh** — 80% công việc tập trung ở tầng Application
- **Hỗ trợ đa persistence** — EF Core cho CRUD chuẩn, Dapper cho query phức tạp
- **Dễ mở rộng** — Generic Repository, DI, Abstraction interfaces
- **Sẵn sàng production** — Error handling, Logging, JWT Auth, Multi-tenant

---

## 2. KIẾN TRÚC TỔNG THỂ

### 2.1 Mô hình Clean Architecture

Dự án áp dụng mô hình **Clean Architecture (Onion Architecture)** với 4 tầng:

```
┌─────────────────────────────────────────────────┐
│                   API Layer                     │
│        (Controllers, Middleware, JWT)            │
├─────────────────────────────────────────────────┤
│              Application Layer                  │
│     (Services, Abstractions, DTOs, Exceptions)  │
├─────────────────────────────────────────────────┤
│            Infrastructure Layer                 │
│   (EF Context, Repositories, RabbitMQ, Redis)   │
├─────────────────────────────────────────────────┤
│               Domain Layer                      │
│      (Entities, Aggregates, Value Objects)       │
└─────────────────────────────────────────────────┘
```

### 2.2 Quy tắc phụ thuộc

| Từ → Đến | Cho phép? | Ghi chú |
|-----------|-----------|---------|
| Api → Application | ✅ Có | Controller inject `IServices` |
| Api → Infrastructure | ✅ Có | Chỉ để đăng ký DI trong Startup |
| Application → Domain | ✅ Có | Service sử dụng Aggregate + Entity |
| Application → Infrastructure | ❌ **Không** | Phải đi qua Abstraction |
| Infrastructure → Application | ✅ Có | Implement các interface Abstraction |
| Infrastructure → Domain | ✅ Có | EF mapping Entity |
| Domain → bất kỳ tầng nào | ❌ **Không** | Domain thuần, không phụ thuộc framework |

### 2.3 Sơ đồ tổng thể hệ thống

```
                    ┌──────────┐
                    │  Client  │
                    │ (FE/App) │
                    └────┬─────┘
                         │ HTTPS
                    ┌────▼─────┐
                    │ API :8080│
                    │ (.NET 10)│
                    └──┬──┬──┬─┘
           ┌───────────┤  │  ├───────────┐
           │           │  │  │           │
    ┌──────▼──┐  ┌─────▼──┐ ┌▼────────┐ ┌▼──────────────┐
    │PostgreSQL│  │ Redis  │ │RabbitMQ │ │ Elasticsearch  │
    │   (DB)   │  │(Cache) │ │  (MQ)   │ │   (Logs)       │
    └──────────┘  └────────┘ └─────────┘ └────────────────┘
```

---

## 3. CẤU TRÚC SOLUTION

### 3.1 Tổng quan các Project

```
BE-EF/
├── Api.sln                           # Solution file
├── global.json                       # .NET SDK version: 10.0.200
├── Dockerfile                        # Docker multi-stage build
├── docker-compose.yml                # Docker compose cho deployment
├── azure-pipelines.yml               # CI/CD Azure DevOps
│
├── Api/                              # Tầng Presentation (ASP.NET Core Web API)
│   ├── Api.csproj                    # Web SDK, references Application + Infrastructure
│   ├── Program.cs                    # Host builder, Serilog config
│   ├── Startup.cs                    # DI, Middleware, Auth, Swagger
│   ├── Controllers/                  # HTTP endpoints (7 controllers)
│   ├── Base/                         # BaseController, Middleware, JWT, Constants
│   ├── Extensions/                   # ServiceExtensions (DI Application services)
│   ├── Properties/                   # launchSettings, PublishProfiles
│   ├── Resources/                    # Localization resources (vi, en)
│   └── wwwroot/teamplate/            # Excel templates
│
├── Application/                      # Tầng Use Cases (Business Logic)
│   ├── Application.csproj            # References Domain
│   ├── IServices/                    # Service interfaces (5 interfaces)
│   ├── Services/                     # Service implementations
│   ├── Abstractions/                 # Persistence + Messaging interfaces
│   ├── Request/                      # Input DTOs
│   ├── GetMap/                       # Output DTOs / Dapper row models
│   ├── Common/                       # Shared utilities, models, configs
│   ├── Exceptions/                   # AppException, CoreException
│   └── Extensions/                   # EPPlus, Queryable extensions
│
├── Domain/                           # Tầng Core (Pure .NET, không dependency)
│   ├── Domain.csproj                 # Không có NuGet packages
│   ├── Entities/public/              # DB-first entities (scaffold từ DB)
│   ├── Aggregates/                   # Business rules wrapper
│   ├── ValueObjects/                 # Immutable value types
│   ├── Helpers/                      # Utility functions
│   └── Exceptions/                   # DomainException
│
├── Infrastructure/                   # Tầng Technical Adapters
│   ├── Infrastructure.csproj         # References Application + Domain
│   ├── DataContext/                  # EF DbContext, Mapping, Queries
│   ├── Repositories/                 # UnitOfWork, GenericRepository
│   ├── Persistence/                  # DapperConnectionFactory
│   ├── Messaging/                    # RabbitMQClient
│   ├── Services/                     # DataConfigService (Redis cache)
│   ├── Extensions/                   # EF query extensions
│   └── DependencyInjection/         # AddInfrastructure()
│
└── docs/                             # Tài liệu
    └── ARCHITECTURE.md               # Hướng dẫn kiến trúc cho team
```

### 3.2 Dependency Graph giữa các Project

```
Api.csproj
  ├── Application.csproj
  │     └── Domain.csproj
  └── Infrastructure.csproj
        ├── Application.csproj
        │     └── Domain.csproj
        └── Domain.csproj
```

---

## 4. TẦNG DOMAIN

**Vai trò:** Chứa core business logic, entities, aggregates. Tầng này **thuần .NET**, không phụ thuộc bất kỳ framework hay thư viện bên ngoài nào.

### 4.1 Entities (DB-First)

Entities được scaffold tự động từ PostgreSQL. **Không thêm business method trực tiếp vào entity.**

| Entity | Table | Primary Key | Mô tả |
|--------|-------|-------------|--------|
| `Departments` | `public.Departments` | `DepartmentId` (UUID) | Phòng ban (hỗ trợ cây phân cấp qua `ParentId`) |
| `Users` | `public.Users` | `UserId` (UUID) | Người dùng |
| `Positions` | `public.Positions` | `PositionId` (UUID) | Chức vụ |
| `UserDepartments` | `public.UserDepartments` | Composite (UserId + DepartmentId) | Bảng trung gian: User ↔ Department ↔ Position |

**Các trường chung (Audit Fields):**

| Trường | Kiểu | Mô tả |
|--------|------|--------|
| `IsActive` | bool | Trạng thái hoạt động (mặc định: true) |
| `IsDelete` | bool | Soft delete flag (mặc định: false) |
| `CreateBy` | Guid? | Người tạo |
| `CreateDate` | long? | Ngày tạo (Unix timestamp) |
| `UpdateBy` | Guid? | Người cập nhật |
| `UpdateDate` | long? | Ngày cập nhật (Unix timestamp) |

### 4.2 Aggregates

Aggregates bọc (wrap) entities và chứa business rules. Mỗi aggregate có:
- `Create()` — factory method tạo mới với validation
- `FromEntity()` — khởi tạo từ entity có sẵn
- `Update()` — cập nhật với validation
- `SoftDelete()` — đánh dấu xoá mềm

| Aggregate | Entity | Business Rules |
|-----------|--------|----------------|
| `DepartmentAggregate` | `Departments` | Validate tên không null/empty, đặt audit fields |
| `UserAggregate` | `Users` + `UserDepartments` | Quản lý user + gán phòng ban/chức vụ |
| `PositionAggregate` | `Positions` | Validate tên không null/empty |

**Ví dụ DepartmentAggregate:**

```csharp
public class DepartmentAggregate
{
    public Departments Root { get; private set; }

    public static DepartmentAggregate Create(EntityCode code, string name, 
        Guid? parentId, string note, Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tên phòng ban không được để trống");
        
        return new DepartmentAggregate
        {
            Root = new Departments
            {
                DepartmentId = Guid.NewGuid(),
                DepartmentCode = code.Value,
                DepartmentName = name,
                ParentId = parentId,
                Note = note,
                IsActive = true,
                IsDelete = false,
                CreateBy = createdBy,
                CreateDate = ConvertDateTime.GetCurrentUnixTimeStamp()
            }
        };
    }
}
```

### 4.3 Value Objects

| Value Object | Mô tả |
|--------------|--------|
| `EntityCode` | Wrapper immutable cho mã code (department, user, position). Validate non-empty, hỗ trợ so sánh case-insensitive |

### 4.4 Helpers

| Helper | Chức năng |
|--------|-----------|
| `CommonFunction` | `ToUnSign()` — bỏ dấu tiếng Việt; `ConvertTitle()` — chuẩn hoá tiêu đề |
| `ConvertDateTime` | Chuyển đổi DateTime ↔ Unix Timestamp; tính tuổi; lấy đầu/cuối tháng |

### 4.5 Exceptions

| Exception | Mô tả |
|-----------|--------|
| `DomainException` | Vi phạm rule nghiệp vụ domain (ném từ Aggregates) |

---

## 5. TẦNG APPLICATION

**Vai trò:** Chứa use case / business logic. Đây là tầng **dev làm việc chính (~80% feature mới)**.

### 5.1 Abstractions (Interfaces)

#### Persistence Interfaces

| Interface | Mô tả | Methods chính |
|-----------|--------|---------------|
| `IUnitOfWork` | Unit of Work pattern cho EF | `Commit()`, `Repository<T>()` |
| `IGenericRepository<T>` | Generic repository cho mọi entity | `Get()`, `FirstOrDefault()`, `Insert()`, `Update()`, `Delete()`, `Count()`, `RawSqlQuery()`, `ExecuteSqlRawAsync()` |
| `IDbConnectionFactory` | Factory tạo DB connection cho Dapper | `CreateConnection()` |

**DbConnectionHelper** (static utility):
- `OpenConnectionAsync(DbConnection)` — mở connection async-safe
- `CloseConnection(DbConnection)` — đóng và dispose connection an toàn

#### Messaging Interfaces

| Interface | Mô tả |
|-----------|--------|
| `IRabbitMQClient` | Gửi message qua RabbitMQ |

### 5.2 Service Interfaces & Implementations

| Interface | Service | Persistence | Methods |
|-----------|---------|-------------|---------|
| `IDepartmentService` | `DepartmentService` | EF + UnitOfWork | `GetPaging`, `CreateDepartment`, `UpdateDepartment`, `DeleteDepartment`, `GetListDepartmentByParentId`, `BuildDepartmentTree` |
| `IDepartmentDapperService` | `DepartmentDapperService` | Dapper + Raw SQL | `CreateDepartmentAsync`, `UpdateDepartmentAsync`, `DeleteDepartmentAsync`, `GetListDepartmentByParentIdAsync` |
| `IUserService` | `UserService` | EF + UnitOfWork | `CreateUser`, `UpdateUser`, `DeleteUser`, `GetUserById` |
| `IPositionService` | `PositionService` | EF + UnitOfWork | `CreatePosition`, `UpdatePosition`, `DeletePosition`, `GetListAllPosition` |
| `IHomeService` | `HomeService` | — | `DemoDynamic()` |

### 5.3 Luồng xử lý — EF Pattern

```
Controller
  └─► Service.CreateDepartment(request)
        ├─► _unitOfWork.Repository<Departments>()
        ├─► repository.FirstOrDefault(check trùng mã)
        ├─► DepartmentAggregate.Create(...)
        ├─► repository.Insert(aggregate.Root)
        ├─► _unitOfWork.Commit()
        └─► return bool → ResponseApi
```

### 5.4 Luồng xử lý — Dapper Pattern

```
Controller
  └─► Service.CreateDepartmentAsync(request)
        ├─► _connectionFactory.CreateConnection()
        ├─► DbConnectionHelper.OpenConnectionAsync(connection)
        ├─► Dapper.Execute(SQL, parameters)    // parameterized queries
        ├─► DbConnectionHelper.CloseConnection(connection)  // finally block
        └─► return bool → ResponseApi
```

### 5.5 Request DTOs

| Request DTO | Trường chính |
|-------------|--------------|
| `DepartmentRequest` | `DepartmentId`, `DepartmentName`, `DepartmentCode`, `ParentId`, `Note`, `IsActive`, `IsDelete`, audit fields |
| `UserRequest` | `UserId`, `UserName`, `UserCode`, `Note`, `IsActive`, `IsDelete`, `ListDepartmentOfUser[]` |
| `PositionRequest` | `PositionId`, `PositionName`, `PositionCode`, `Note`, `IsActive`, `IsDelete` |
| `DepartmentOfUser` | `DepartmentId`, `PositionId` (gán user vào department) |
| `QueuesRequest` | `QueueId`, `QueueName`, `QueueContent`, `Rabbitmq_Queue_Name`, `Type` |
| `ParkingTicketRequest` | `KeyParking`, `LicensePlate`, `VehicleType`, `TimeIn`, `TimeOut`, `Fee` |

### 5.6 Response DTOs (GetMap)

| Response DTO | Mô tả |
|-------------|--------|
| `ResponseApi` | Wrapper chuẩn API: `data`, `isSuccess`, `message`, `status` |
| `DepartmentTree` | Cây phòng ban phân cấp (có `SubDepartments[]`) |
| `DepartmentRow` | Row model cho Dapper mapping (phẳng, map sang `DepartmentTree`) |
| `UserInfoGetMap` | Thông tin user kèm danh sách phòng ban |
| `DepartmentOfUserGetMap` | Chi tiết phòng ban theo user (mã, tên, chức vụ) |
| `InvoiceCreateResponse` | Response tạo hoá đơn (mock) |

### 5.7 Common Utilities

| Thành phần | File/Folder | Mô tả |
|------------|-------------|--------|
| Pagination Model | `Common/Models/PagingModel<T>` | `Items[]`, `TotalCount`, `PageIndex`, `PageSize` |
| Pagination Query | `Common/Query/PagingQuery` | `SearchTerm`, `PageSize`, `PageIndex`, `UserPermissionId` |
| App Settings DTO | `Common/Appsetting/Appsettings` | Connection strings, RabbitMQ, Redis, JWT, API Gateway |
| Expression Combiner | `Common/Specifications/ExpressionCombiner` | Kết hợp LINQ expression: `And()`, `Or()` |
| DateTime Converter | `Common/Converters/DateTimeTypeConverter` | AutoMapper: DateTime ↔ Unix timestamp |
| Status Enums | `Common/Status/ApplicationStatus` | `BusinessServiceTypeSendRabbitMq` |

### 5.8 Extensions

| Extension | Mô tả |
|-----------|--------|
| `EPPlusExtension` | Style Excel cell: `SetStyleTemplate()`, `SetValueNumber()` |
| `QueryableExtensions` | `WhereIf()` — conditional LINQ where clause |

### 5.9 Exceptions

| Exception | Mô tả | Xử lý bởi |
|-----------|--------|------------|
| `AppException` | Lỗi user-facing (business rule) | `ErrorHandlerMiddleware` → HTTP 200 + `ResponseApi` |
| `CoreException` | Lỗi hệ thống generic | `ErrorHandlerMiddleware` |

---

## 6. TẦNG INFRASTRUCTURE

**Vai trò:** Implement các kỹ thuật cụ thể: EF Context, Repositories, Dapper, RabbitMQ, Redis.

### 6.1 Data Context (EF Core)

#### SopContext (DbContext)

```csharp
public class SopContext : DbContext
{
    public DbSet<Departments> Departments { get; set; }
    public DbSet<Positions> Positions { get; set; }
    public DbSet<UserDepartments> UserDepartments { get; set; }
    public DbSet<Users> Users { get; set; }
}
```

#### Entity Mapping (Fluent API)

| Mapping Class | Table | Schema | Cấu hình đặc biệt |
|---------------|-------|--------|---------------------|
| `DepartmentsMap` | `Departments` | `public` | `DepartmentCode` varchar(50) required, `DepartmentName` varchar(255) required, `IsActive` default true, `IsDelete` default false |
| `UsersMap` | `Users` | `public` | `UserCode` varchar(50), `UserName` varchar(255), `IsActive` default true |
| `PositionsMap` | `Positions` | `public` | `PositionCode` varchar(50), `PositionName` varchar(255) |
| `UserDepartmentsMap` | `UserDepartments` | `public` | Composite FK (UserId + DepartmentId), optional PositionId |

### 6.2 Repositories

#### GenericRepository\<T\>

Repository chung cho mọi entity, cung cấp:

| Nhóm | Methods |
|------|---------|
| **Query** | `Get(filter, orderBy, include, enableTracking, offset, limit)`, `FirstOrDefault(...)`, `Count(filter)` |
| **Command** | `Insert(entity)`, `InsertRange(entities)`, `Update(entity)`, `UpdateRange(entities)`, `Delete(id)`, `DeleteRange(entities)` |
| **Raw SQL** | `RawSqlQuery<T>(query, map, parameters)`, `ExecuteSqlRawAsync(sql, params)`, `GetDataFromSqlRaw(sql, params)` |

**Đặc điểm:**
- **NoTracking** mặc định cho queries (hiệu năng)
- **Retry logic** cho concurrency exceptions
- Hỗ trợ **Include** (eager loading)
- Hỗ trợ **dynamic OrderBy** qua tên cột

#### UnitOfWork

```csharp
public class UnitOfWork : IUnitOfWork
{
    // Cache repository instances per entity type
    Repository<T>() → returns cached or new GenericRepository<T>
    
    // SaveChanges with 3 retry attempts for concurrency
    Commit() → SaveChangesAsync()
    
    Dispose()
}
```

### 6.3 Dapper Persistence

#### DapperConnectionFactory

- Implements `IDbConnectionFactory`
- Tạo `NpgsqlConnection` từ connection string trong `Appsettings`
- Đăng ký **Singleton** trong DI

#### Mẫu sử dụng Dapper trong Service:

```csharp
public async Task<List<T>> GetDataAsync(...)
{
    var connection = _connectionFactory.CreateConnection();
    try
    {
        await DbConnectionHelper.OpenConnectionAsync(connection);
        var result = await connection.QueryAsync<T>(sql, parameters);
        return result.ToList();
    }
    finally
    {
        DbConnectionHelper.CloseConnection(connection);
    }
}
```

### 6.4 Messaging — RabbitMQ

```csharp
public class RabbitMQClient : IRabbitMQClient
{
    // Cấu hình: host=192.168.2.103, port=5672, user=Administrator
    // Queue: durable=true
    // Message: JSON serialized
    // Exception: silent catch (không rethrow)
    
    SendRabbitMQClientQueues(QueueServiceBusiness message)
}
```

### 6.5 Services — DataConfigService

```csharp
public class DataConfigService : IDataConfig
{
    // Redis cache (24h) → PostgreSQL fallback
    GetConnectStringByConnectName(key) → connection string
    
    // Redis cache (15m) → PostgreSQL fallback
    GetUserIDByUserName(userName) → Guid
}
```

### 6.6 Dependency Injection Registration

```csharp
public static class InfrastructureServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRabbitMQClient, RabbitMQClient>();
        services.AddScoped<IDataConfig, DataConfigService>();
        services.AddSingleton<IDbConnectionFactory, DapperConnectionFactory>();
    }
}
```

---

## 7. TẦNG API (PRESENTATION)

### 7.1 Program.cs — Host Configuration

```csharp
- CreateHostBuilder() loads appsettings.json + appsettings.{env}.json
- Serilog with Elasticsearch sink
  → Index format: z113_{appname}-{env}-{date}
- UseSerilog() for all logging
```

### 7.2 Startup.cs — Service Configuration

**ConfigureServices():**

| Cấu hình | Chi tiết |
|-----------|----------|
| CORS | `AllowAnyOrigin`, `AllowAnyMethod`, `AllowAnyHeader` |
| Authentication | JWT Bearer (HS256) |
| DbContext | `SopContext` — PostgreSQL, multi-tenant support |
| DI | `AddInfrastructure()` + `AddApplicationServices()` |
| Swagger | JWT Bearer auth scheme |
| AutoMapper | Custom DateTime ↔ Unix timestamp converter |
| Compression | Gzip + Brotli response compression |

**Configure() Middleware Pipeline:**

```
UseSwagger()
UseSwaggerUI()          → /swagger/v1
UsePathBase()           → /e-invoice-ho
UseCors()
UseForwardedHeaders()   → X-Forwarded-For, X-Forwarded-Proto
RequestCultureMiddleware
UseAuthentication()
UseAuthorization()
ErrorHandlerMiddleware
UseEndpoints()
```

### 7.3 Controllers

| Controller | Route Prefix | Trạng thái | Chức năng |
|------------|-------------|------------|-----------|
| `AuthenController` | `/api/e-invoice-holding/Authen` | ✅ Active | Tạo JWT token (login) |
| `DepartmentController` | `/api/department-service/department` | ✅ Active | CRUD phòng ban (EF + UnitOfWork) |
| `DepartmentDapperController` | `/api/department-service/department-dapper` | ✅ Active | CRUD phòng ban (Dapper raw SQL) |
| `EInvoiceController` | `/api/e-invoice-holding/e-invoice` | ✅ Stub | Tạo hoá đơn (mock response) |
| `UserController` | `/api/department-service/user` | ⏸️ Commented | Quản lý user |
| `PositionController` | `/api/department-service/position` | ⏸️ Commented | Quản lý chức vụ |
| `HomeController` | `/api/department-service/home` | ⏸️ Commented | Demo endpoints |

### 7.4 BaseController

Lớp base cho tất cả controllers, cung cấp:

| Property | Nguồn | Mô tả |
|----------|-------|--------|
| `Sid` | JWT Claim | User ID hiện tại |
| `IsSuperAdmin` | JWT Claim | Quyền Super Admin |
| `IsAdminSystem` | JWT Claim | Quyền Admin hệ thống |

Tích hợp với Redis cache cho việc kiểm tra quyền.

### 7.5 Service Extensions (DI — Application)

```csharp
// Api/Extensions/ServiceExtensions.cs
services.AddScoped<IDepartmentService, DepartmentService>();
services.AddScoped<IDepartmentDapperService, DepartmentDapperService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IPositionService, PositionService>();
services.AddScoped<IHomeService, HomeService>();
services.AddAutoMapper(...);
```

---

## 8. CƠ SỞ DỮ LIỆU

### 8.1 ERD (Entity Relationship Diagram)

```
┌──────────────────┐         ┌──────────────────────┐
│   Departments    │         │       Users           │
├──────────────────┤         ├──────────────────────┤
│ DepartmentId(PK) │◄───┐    │ UserId (PK)          │
│ DepartmentCode   │    │    │ UserCode              │
│ DepartmentName   │    │    │ UserName              │
│ ParentId (FK→self)│    │    │ Note                  │
│ Note             │    │    │ IsActive              │
│ IsActive         │    │    │ IsDelete              │
│ IsDelete         │    │    │ CreateBy, CreateDate  │
│ CreateBy         │    │    │ UpdateBy, UpdateDate  │
│ CreateDate       │    │    └──────────┬───────────┘
│ UpdateBy         │    │               │
│ UpdateDate       │    │               │
└──────────────────┘    │    ┌──────────▼───────────┐
                        │    │  UserDepartments      │
┌──────────────────┐    │    ├──────────────────────┤
│   Positions      │    ├────│ DepartmentId (FK)     │
├──────────────────┤    │    │ UserId (FK)           │
│ PositionId (PK)  │◄───┼────│ PositionId (FK, opt)  │
│ PositionCode     │    │    │ CreateBy, CreateDate  │
│ PositionName     │    │    │ UpdateBy, UpdateDate  │
│ Note             │    │    └──────────────────────┘
│ IsActive         │    │
│ IsDelete         │    │
│ CreateBy         │    │
│ CreateDate       │    │
│ UpdateBy         │    │
│ UpdateDate       │    │
└──────────────────┘    │
                        │
    Departments.ParentId ───► Departments.DepartmentId
         (Self-referencing: cây phân cấp phòng ban)
```

### 8.2 Quan hệ

| Quan hệ | Kiểu | Mô tả |
|---------|------|--------|
| Departments → UserDepartments | 1:N | Một phòng ban có nhiều user |
| Users → UserDepartments | 1:N | Một user thuộc nhiều phòng ban |
| Positions → UserDepartments | 0..1:N | Một chức vụ gán cho nhiều user-department |
| Departments → Departments | Self-ref | Cây phân cấp qua `ParentId` |

### 8.3 Chiến lược xoá

- **Soft Delete** — sử dụng flag `IsDelete = true`
- Không cascade delete qua FK
- Application layer quản lý logic xoá

---

## 9. XÁC THỰC & PHÂN QUYỀN

### 9.1 JWT Authentication

| Cấu hình | Giá trị |
|-----------|---------|
| Algorithm | HS256 (HMAC SHA-256) |
| Issuer | Cấu hình trong `appsettings.json` |
| Audience | Cấu hình trong `appsettings.json` |
| Expiry | Cấu hình qua `ExpC` claim (hours) |

### 9.2 JWT Token Claims

| Claim | Mô tả |
|-------|--------|
| `ma_user` | Mã user |
| `ten_user` | Tên user |
| `ma_donvi` | Mã đơn vị |
| `full_name` | Họ tên đầy đủ |
| `EmailId` | Email |
| `ExpC` | Thời gian hết hạn (giờ) |
| `TypeLogin` | Loại đăng nhập |

### 9.3 Token Response

```json
{
    "token": "eyJhbGci...",
    "userName": "admin",
    "id": "guid",
    "validaty": "2026-06-28T00:00:00",
    "refreshToken": "...",
    "expiredTime": "2026-05-28T12:00:00"
}
```

### 9.4 Keycloak Integration

Dự án tích hợp `Keycloak.AuthServices.Authentication` (v3.0.0) cho SSO enterprise.

---

## 10. MIDDLEWARE & CROSS-CUTTING CONCERNS

### 10.1 ErrorHandlerMiddleware

Xử lý exception tập trung cho toàn ứng dụng:

| Exception Type | HTTP Status | Response |
|---------------|-------------|----------|
| `AppException` | 200 | `ResponseApi { isSuccess: false, message: "..." }` |
| `DomainException` | 200 | `ResponseApi { isSuccess: false, message: "..." }` |
| `KeyNotFoundException` | 200 | `ResponseApi { isSuccess: false, message: "..." }` |
| Unhandled Exception | 200 | `ResponseApi { isSuccess: false, message: "..." }` + Serilog Error |

### 10.2 RequestCultureMiddleware

| Header | Giá trị | Mặc định |
|--------|---------|----------|
| `language` | `vi`, `en` | `vi` (Vietnamese) |

Middleware đọc header `language` và set `CultureInfo` cho request.

### 10.3 Response Compression

- **Gzip** compression
- **Brotli** compression
- Áp dụng cho tất cả HTTP responses

---

## 11. HỆ THỐNG MESSAGING (RABBITMQ)

### 11.1 Cấu hình

| Thông số | Giá trị |
|----------|---------|
| Host | 192.168.2.103 |
| Port | 5672 |
| Username | Administrator |
| Queue | Durable = true |
| Message Format | JSON (Newtonsoft.Json) |

### 11.2 Flow

```
Service
  └─► IRabbitMQClient.SendRabbitMQClientQueues(message)
        ├─► ConnectionFactory.CreateConnection()
        ├─► channel.QueueDeclare(durable: true)
        ├─► channel.BasicPublish(body: JSON)
        └─► Exception: silent catch (log, không rethrow)
```

### 11.3 Message Model

```csharp
public class QueueServiceBusiness
{
    public string QueueId { get; set; }
    public string QueueName { get; set; }
    public string QueueContent { get; set; }
    public string Rabbitmq_Queue_Name { get; set; }
    public int Type { get; set; }  // BusinessServiceTypeSendRabbitMq enum
}
```

---

## 12. CACHING (REDIS)

### 12.1 Cấu hình

| Thông số | Giá trị |
|----------|---------|
| Host | 172.31.2.121:6379 |
| Database | Mặc định |

### 12.2 Chiến lược Cache

| Dữ liệu | TTL | Pattern |
|----------|-----|---------|
| Connection String | 24 giờ | Cache-aside: Redis → PostgreSQL fallback |
| User ID by UserName | 15 phút | Cache-aside: Redis → PostgreSQL fallback |

### 12.3 Xử lý lỗi

Nếu Redis không khả dụng, service tự động fallback về truy vấn trực tiếp PostgreSQL.

---

## 13. LOGGING & MONITORING

### 13.1 Serilog Configuration

```csharp
// Sinks (đích ghi log):
- Console
- Debug
- Elasticsearch

// Elasticsearch Index:
z113_{appname}-{environment}-{date:yyyy.MM}

// Enrichers:
- Environment
- Exception details
```

### 13.2 Log Levels

| Level | Sử dụng |
|-------|---------|
| Information | Request/response flow |
| Warning | Business rule violations |
| Error | Unhandled exceptions (ErrorHandlerMiddleware) |
| Debug | Development tracing |

---

## 14. API ENDPOINTS

### 14.1 Authentication

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/e-invoice-holding/Authen/get-token` | Đăng nhập, lấy JWT token |

### 14.2 Department (EF)

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/department-service/department/create` | Tạo phòng ban mới |
| POST | `/api/department-service/department/update` | Cập nhật phòng ban |
| GET | `/api/department-service/department/delete?departmentId={id}` | Xoá mềm phòng ban |
| GET | `/api/department-service/department/get-list-department-by-parent-id` | Lấy danh sách phòng ban theo parent (cây) |

### 14.3 Department (Dapper)

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/department-service/department-dapper/create` | Tạo phòng ban (raw SQL) |
| POST | `/api/department-service/department-dapper/update` | Cập nhật phòng ban (raw SQL) |
| GET | `/api/department-service/department-dapper/delete?departmentId={id}` | Xoá phòng ban (raw SQL) |
| GET | `/api/department-service/department-dapper/get-list-department-by-parent-id` | Lấy danh sách phòng ban (raw SQL) |

### 14.4 E-Invoice

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/e-invoice-holding/e-invoice/create` | Tạo hoá đơn (mock/stub) |

### 14.5 Swagger UI

| URL | Mô tả |
|-----|--------|
| `/swagger/v1` | Swagger UI Documentation |

### 14.6 Response Format chuẩn

```json
{
    "data": { ... },
    "isSuccess": true,
    "message": "Thành công",
    "status": 200
}
```

---

## 15. CẤU HÌNH HỆ THỐNG

### 15.1 appsettings.json — Cấu trúc

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
    },
    "JwtSettings": {
        "SecretKey": "...",
        "Issuer": "...",
        "Audience": "...",
        "ExpirationHours": 24
    },
    "RabbitMQ": {
        "Host": "192.168.2.103",
        "Port": 5672,
        "UserName": "Administrator",
        "Password": "..."
    },
    "Redis": {
        "ConnectionString": "172.31.2.121:6379"
    },
    "Elasticsearch": {
        "Uri": "http://172.31.2.121:25"
    },
    "Serilog": { ... }
}
```

### 15.2 Launch Profiles

| Profile | Port | Protocol |
|---------|------|----------|
| IIS Express | 44358 | HTTPS |
| Evo.Mes.Template.Api | 5001 (HTTPS) / 5000 (HTTP) | HTTPS + HTTP |

### 15.3 Multi-Tenant Support

Trong môi trường **Production_Custom**, hệ thống hỗ trợ multi-tenant:
- Parse JWT token → extract `TenantName`
- Chọn connection string theo tenant
- Mỗi tenant có database riêng

---

## 16. DEVOPS & TRIỂN KHAI

### 16.1 Docker

**Dockerfile (Multi-stage build):**

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish Api/Api.csproj -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]
```

**docker-compose.yml:**

```yaml
services:
  api:
    build: .
    ports:
      - "6789:80"
    restart: always
```

### 16.2 Azure Pipelines (CI/CD)

| Bước | Lệnh |
|------|-------|
| 1. Install SDK | .NET 10 SDK |
| 2. Restore | `dotnet restore Api.sln` |
| 3. Publish | `dotnet publish Api/Api.csproj -c Release -o $(publishOutputPath)` |
| 4. Deploy | Copy to `D:\Public\247\api` |
| 5. Artifacts | Publish to Azure DevOps drop |

**Trigger:** Push to `main` branch  
**Agent Pool:** "Agent server"

---

## 17. QUY TRÌNH PHÁT TRIỂN

### 17.1 Thêm Feature CRUD mới (EF)

| Bước | Tầng | Công việc |
|------|------|-----------|
| 1 | DB | Tạo/alter bảng PostgreSQL → scaffold Entity |
| 2 | Domain | Thêm `XxxAggregate` (nếu có business rule) |
| 3 | Application | Tạo `Request/XxxRequest.cs` |
| 4 | Application | Tạo `GetMap/XxxDto.cs` (nếu response khác entity) |
| 5 | Application | Tạo `IServices/IXxxService.cs` + `Services/XxxService.cs` |
| 6 | Api | Tạo `Controllers/XxxController.cs` inject `IXxxService` |
| 7 | Api | Đăng ký DI trong `ServiceExtensions.cs` |

### 17.2 Thêm Feature Query phức tạp (Dapper)

| Bước | Tầng | Công việc |
|------|------|-----------|
| 1 | Application | Tạo `GetMap/XxxRow.cs` — model map cột SQL |
| 2 | Application | Tạo `IServices/IXxxDapperService.cs` + `Services/XxxDapperService.cs` |
| 3 | Api | Tạo Controller inject `IXxxDapperService` |
| 4 | Api | Đăng ký DI |

### 17.3 Khi nào dùng EF vs Dapper?

| Tiêu chí | EF Core | Dapper |
|----------|---------|--------|
| CRUD đơn giản | ✅ Khuyến nghị | Không cần |
| Tracking entity | ✅ Có | ❌ Không |
| Transaction | `UnitOfWork.Commit()` | Tự quản lý |
| Domain Aggregate | ✅ Sử dụng | ❌ Map thẳng SQL |
| Query phức tạp (CTE, recursive) | Hạn chế | ✅ Khuyến nghị |
| Báo cáo / Report | Chậm hơn | ✅ Nhanh hơn |
| Debug SQL | Khó | ✅ Rõ ràng |

### 17.4 Quy tắc quan trọng

| ✅ NÊN LÀM | ❌ KHÔNG LÀM |
|-------------|--------------|
| Application reference Domain | Application reference Infrastructure |
| SQL trong Service (Dapper) | SQL trong Controller |
| Business rule trong Aggregate | Business rule trong Entity (scaffold) |
| DTO trong `Application/GetMap` | DTO lẻ trong Service |
| Connection qua `IDbConnectionFactory` | `new NpgsqlConnection()` trực tiếp |
| Exception qua `AppException` | Throw raw Exception |

---

## 18. PHỤ LỤC

### 18.1 So sánh 2 Controller Department

| Đặc điểm | DepartmentController (EF) | DepartmentDapperController (Dapper) |
|-----------|---------------------------|--------------------------------------|
| Route | `/api/department-service/department` | `/api/department-service/department-dapper` |
| Service | `IDepartmentService` | `IDepartmentDapperService` |
| Persistence | EF + UnitOfWork | Dapper + Raw SQL |
| Aggregate | ✅ Có | ❌ Không |
| Connection | EF Connection Pool | `DbConnectionHelper` + Factory |
| Transaction | `Commit()` | Tự quản lý |

### 18.2 Cấu trúc thư mục Constants & Enums

| File | Nội dung |
|------|----------|
| `ConstApi.cs` | URL endpoints gọi API bên ngoài (location, machine, material, delivery) |
| `Enum.cs` | `TypeMesCreateAccount` — status codes tạo tài khoản |

### 18.3 Internationalization (i18n)

| File | Ngôn ngữ |
|------|----------|
| `RabbitMQController.vi.resx` | Tiếng Việt |
| `RabbitMQController.en.resx` | Tiếng Anh |

Middleware `RequestCultureMiddleware` set culture dựa trên header `language`.

### 18.4 Excel Templates

Các template Excel được lưu tại `Api/wwwroot/teamplate/`:
- `template_lenhsx.xlsx`
- `import_product_v2.xlsx`
- `template_dinhmuc_ngang.xlsx`
- `template_import_dinh_muc.xlsx`
- `template_import_san_pham.xlsx`

---

*Tài liệu này được tạo tự động từ source code dự án BE-EF.*  
*Ngày tạo: 28/05/2026*  
*Framework: .NET 10 — Clean Architecture*
