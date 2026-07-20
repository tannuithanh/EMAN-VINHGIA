namespace Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Dtos;

public sealed record LoaiDoiTacDto(
    Guid Id,
    string MaLoaiDoiTac,
    string TenLoaiDoiTac,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
