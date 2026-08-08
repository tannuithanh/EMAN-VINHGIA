using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Dtos;

public sealed class CapNhatMauSacRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Hệ sản phẩm không hợp lệ.")]
    public long HeSanPhamId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Đề tài không hợp lệ.")]
    public long DeTaiId { get; init; }

    [Required(ErrorMessage = "Mã màu là bắt buộc.")]
    [MaxLength(30, ErrorMessage = "Mã màu không được vượt quá 30 ký tự.")]
    public string MaMau { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên màu là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên màu không được vượt quá 200 ký tự.")]
    public string TenMau { get; init; } = string.Empty;

    [MaxLength(30, ErrorMessage = "Mã cốt thô không được vượt quá 30 ký tự.")]
    public string? MaCotTho { get; init; }

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
