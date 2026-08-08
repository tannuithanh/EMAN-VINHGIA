using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Production.PhanXuong.Dtos;

public sealed class CapNhatPhanXuongRequest
{
    [Required(ErrorMessage = "Mã phân xưởng là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã phân xưởng không được vượt quá 50 ký tự.")]
    public string MaPhanXuong { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên phân xưởng là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên phân xưởng không được vượt quá 200 ký tự.")]
    public string TenPhanXuong { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte TrangThai { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
