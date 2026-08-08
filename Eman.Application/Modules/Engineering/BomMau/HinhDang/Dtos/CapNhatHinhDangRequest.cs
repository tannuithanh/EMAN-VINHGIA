using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Dtos;

public sealed class CapNhatHinhDangRequest
{
    [Required(ErrorMessage = "Mã hình dáng là bắt buộc.")]
    [MaxLength(30, ErrorMessage = "Mã hình dáng không được vượt quá 30 ký tự.")]
    public string MaHinhDang { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên hình dáng là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên hình dáng không được vượt quá 200 ký tự.")]
    public string TenHinhDang { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
