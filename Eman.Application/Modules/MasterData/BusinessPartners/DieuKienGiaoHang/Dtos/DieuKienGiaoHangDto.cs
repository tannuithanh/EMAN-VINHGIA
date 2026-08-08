
namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Dtos;

public sealed record DieuKienGiaoHangDto(
    Guid Id,
    string MaDieuKienGiaoHang,
    string TenDieuKienGiaoHang,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
