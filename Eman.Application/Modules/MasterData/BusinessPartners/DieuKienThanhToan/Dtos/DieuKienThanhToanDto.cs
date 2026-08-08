
namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Dtos;

public sealed record DieuKienThanhToanDto(
    Guid Id,
    string MaDieuKienThanhToan,
    string TenDieuKienThanhToan,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
