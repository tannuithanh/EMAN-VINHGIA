namespace Eman.Application.Modules.MasterData.Production.NhomNangLuc.Dtos;

public sealed record NhomNangLucDto(
    Guid Id,
    string MaNhomNangLuc,
    string TenNhomNangLuc,
    int? ThoiGianLamHang,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
