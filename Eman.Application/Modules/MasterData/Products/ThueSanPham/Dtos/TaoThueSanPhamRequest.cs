using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Products.ThueSanPham.Dtos;

public sealed class TaoThueSanPhamRequest
{
    [Required(ErrorMessage = "Mã thuế là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã thuế không được vượt quá 50 ký tự.")]
    public string MaThue { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên thuế là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên thuế không được vượt quá 200 ký tự.")]
    public string TenThue { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "100", ErrorMessage = "Thuế suất phải từ 0 đến 100.")]
    public decimal ThueSuat { get; init; }
}
