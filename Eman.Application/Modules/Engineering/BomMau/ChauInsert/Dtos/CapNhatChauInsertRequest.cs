using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Dtos;

public sealed class CapNhatChauInsertRequest
{
    [Required(ErrorMessage = "Mã chậu insert là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Mã chậu insert không được vượt quá 100 ký tự.")]
    public string MaChauInsert { get; init; } = string.Empty;

    [MaxLength(300, ErrorMessage = "Tên chậu insert không được vượt quá 300 ký tự.")]
    public string? TenChauInsert { get; init; }

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
