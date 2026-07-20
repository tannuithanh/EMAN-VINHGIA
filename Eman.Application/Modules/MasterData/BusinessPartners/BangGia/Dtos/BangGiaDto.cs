namespace Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Dtos;

public sealed record BangGiaDto(
    Guid Id,
    string MaBangGia,
    string TenBangGia,
    Guid DoiTacKinhDoanhId,
    string MaDoiTac,
    string TenDoiTac,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
