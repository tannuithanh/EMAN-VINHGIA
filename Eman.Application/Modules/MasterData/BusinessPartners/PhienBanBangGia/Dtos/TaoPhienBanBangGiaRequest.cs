using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Dtos;

public sealed class TaoPhienBanBangGiaRequest
{
    public Guid BangGiaId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Số phiên bản phải lớn hơn 0.")]
    public int SoPhienBan { get; init; }

    public DateOnly TuNgay { get; init; }

    public DateOnly? DenNgay { get; init; }
}
