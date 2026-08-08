using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Inventory.Kho.Dtos;

public sealed class TaoKhoRequest
{
    [Required(ErrorMessage = "Mã kho là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã kho không được vượt quá 50 ký tự.")]
    public string MaKho { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên kho là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên kho không được vượt quá 200 ký tự.")]
    public string TenKho { get; init; } = string.Empty;

    public bool HangTon { get; init; }

    public bool HangTru { get; init; }

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }
}
