# ELK Stack (Elasticsearch + Logstash + Kibana)

## Cấu trúc thư mục

```
Service_Logging/
├── docker-compose.yml
├── logstash/
│   ├── config/
│   │   └── logstash.yml
│   └── pipeline/
│       └── logstash.conf
└── README.md
```

## Khởi động

```bash
cd Service_Logging
docker-compose up -d
```

## Dừng

```bash
docker-compose down
```

## Xóa toàn bộ dữ liệu

```bash
docker-compose down -v
```

## Endpoints

| Service       | URL                        |
|---------------|----------------------------|
| Elasticsearch | http://localhost:9200       |
| Kibana        | http://localhost:5601       |
| Logstash TCP  | localhost:5000 (TCP/UDP)    |
| Logstash Beats| localhost:5044              |

## Tích hợp với .NET (Serilog)

Cài package:

```bash
dotnet add package Serilog.Sinks.Network
```

Cấu hình trong `appsettings.json`:

```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "TCPSink",
        "Args": {
          "uri": "tcp://localhost:5000",
          "textFormatter": "Serilog.Formatting.Json.JsonFormatter, Serilog"
        }
      }
    ]
  }
}
```

## Yêu cầu hệ thống

- Docker Desktop
- RAM tối thiểu: 4GB (khuyến nghị 8GB)
- Elasticsearch mặc định dùng 512MB heap, Logstash dùng 256MB heap
