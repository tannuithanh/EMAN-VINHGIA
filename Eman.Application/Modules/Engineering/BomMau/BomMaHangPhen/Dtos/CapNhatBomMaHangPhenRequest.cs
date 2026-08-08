using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;

public sealed class CapNhatBomMaHangPhenRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Mã hàng không hợp lệ.")]
    public long MaHangId { get; init; }

    [Required(ErrorMessage = "Mã hàng phên là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Mã hàng phên không được vượt quá 100 ký tự.")]
    public string MaHangPhen { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
