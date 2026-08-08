
using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Dtos;

public sealed class CapNhatDieuKienThanhToanRequest
{
    [Required(ErrorMessage = "Mã điều kiện thanh toán là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã điều kiện thanh toán không được vượt quá 50 ký tự.")]
    public string MaDieuKienThanhToan { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên điều kiện thanh toán là bắt buộc.")]
    [MaxLength(500, ErrorMessage = "Tên điều kiện thanh toán không được vượt quá 500 ký tự.")]
    public string TenDieuKienThanhToan { get; init; } = string.Empty;

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte TrangThai { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
