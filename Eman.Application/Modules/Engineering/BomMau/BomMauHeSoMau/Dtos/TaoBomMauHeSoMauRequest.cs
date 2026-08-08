using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Dtos;

public sealed class TaoBomMauHeSoMauRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Hệ sản phẩm không hợp lệ.")]
    public long HeSanPhamId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Đề tài không hợp lệ.")]
    public long DeTaiId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Màu sắc không hợp lệ.")]
    public long MauSacId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Bước B.O.M màu không hợp lệ.")]
    public long BuocId { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Hệ số không được nhỏ hơn 0.")]
    public decimal HeSo { get; init; }

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }
}
