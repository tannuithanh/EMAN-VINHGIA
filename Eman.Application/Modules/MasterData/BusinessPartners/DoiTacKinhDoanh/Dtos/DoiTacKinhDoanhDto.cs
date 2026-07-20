namespace Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Dtos;

public sealed record DoiTacKinhDoanhDto(
    Guid Id,
    string MaDoiTac,
    string TenDoiTac,
    Guid LoaiDoiTacId,
    string MaLoaiDoiTac,
    string TenLoaiDoiTac,
    bool LaNhaCungCap,
    string? MaSoThue,
    string? DiaChi,
    string? NguoiLienHe,
    string? DienThoai,
    string? Email,
    string? SoTaiKhoan,
    string? TenNganHang,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
