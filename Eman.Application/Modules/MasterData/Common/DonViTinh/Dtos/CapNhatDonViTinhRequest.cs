using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Common.DonViTinh.Dtos;

public sealed class CapNhatDonViTinhRequest
{
    [Required(ErrorMessage = "Mã đơn vị tính là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã đơn vị tính không được vượt quá 50 ký tự.")]
    public string MaDonViTinh { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên đơn vị tính là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên đơn vị tính không được vượt quá 200 ký tự.")]
    public string TenDonViTinh { get; init; } = string.Empty;

    [MaxLength(50, ErrorMessage = "Ký hiệu không được vượt quá 50 ký tự.")]
    public string? KyHieu { get; init; }

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte TrangThai { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
