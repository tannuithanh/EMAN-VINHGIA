namespace Eman.Application.Modules.MasterData.Common.DonViTinh.Dtos;

public sealed record DonViTinhDto(
    Guid Id,
    string MaDonViTinh,
    string TenDonViTinh,
    string? KyHieu,
    string? MoTa,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
