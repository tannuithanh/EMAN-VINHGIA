using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Production.NhomNangLuc.Dtos;

public sealed class CapNhatNhomNangLucRequest
{
    [Required(ErrorMessage = "Mã nhóm năng lực là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã nhóm năng lực không được vượt quá 50 ký tự.")]
    public string MaNhomNangLuc { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên nhóm năng lực là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên nhóm năng lực không được vượt quá 200 ký tự.")]
    public string TenNhomNangLuc { get; init; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Thời gian làm hàng không được âm.")]
    public int? ThoiGianLamHang { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte TrangThai { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
