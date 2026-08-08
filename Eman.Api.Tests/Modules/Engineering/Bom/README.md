# Kiểm thử API B.O.M

Bộ kiểm thử bao phủ 15 chức năng thuộc hai nhóm Swagger:

- `Engineering - B.O.M dùng chung`
- `Engineering - B.O.M màu`

Phạm vi kiểm tra:

- Danh sách và phân trang.
- Tạo mới, lấy chi tiết, cập nhật và xóa.
- DataAnnotations và body thiếu dữ liệu.
- Khóa ngoại không tồn tại.
- Quan hệ Hệ → Đề tài → Màu sắc.
- Mã cốt thô được quản lý trực tiếp trong Màu sắc; không còn API cốt thô riêng.
- Trùng khóa nghiệp vụ.
- Phạm vi `BOM_MAU`/`BOM_THO`.
- Khoảng diện tích, định mức và hệ số không âm.
- Kiểm soát đồng thời bằng `rowVersion`.

Chạy kiểm thử tại thư mục solution:

```powershell
dotnet test .\Eman.Api.Tests\Eman.Api.Tests.csproj
```

Bộ kiểm thử chỉ được phép kết nối tới database `EmanMasterDataDb_Test` theo cấu hình trong `appsettings.Testing.json`.
