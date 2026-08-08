namespace Eman.Application.Modules.MasterData.Products.ThueSanPham.Dtos;

public sealed record ThueSanPhamDto(
    Guid Id,
    string MaThue,
    string TenThue,
    decimal ThueSuat,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
