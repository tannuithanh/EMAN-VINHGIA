using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Dtos;

public sealed class CapNhatCoSoMuaVatTuRequest
{
    [Required(ErrorMessage = "Mã cơ sở mua vật tư là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã cơ sở mua vật tư không được vượt quá 50 ký tự.")]
    public string MaCoSoMuaVatTu { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên cơ sở mua vật tư là bắt buộc.")]
    [MaxLength(300, ErrorMessage = "Tên cơ sở mua vật tư không được vượt quá 300 ký tự.")]
    public string TenCoSoMuaVatTu { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? MoTa { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte TrangThai { get; init; }

    [MaxLength(50, ErrorMessage = "Mã nhân viên người cập nhật không được vượt quá 50 ký tự.")]
    public string? UpdatedByMsnv { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
