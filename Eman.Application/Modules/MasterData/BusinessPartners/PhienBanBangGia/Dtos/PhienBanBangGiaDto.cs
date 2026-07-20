namespace Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Dtos;

public sealed record PhienBanBangGiaDto(
    Guid Id,
    Guid BangGiaId,
    string MaBangGia,
    string TenBangGia,
    int SoPhienBan,
    DateOnly TuNgay,
    DateOnly? DenNgay,
    byte TrangThai,
    string TenTrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
