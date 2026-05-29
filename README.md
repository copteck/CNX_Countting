# CNX_Counting - Hệ thống Quản trị Kế toán Đa khách hàng

## 🤖 Đặc tả cho AI / Copilot (đọc trước khi sinh code)

Để AI **luôn bám theo chuẩn cốt lõi** (không mỗi lần một kiểu), dự án có 2 file chuẩn:

- **`docs/cnx-agent-spec.json`** — đặc tả cốt lõi (single source of truth): kiến trúc, tech stack, bảng màu, module, chuẩn API.
- **`.github/copilot-instructions.md`** — file mà GitHub Copilot & Visual Studio **tự động đọc** mỗi lần chạy, ép AI tuân theo đặc tả trên.

**Cách dùng:**
1. Trong Visual Studio 2022 / VS Code, mở Copilot Chat ở chế độ **Agent**.
2. Copilot sẽ tự đọc `.github/copilot-instructions.md`. Bạn chỉ cần ra lệnh ngắn gọn, ví dụ:
   > "Đọc `docs/cnx-agent-spec.json` và triển khai module Inventory đúng đặc tả, build và sửa lỗi đến khi pass."
3. Khi AI đi lệch hướng, nhắc lại: *"Bám theo `docs/cnx-agent-spec.json`."* — toàn bộ căn bản cốt lõi đã được giữ trong file đó.

> 💡 Muốn đổi yêu cầu cốt lõi (màu sắc, module, kiến trúc...)? Hãy sửa trong `docs/cnx-agent-spec.json` — đừng chỉ nói trong chat, để lần sau AI vẫn nhớ.

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
│   ├── Presentation/
│   │   ├── CNX.Admin.Api/             # Admin Web API (quản trị tổng thể)
│   │   └── CNX.Portal.Api/            # Client Portal API (subdomain)
│   ├── Tools/
│   │   └── CNX.CodeGen/               # Công cụ chuyên viết tool (scaffolding CLI)
│   └── Web/
│       ├── CNX.StaffPortal/           # Web portal cho nhân viên (Radzen Blazor)
│       └── CNX.TenantWeb/             # Web cho khách hàng/tenant (Radzen Blazor)
└── tests/
```

### Chạy các ứng dụng Web & công cụ

```bash
# Web nhân viên (Radzen Blazor)
dotnet run --project src/Web/CNX.StaffPortal

# Web khách hàng (Radzen Blazor)
dotnet run --project src/Web/CNX.TenantWeb

# Công cụ sinh code (ví dụ: sinh DTO)
dotnet run --project src/Tools/CNX.CodeGen -- dto --name Customer --fields "Name:string,Code:string,IsActive:bool"
```

> Giao diện 2 web dùng **Radzen Blazor** với theme thương hiệu `wwwroot/cnx-theme.css` (cam đậm `#E8590C` + xanh đen `#0A1929` + xanh công nghệ `#0EA5E9`).

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

## 💰 Mẹo tối ưu token mỗi lần gọi AI

Để AI làm đúng mà tốn ít token nhất:

1. **Ra lệnh ngắn, trỏ tới file đặc tả** thay vì mô tả lại từ đầu:
   > "Theo `docs/cnx-agent-spec.json`, làm module Inventory." — đặc tả dài đã nằm trong file, không cần lặp lại trong chat.
2. **Làm từng module/việc nhỏ một**, đừng yêu cầu "code hết tất cả" trong 1 lần — vừa tốn token vừa dễ sai, khó kiểm soát.
3. **Chỉ rõ phạm vi file/thư mục** cần sửa (ví dụ "chỉ sửa trong `src/Web/CNX.StaffPortal`") để AI không đọc cả repo.
4. **Mở session mới cho việc mới.** Chat càng dài, mỗi lần gọi càng phải "nhớ" lại toàn bộ lịch sử ⇒ tốn token. Việc cốt lõi đã lưu ở file nên không sợ mất.
5. **Sửa yêu cầu nền tảng trong file**, không tranh luận trong chat: đổi màu/module/kiến trúc thì sửa `docs/cnx-agent-spec.json` rồi bảo AI "đọc lại spec".
6. **Tận dụng công cụ `CNX.CodeGen`** để sinh code lặp đi lặp lại (DTO, CRUD) bằng máy — gần như tốn 0 token AI.
7. **Dùng `.github/copilot-instructions.md`** (đã có sẵn): Copilot tự đọc nên bạn không phải nhắc lại quy tắc mỗi lần.

