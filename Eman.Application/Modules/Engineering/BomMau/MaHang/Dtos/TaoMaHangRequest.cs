using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Dtos;

public sealed class TaoMaHangRequest
{
    [Required(ErrorMessage = "Mã hàng là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Mã hàng không được vượt quá 100 ký tự.")]
    public string MaHang { get; init; } = string.Empty;

    [Required(ErrorMessage = "Diện tích là bắt buộc.")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Diện tích không được nhỏ hơn 0.")]
    public decimal? DienTich { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Hình dáng B.O.M thô không hợp lệ.")]
    public long? HinhDangBomThoId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Hình dáng B.O.M màu không hợp lệ.")]
    public long? HinhDangBomMauId { get; init; }

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    [MaxLength(20, ErrorMessage = "Loại mã hàng không được vượt quá 20 ký tự.")]
    public string? LoaiMaHang { get; init; }
}
