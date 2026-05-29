# Hướng dẫn cho Copilot / AI Agent — Dự án CNX_Counting

> **QUAN TRỌNG:** Đây là file mà GitHub Copilot và Visual Studio **tự động đọc** trước mỗi lần sinh/sửa code.
> Đặc tả cốt lõi (single source of truth) nằm tại **`docs/cnx-agent-spec.json`**. Luôn đọc và tuân theo file đó.

## Quy tắc bắt buộc (đọc trước khi làm bất cứ việc gì)

1. **LUÔN đọc `docs/cnx-agent-spec.json`** trước khi sinh hoặc sửa code. Đó là nguồn chuẩn duy nhất.
2. **KHÔNG tự ý thay đổi** tech stack, kiến trúc, bảng màu, hay cấu trúc solution nếu người dùng không yêu cầu rõ ràng.
3. **KHÔNG bịa thêm** module, thư viện, hay tính năng ngoài đặc tả. Nếu thiếu thông tin → **HỎI LẠI**, không tự suy diễn.
4. **KHÔNG xóa** code hoặc test đang có. Chỉ mở rộng/hoàn thiện.
5. Mọi thay đổi **phải build được** và không phá vỡ code/test hiện có.
6. Trả lời và comment code bằng **tiếng Việt**, ngắn gọn, không lan man.

## Tóm tắt cốt lõi (chi tiết xem JSON)

- **Kiến trúc:** Clean Architecture — Domain / Application / Infrastructure / Presentation.
- **Tech:** .NET 8, EF Core 8, SQL Server, JWT + ASP.NET Identity, FluentValidation, Serilog, Polly.
- **Multi-tenant:** database-per-tenant — `MasterDbContext` (CNX_Master) + `ApplicationDbContext` per tenant; phân giải theo subdomain.
- **UI:** Radzen Blazor; màu chủ đạo **cam đậm `#E8590C`** + **xanh đen `#0A1929`** + **xanh công nghệ `#0EA5E9`**.
- **API chuẩn:** mọi endpoint trả về `ApiResponse<T>`; list dùng `PagedResult<T>`; full DTO, không lộ Entity.

## Các ứng dụng cần có

| Project | Vai trò |
|---------|---------|
| `src/Presentation/CNX.Admin.Api` | Admin API (port 5001) |
| `src/Presentation/CNX.Portal.Api` | Client Portal API (port 5002) |
| `src/Tools/CNX.CodeGen` | Công cụ chuyên viết tool (scaffolding CLI) |
| `src/Web/CNX.StaffPortal` | Web portal cho **nhân viên** (Radzen) |
| `src/Web/CNX.TenantWeb` | Web cho **khách hàng/tenant** (Radzen) |

## Khi có mâu thuẫn

Nếu yêu cầu nhất thời mâu thuẫn với `docs/cnx-agent-spec.json` → **ưu tiên file đặc tả** hoặc hỏi lại người dùng trước khi làm. Đừng tự đổi cái cốt lõi.
