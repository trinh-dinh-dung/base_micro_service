# Hướng dẫn chạy Docker — Base Microservice

> Tài liệu hướng dẫn chi tiết cách build, chạy, debug và quản lý các container Docker trong dự án.

---

## Mục lục

1. [Yêu cầu hệ thống](#1-yêu-cầu-hệ-thống)
2. [Tổng quan kiến trúc Docker](#2-tổng-quan-kiến-trúc-docker)
3. [Chạy nhanh — Full Stack](#3-chạy-nhanh--full-stack)
4. [Chạy từng service riêng lẻ](#4-chạy-từng-service-riêng-lẻ)
5. [Cấu hình environment](#5-cấu-hình-environment)
6. [Quản lý container](#6-quản-lý-container)
7. [Xem log & debug](#7-xem-log--debug)
8. [Build lại image](#8-build-lại-image)
9. [Docker network & giao tiếp giữa các service](#9-docker-network--giao-tiếp-giữa-các-service)
10. [Quản lý volume & dữ liệu](#10-quản-lý-volume--dữ-liệu)
11. [Scale service](#11-scale-service)
12. [Bật Consul Service Discovery](#12-bật-consul-service-discovery)
13. [Kết nối Database](#13-kết-nối-database)
14. [Troubleshooting](#14-troubleshooting)
15. [Các lệnh Docker thường dùng](#15-các-lệnh-docker-thường-dùng)

---

## 1. Yêu cầu hệ thống

| Phần mềm | Phiên bản tối thiểu | Ghi chú |
|-----------|---------------------|---------|
| Docker Desktop | 4.x+ | Windows/macOS — bật WSL 2 trên Windows |
| Docker Engine | 24.x+ | Linux |
| Docker Compose | v2.x+ | Đã tích hợp trong Docker Desktop |
| RAM | 8 GB+ | Khuyến nghị 16 GB khi chạy full stack |
| Disk | 5 GB+ | Cho Docker images & volumes |

### Kiểm tra cài đặt

```bash
docker --version
docker compose version
```

> **Lưu ý:** Tài liệu dùng `docker-compose` (v1 syntax). Nếu dùng Docker Compose v2, thay bằng `docker compose` (không có dấu gạch nối).

---

## 2. Tổng quan kiến trúc Docker

```
┌─────────────────────────────── Docker Host ───────────────────────────────┐
│                                                                           │
│   Network: microservice-network                                           │
│   ┌───────────────────────────────────────────────────────────────────┐   │
│   │                                                                   │   │
│   │   ┌────────────┐   ┌────────────┐   ┌──────────────────────┐     │   │
│   │   │ consul     │   │ auth-server│   │ gateway              │     │   │
│   │   │ :8500      │   │ :5010      │   │ :5050                │     │   │
│   │   │ (Go)       │   │ (.NET 10)  │   │ (.NET 10 + YARP)     │     │   │
│   │   └────────────┘   └────────────┘   └──────────┬───────────┘     │   │
│   │                                                 │                 │   │
│   │                          ┌──────────────────────┤                 │   │
│   │                          │                      │                 │   │
│   │              ┌───────────▼──┐       ┌───────────▼──┐             │   │
│   │              │ service-base │       │service-upload│             │   │
│   │              │ :5000        │       │ :5001        │             │   │
│   │              │ (.NET 10)    │       │ (.NET 10)    │             │   │
│   │              └──────────────┘       └──────────────┘             │   │
│   │                                                                   │   │
│   │              ┌──────────────┐                                     │   │
│   │              │ react-app    │                                     │   │
│   │              │ :3000        │                                     │   │
│   │              │ (Nginx)      │                                     │   │
│   │              └──────────────┘                                     │   │
│   │                                                                   │   │
│   └───────────────────────────────────────────────────────────────────┘   │
│                                                                           │
│   Volume: microservice_uploads (service-upload → /app/wwwroot/uploads)    │
│                                                                           │
└───────────────────────────────────────────────────────────────────────────┘
```

### Danh sách container & port

| Container | Image | Port Host | Port Container | Healthcheck |
|-----------|-------|-----------|----------------|-------------|
| `consul` | `hashicorp/consul:1.18` | 8500 | 8500 | `consul members` |
| `auth-server` | Build từ `AuthServer/Dockerfile` | 5010 | 8080 | `/.well-known/openid-configuration` |
| `gateway` | Build từ `Gateway/Dockerfile` | 5050 | 8080 | `/health` |
| `service-base` | Build từ `Service_Base/Dockerfile` | 5000 | 8080 | — |
| `service-upload` | Build từ `Service_Upload/Dockerfile` | 5001 | 8080 | — |
| `react-app` | Build từ `React_App/Dockerfile` | 3000 | 80 | — |

---

## 3. Chạy nhanh — Full Stack

### Bước 1: Clone & di chuyển vào thư mục dự án

```bash
cd base_micro_service
```

### Bước 2: (Tùy chọn) Tạo file `.env`

```bash
cp .env.example .env
# Chỉnh sửa connection string database nếu cần
```

### Bước 3: Build và chạy toàn bộ

```bash
# Build image + khởi động tất cả container
docker-compose up -d --build
```

### Bước 4: Kiểm tra trạng thái

```bash
docker-compose ps
```

Kết quả mong đợi — tất cả container ở trạng thái `Up`:

```
NAME             STATUS                   PORTS
auth-server      Up (healthy)             0.0.0.0:5010->8080/tcp
consul           Up (healthy)             0.0.0.0:8500->8500/tcp
gateway          Up (healthy)             0.0.0.0:5050->8080/tcp
react-app        Up                       0.0.0.0:3000->80/tcp
service-base     Up                       0.0.0.0:5000->8080/tcp
service-upload   Up                       0.0.0.0:5001->8080/tcp
```

### Bước 5: Truy cập ứng dụng

| URL | Dịch vụ |
|-----|---------|
| http://localhost:3000 | React App (Frontend) |
| http://localhost:5050 | Gateway (API entry point) |
| http://localhost:5010 | AuthServer (Login page) |
| http://localhost:5000/swagger | Service Base (Swagger UI) |
| http://localhost:5001/swagger | Service Upload (Swagger UI) |
| http://localhost:8500/ui | Consul (Service Discovery UI) |

### Tài khoản demo

| Username | Password | Role |
|----------|----------|------|
| `admin` | `admin123` | Admin |
| `user` | `user123` | User |

---

## 4. Chạy từng service riêng lẻ

### Chỉ chạy AuthServer + Gateway (backend core)

```bash
docker-compose up -d consul auth-server gateway
```

### Chỉ chạy 1 service cụ thể

```bash
# Chạy riêng Service Base (và các dependency)
docker-compose up -d service-base

# Chạy riêng React App (cần gateway đã chạy)
docker-compose up -d react-app
```

### Chạy Service_Api_Base standalone (docker-compose riêng)

Service_Api_Base có `docker-compose.yml` riêng, dùng cho deploy độc lập:

```bash
cd Service_Api_Base
docker-compose up -d --build
# Swagger: http://localhost:6789/swagger
```

### Chạy Consul standalone

```bash
cd Service_Discovery
docker-compose up -d
# Consul UI: http://localhost:8500/ui
```

> **Lưu ý:** Consul standalone dùng network `microservice-network` (external). Cần tạo network trước nếu chưa có:
> ```bash
> docker network create microservice-network
> ```

---

## 5. Cấu hình environment

### Biến môi trường trong `docker-compose.yml`

Các biến quan trọng có thể override qua file `.env` ở thư mục root:

| Biến | Mặc định | Mô tả |
|------|----------|-------|
| `SERVICE_BASE_DB` | `Server=host.docker.internal;Port=5432;...` | Connection string PostgreSQL cho Service Base |
| `SERVICE_UPLOAD_DB` | `Server=host.docker.internal;Port=5432;...` | Connection string PostgreSQL cho Service Upload |
| `ELASTIC_URI` | `http://localhost:9200` | Elasticsearch URI |

### Ví dụ file `.env`

```env
# Database
SERVICE_BASE_DB=Server=192.168.1.100;Port=5432;Database=MyAppDB;User Id=postgres;Password=StrongPass123;
SERVICE_UPLOAD_DB=Server=192.168.1.100;Port=5432;Database=UploadDB;User Id=postgres;Password=StrongPass123;

# Elasticsearch
ELASTIC_URI=http://192.168.1.100:9200
```

### Biến build-time cho React App

React App dùng `ARG` trong Dockerfile (build-time), cấu hình trong `docker-compose.yml`:

```yaml
react-app:
  build:
    args:
      VITE_AUTH_AUTHORITY: http://localhost:5010     # URL AuthServer (browser truy cập)
      VITE_CLIENT_ID: react-app                      # OIDC client ID
      VITE_REDIRECT_URI: http://localhost:3000/callback
      VITE_POST_LOGOUT_URI: http://localhost:3000
      VITE_GATEWAY_URL: http://localhost:5050        # URL Gateway (browser truy cập)
```

> **Quan trọng:** `VITE_*` là biến build-time, trình duyệt truy cập trực tiếp → phải dùng `localhost` (không phải tên container Docker).

---

## 6. Quản lý container

### Khởi động

```bash
# Khởi động tất cả (dùng image đã build)
docker-compose up -d

# Khởi động + build lại
docker-compose up -d --build

# Khởi động 1 service cụ thể
docker-compose up -d service-base
```

### Dừng

```bash
# Dừng tất cả (giữ volume)
docker-compose down

# Dừng + xóa volume (mất dữ liệu upload)
docker-compose down -v

# Dừng 1 service
docker-compose stop service-base
```

### Restart

```bash
# Restart tất cả
docker-compose restart

# Restart 1 service
docker-compose restart gateway

# Restart + build lại 1 service
docker-compose up -d --build gateway
```

---

## 7. Xem log & debug

### Xem log

```bash
# Log tất cả service (follow mode)
docker-compose logs -f

# Log 1 service cụ thể
docker-compose logs -f gateway
docker-compose logs -f auth-server
docker-compose logs -f service-base

# Log 100 dòng cuối
docker-compose logs --tail=100 service-base

# Log từ thời điểm cụ thể
docker-compose logs --since="2024-01-01T10:00:00" gateway
```

### Truy cập vào container (shell)

```bash
# Vào bash của container .NET
docker exec -it service-base /bin/bash

# Vào sh của container Nginx (React App)
docker exec -it react-app /bin/sh

# Vào sh của container Consul
docker exec -it consul /bin/sh
```

### Kiểm tra health

```bash
# Kiểm tra health tất cả container
docker-compose ps

# Kiểm tra chi tiết health 1 container
docker inspect --format='{{json .State.Health}}' auth-server | python -m json.tool

# Test health endpoint thủ công
curl http://localhost:5050/health
curl http://localhost:5010/.well-known/openid-configuration
```

### Kiểm tra network

```bash
# Xem các container trong network
docker network inspect microservice-network

# Test kết nối giữa các container (từ trong container)
docker exec -it gateway curl http://auth-server:8080/.well-known/openid-configuration
docker exec -it service-base curl http://service-upload:8080/api/FileUpload/list
```

---

## 8. Build lại image

### Build lại tất cả

```bash
docker-compose build
```

### Build lại 1 service (không cache)

```bash
docker-compose build --no-cache gateway
```

### Build lại và khởi động

```bash
# Build + restart 1 service (không ảnh hưởng service khác)
docker-compose up -d --build --no-deps service-base
```

| Flag | Ý nghĩa |
|------|---------|
| `--build` | Build lại image trước khi start |
| `--no-cache` | Build từ đầu, bỏ qua Docker layer cache |
| `--no-deps` | Không restart dependency services |

### Xóa image cũ

```bash
# Xóa image không sử dụng
docker image prune -f

# Xóa tất cả image của project
docker-compose down --rmi all
```

---

## 9. Docker network & giao tiếp giữa các service

Tất cả container dùng chung network `microservice-network`. Trong cùng network, các container giao tiếp qua **tên container** (DNS tự động):

```
Container name      →  Hostname nội bộ       →  Port nội bộ
──────────────────────────────────────────────────────────────
auth-server         →  auth-server            →  8080
gateway             →  gateway                →  8080
service-base        →  service-base           →  8080
service-upload      →  service-upload         →  8080
consul              →  consul                 →  8500
react-app           →  react-app              →  80
```

### Quy tắc traffic

```
╔══════════════════════════════════════════════════════════════════════╗
║  Browser/Client bên ngoài  →  Qua Gateway (:5050)                   ║
║  Service ↔ Service         →  Gọi trực tiếp qua tên container      ║
╚══════════════════════════════════════════════════════════════════════╝
```

**Ví dụ:** Gateway gọi Auth Server (trong Docker):
```
http://auth-server:8080   ← ĐÚNG (tên container + port nội bộ)
http://localhost:5010      ← SAI (localhost trong container ≠ host machine)
```

---

## 10. Quản lý volume & dữ liệu

### Volume trong dự án

| Volume | Container | Mount path | Dữ liệu |
|--------|-----------|------------|----------|
| `microservice_uploads` | `service-upload` | `/app/wwwroot/uploads` | File upload của người dùng |

### Xem volume

```bash
# Liệt kê volume
docker volume ls

# Xem chi tiết
docker volume inspect microservice_uploads
```

### Backup volume

```bash
# Backup thư mục uploads
docker run --rm -v microservice_uploads:/data -v $(pwd):/backup alpine \
  tar czf /backup/uploads_backup.tar.gz -C /data .
```

### Xóa volume

```bash
# Xóa volume khi dừng
docker-compose down -v

# Xóa volume cụ thể
docker volume rm microservice_uploads
```

> **Cảnh báo:** Xóa volume sẽ mất toàn bộ file đã upload.

---

## 11. Scale service

Có thể scale số instance của service (khi dùng Consul):

```bash
# Scale Service Base lên 3 instance
docker-compose up -d --scale service-base=3

# Scale Service Upload lên 2 instance
docker-compose up -d --scale service-upload=2
```

> **Lưu ý:** Khi scale, cần bật `ServiceDiscovery__UseConsul=true` để Consul load-balance giữa các instance. Nếu không bật Consul, chỉ có 1 instance được Gateway route đến.

> **Lưu ý:** Khi scale, không thể dùng `container_name` cố định và `ports` mapping cố định. Cần bỏ 2 thuộc tính này trong `docker-compose.yml` cho service cần scale.

---

## 12. Bật Consul Service Discovery

### Bước 1: Sửa `docker-compose.yml`

Đổi biến `ServiceDiscovery__UseConsul` từ `false` sang `true` cho **Gateway**, **service-base**, **service-upload**:

```yaml
# Gateway
- ServiceDiscovery__UseConsul=true

# service-base
- ServiceDiscovery__UseConsul=true

# service-upload
- ServiceDiscovery__UseConsul=true
```

### Bước 2: Restart

```bash
docker-compose up -d --build
```

### Bước 3: Kiểm tra

Truy cập Consul UI: http://localhost:8500/ui — kiểm tra các service đã đăng ký và healthy.

---

## 13. Kết nối Database

### PostgreSQL trên máy host

Các service .NET kết nối PostgreSQL qua `host.docker.internal`:

```
Server=host.docker.internal;Port=5432;Database=ManageDepartment;User Id=postgres;Password=123456aA@;
```

> `host.docker.internal` là DNS đặc biệt của Docker, trỏ về máy host. Hoạt động trên Docker Desktop (Windows/macOS). Trên Linux, cần thêm `--add-host=host.docker.internal:host-gateway`.

### PostgreSQL trong Docker (tùy chọn)

Nếu muốn chạy PostgreSQL trong Docker cùng stack, thêm vào `docker-compose.yml`:

```yaml
services:
  postgres:
    image: postgres:16-alpine
    container_name: postgres
    restart: unless-stopped
    ports:
      - "5432:5432"
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: 123456aA@
      POSTGRES_DB: ManageDepartment
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks: [microservice-network]

volumes:
  postgres_data:
```

Sau đó đổi connection string:
```
Server=postgres;Port=5432;Database=ManageDepartment;User Id=postgres;Password=123456aA@;
```

---

## 14. Troubleshooting

### Container không khởi động được

```bash
# Xem log chi tiết
docker-compose logs service-base

# Xem exit code
docker inspect --format='{{.State.ExitCode}}' service-base
```

### Auth Server chưa healthy → Gateway không start

Gateway phụ thuộc `auth-server` (condition: `service_healthy`). Nếu Auth Server chưa healthy:

```bash
# Kiểm tra health
docker inspect --format='{{json .State.Health}}' auth-server

# Xem log auth-server
docker-compose logs -f auth-server

# Restart auth-server
docker-compose restart auth-server
```

### Lỗi "port already in use"

```bash
# Kiểm tra port đang dùng (Windows)
netstat -ano | findstr :5050

# Kiểm tra port đang dùng (Linux/macOS)
lsof -i :5050

# Giải pháp: dừng process đang dùng port, hoặc đổi port trong docker-compose.yml
```

### Lỗi kết nối giữa các service

```bash
# Kiểm tra network
docker network inspect microservice-network

# Test DNS resolution từ trong container
docker exec -it gateway nslookup auth-server

# Test kết nối HTTP
docker exec -it gateway curl -v http://auth-server:8080/.well-known/openid-configuration
```

### Lỗi kết nối Database

```bash
# Kiểm tra PostgreSQL có chạy không
docker exec -it service-base curl -v telnet://host.docker.internal:5432

# Kiểm tra connection string
docker exec -it service-base env | grep DefaultConnection
```

### React App build lỗi / hiển thị sai URL

React App dùng biến `VITE_*` lúc **build** (không phải runtime). Nếu đổi URL:

```bash
# Phải build lại image
docker-compose build --no-cache react-app
docker-compose up -d react-app
```

### Xóa sạch và chạy lại từ đầu

```bash
# Dừng tất cả + xóa volume
docker-compose down -v

# Xóa image cũ
docker-compose down --rmi all

# Build lại từ đầu
docker-compose up -d --build
```

---

## 15. Các lệnh Docker thường dùng

### Tổng hợp nhanh

```bash
# ── Lifecycle ──────────────────────────────────────────────
docker-compose up -d --build          # Build + chạy tất cả
docker-compose up -d                  # Chạy tất cả (dùng image cũ)
docker-compose down                   # Dừng tất cả
docker-compose down -v                # Dừng + xóa volume
docker-compose restart                # Restart tất cả
docker-compose restart gateway        # Restart 1 service

# ── Build ──────────────────────────────────────────────────
docker-compose build                  # Build tất cả
docker-compose build --no-cache gateway   # Build lại không cache
docker-compose up -d --build --no-deps service-base  # Build + restart 1 service

# ── Trạng thái ─────────────────────────────────────────────
docker-compose ps                     # Xem trạng thái containers
docker stats                          # Xem CPU/RAM realtime

# ── Log ────────────────────────────────────────────────────
docker-compose logs -f                # Log tất cả (follow)
docker-compose logs -f gateway        # Log 1 service
docker-compose logs --tail=50 gateway # 50 dòng cuối

# ── Debug ──────────────────────────────────────────────────
docker exec -it service-base /bin/bash    # Shell vào container
docker inspect service-base               # Chi tiết container

# ── Dọn dẹp ───────────────────────────────────────────────
docker image prune -f                 # Xóa image không dùng
docker volume prune -f                # Xóa volume không dùng
docker system prune -f                # Xóa tất cả không dùng (image + container + network)
```

### Workflow phát triển hàng ngày

```bash
# Sáng — Khởi động
docker-compose up -d

# Sửa code Service Base → Build lại chỉ service đó
docker-compose up -d --build --no-deps service-base

# Sửa code React App → Build lại
docker-compose build --no-cache react-app
docker-compose up -d react-app

# Debug lỗi
docker-compose logs -f service-base

# Cuối ngày — Dừng
docker-compose down
```
