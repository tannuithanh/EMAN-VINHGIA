using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Dtos;

public sealed class CapNhatDeTaiRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Hệ sản phẩm không hợp lệ.")]
    public long HeSanPhamId { get; init; }

    [Required(ErrorMessage = "Mã đề tài là bắt buộc.")]
    [MaxLength(30, ErrorMessage = "Mã đề tài không được vượt quá 30 ký tự.")]
    public string MaDeTai { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên đề tài là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên đề tài không được vượt quá 200 ký tự.")]
    public string TenDeTai { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
