namespace Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;

public sealed record BomVatTuChiTietDto(
    Guid Id,
    Guid BomVatTuPhienBanId,
    Guid VatTuThanhPhanId,
    string MaVatTuThanhPhan,
    string TenVatTuThanhPhan,
    Guid DonViTinhId,
    string MaDonViTinh,
    string TenDonViTinh,
    decimal SoLuong,
    int ThuTu,
    string? GhiChu,
    DateTime CreatedAt,
    string? CreatedByMsnv,
    DateTime? UpdatedAt,
    string? UpdatedByMsnv,
    string RowVersion);
