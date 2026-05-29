# CNX_Counting - Hệ thống Quản trị Kế toán Đa khách hàng

## Thông tin siêu dự án

Hệ thống quản trị số liệu kế toán cho công ty dịch vụ kế toán, phục vụ nhiều khách hàng với các phân hệ:
- **Kế toán tổng hợp** (Chart of Accounts, Journal Entries, Invoices)
- **Báo cáo thuế** (VAT, TNDN, TNCN, Nhà thầu)
- **Quản lý tồn kho** (Kho, Sản phẩm, Nhập/Xuất)
- **Sản xuất** (Lệnh sản xuất, BOM, Chi phí)

## Kiến trúc hệ thống

```
┌─────────────────────────────────────────────────────┐
│                   CLIENTS                            │
├─────────────────────┬───────────────────────────────┤
│  admin.cnx.com      │  {tenant}.cnx.com             │
│  (Admin Dashboard)  │  (Client Portal)              │
├─────────────────────┼───────────────────────────────┤
│  CNX.Admin.Api      │  CNX.Portal.Api               │
│  Port: 5001         │  Port: 5002                   │
├─────────────────────┴───────────────────────────────┤
│              Application Layer                       │
│   (Services, DTOs, Interfaces, Validators)          │
├─────────────────────────────────────────────────────┤
│              Infrastructure Layer                    │
│   (EF Core, Repositories, Multi-tenancy)            │
├─────────────────────────────────────────────────────┤
│              Domain Layer                            │
│   (Entities, Enums, Interfaces)                     │
├─────────────────────────────────────────────────────┤
│              SQL Server Database                     │
└─────────────────────────────────────────────────────┘
```

## Multi-tenancy (Đa khách hàng)

Mỗi khách hàng được cấp một **subdomain riêng**:
- `khachhang1.cnxcounting.com` → Tenant A
- `khachhang2.cnxcounting.com` → Tenant B
- `admin.cnxcounting.com` → Admin Panel

Hệ thống sử dụng **Query Filter** của EF Core để tự động lọc dữ liệu theo TenantId.

## Tech Stack

| Component | Technology |
|-----------|------------|
| Backend | .NET 8 (ASP.NET Core Web API) |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Auth | JWT ****** + ASP.NET Identity |
| API Docs | Swagger/OpenAPI |
| Architecture | Clean Architecture |
| IDE | Visual Studio 2022 |

## Cấu trúc Solution

```
CNX_Counting.sln
├── src/
│   ├── Domain/CNX.Domain/              # Entities, Enums, Interfaces
│   ├── Application/CNX.Application/    # Services, DTOs, Business Logic
│   ├── Infrastructure/CNX.Infrastructure/ # EF Core, Repositories, Multi-tenancy
│   ├── CNX.Shared/                     # Constants, Extensions, Helpers
│   └── Presentation/
│       ├── CNX.Admin.Api/              # Admin Web API (quản trị tổng thể)
│       └── CNX.Portal.Api/            # Client Portal API (subdomain)
└── tests/
```

## Phân hệ chính

### 1. Quản trị Tenant (Admin)
- CRUD khách hàng
- Quản lý subdomain
- Kích hoạt/Vô hiệu hóa
- Dashboard tổng quan

### 2. Kế toán
- Hệ thống tài khoản (Chart of Accounts)
- Bút toán (Journal Entries)
- Hóa đơn đầu vào/đầu ra
- Ghi sổ kế toán

### 3. Báo cáo thuế
- Kê khai thuế GTGT
- Thuế TNDN
- Thuế TNCN
- Workflow: Nháp → Chờ duyệt → Nộp → Duyệt

### 4. Quản lý tồn kho
- Quản lý kho
- Sản phẩm/Vật tư
- Nhập/Xuất/Chuyển kho
- Cảnh báo tồn kho thấp

### 5. Sản xuất
- Lệnh sản xuất
- Định mức NVL (BOM)
- Chi phí sản xuất
- Theo dõi tiến độ

## Hướng dẫn cài đặt

### Yêu cầu
- .NET 8 SDK
- SQL Server 2019+
- Visual Studio 2022

### Các bước

1. Clone repository
```bash
git clone https://github.com/copteck/CNX_Countting.git
cd CNX_Countting
```

2. Cập nhật connection string trong `appsettings.json`

3. Chạy migration
```bash
dotnet ef migrations add InitialCreate --project src/Infrastructure/CNX.Infrastructure --startup-project src/Presentation/CNX.Admin.Api
dotnet ef database update --project src/Infrastructure/CNX.Infrastructure --startup-project src/Presentation/CNX.Admin.Api
```

4. Chạy ứng dụng
```bash
# Admin API
dotnet run --project src/Presentation/CNX.Admin.Api

# Portal API
dotnet run --project src/Presentation/CNX.Portal.Api
```

## API Endpoints

### Admin API (Port 5001)
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/register` - Tạo tài khoản (Admin only)
- `GET /api/tenants` - Danh sách khách hàng
- `POST /api/tenants` - Tạo khách hàng
- `GET /api/dashboard/overview` - Tổng quan

### Portal API (Port 5002) - Theo subdomain
- `GET /api/accounting/accounts` - Hệ thống tài khoản
- `POST /api/accounting/journal-entries` - Tạo bút toán
- `GET /api/accounting/invoices` - Danh sách hóa đơn
- `GET /api/inventory/items` - Tồn kho
- `POST /api/inventory/transactions` - Nhập/Xuất kho
- `GET /api/production/orders` - Lệnh sản xuất
- `GET /api/tax/reports` - Báo cáo thuế

## License

Private - CNX Counting © 2024

