using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Dtos;

public sealed class KiemThuTinhBomMauRequest
{
    [Required(ErrorMessage = "Mã sản phẩm là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Mã sản phẩm không được vượt quá 100 ký tự.")]
    public string MaSanPham { get; init; } = string.Empty;
}
