using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Dtos;

public sealed class CapNhatNhomMRequest
{
    [Required(ErrorMessage = "Phạm vi B.O.M là bắt buộc.")]
    [MaxLength(20, ErrorMessage = "Phạm vi B.O.M không được vượt quá 20 ký tự.")]
    public string PhamViBom { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mã nhóm M là bắt buộc.")]
    [MaxLength(20, ErrorMessage = "Mã nhóm M không được vượt quá 20 ký tự.")]
    public string MaNhomM { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên nhóm M là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên nhóm M không được vượt quá 200 ký tự.")]
    public string TenNhomM { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int ThuTu { get; init; }

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
