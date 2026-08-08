namespace Eman.Application.Modules.MasterData.Production.PhanXuong.Dtos;

public sealed record PhanXuongDto(
    Guid Id,
    string MaPhanXuong,
    string TenPhanXuong,
    string? MoTa,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
