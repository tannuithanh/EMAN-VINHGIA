using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;

public sealed class TaoBomVatTuPhienBanRequest
{
    public Guid VatTuId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Số phiên bản phải lớn hơn 0.")]
    public int SoPhienBan { get; init; }

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }

    [MaxLength(50, ErrorMessage = "Mã nhân viên tạo không được vượt quá 50 ký tự.")]
    public string? CreatedByMsnv { get; init; }
}
