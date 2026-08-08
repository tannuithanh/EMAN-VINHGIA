namespace Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;

public sealed record BomVatTuPhienBanDto(
    Guid Id,
    Guid VatTuId,
    string MaVatTu,
    string TenVatTu,
    Guid DonViTinhId,
    string MaDonViTinh,
    string TenDonViTinh,
    int SoPhienBan,
    byte TrangThai,
    string TenTrangThai,
    string? GhiChu,
    int SoThanhPhan,
    IReadOnlyList<BomVatTuChiTietDto> ChiTiets,
    DateTime CreatedAt,
    string? CreatedByMsnv,
    DateTime? UpdatedAt,
    string? UpdatedByMsnv,
    string RowVersion);
