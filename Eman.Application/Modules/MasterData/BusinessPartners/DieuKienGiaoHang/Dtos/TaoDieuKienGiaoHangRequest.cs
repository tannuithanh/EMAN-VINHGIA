
using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Dtos;

public sealed class TaoDieuKienGiaoHangRequest
{
    [Required(ErrorMessage = "Mã điều kiện giao hàng là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã điều kiện giao hàng không được vượt quá 50 ký tự.")]
    public string MaDieuKienGiaoHang { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên điều kiện giao hàng là bắt buộc.")]
    [MaxLength(500, ErrorMessage = "Tên điều kiện giao hàng không được vượt quá 500 ký tự.")]
    public string TenDieuKienGiaoHang { get; init; } = string.Empty;
}
