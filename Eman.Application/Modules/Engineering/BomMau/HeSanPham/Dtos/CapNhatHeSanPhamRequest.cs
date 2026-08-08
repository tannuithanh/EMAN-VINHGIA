using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Dtos;

public sealed class CapNhatHeSanPhamRequest
{
    [Required(ErrorMessage = "Mã hệ là bắt buộc.")]
    [MaxLength(20, ErrorMessage = "Mã hệ không được vượt quá 20 ký tự.")]
    public string MaHe { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên hệ sản phẩm là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên hệ sản phẩm không được vượt quá 200 ký tự.")]
    public string TenHe { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    public bool IsActive { get; init; }
}
