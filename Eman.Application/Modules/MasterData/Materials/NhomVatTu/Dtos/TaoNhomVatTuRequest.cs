using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Materials.NhomVatTu.Dtos;

public sealed class TaoNhomVatTuRequest
{
    [Required(ErrorMessage = "Mã nhóm vật tư là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã nhóm vật tư không được vượt quá 50 ký tự.")]
    public string MaNhomVatTu { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên nhóm vật tư là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên nhóm vật tư không được vượt quá 200 ký tự.")]
    public string TenNhomVatTu { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    [MaxLength(50, ErrorMessage = "Mã nhân viên người tạo không được vượt quá 50 ký tự.")]
    public string? CreatedByMsnv { get; init; }
}
