using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Dtos;

public sealed class CapNhatBangGiaRequest
{
    [Required(ErrorMessage = "Mã bảng giá là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã bảng giá không được vượt quá 50 ký tự.")]
    public string MaBangGia { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên bảng giá là bắt buộc.")]
    [MaxLength(250, ErrorMessage = "Tên bảng giá không được vượt quá 250 ký tự.")]
    public string TenBangGia { get; init; } = string.Empty;

    public Guid DoiTacKinhDoanhId { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte TrangThai { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
