using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;

public sealed class CapNhatBomVatTuPhienBanRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Số phiên bản phải lớn hơn 0.")]
    public int SoPhienBan { get; init; }

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }

    [MaxLength(50, ErrorMessage = "Mã nhân viên cập nhật không được vượt quá 50 ký tự.")]
    public string? UpdatedByMsnv { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
