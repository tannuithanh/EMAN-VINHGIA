using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Dtos;

public sealed class CapNhatPhienBanBangGiaRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Số phiên bản phải lớn hơn 0.")]
    public int SoPhienBan { get; init; }

    public DateOnly TuNgay { get; init; }

    public DateOnly? DenNgay { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
