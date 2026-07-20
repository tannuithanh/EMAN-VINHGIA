using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Dtos;

public sealed class BoLocPhienBanBangGiaRequest
{
    public Guid? BangGiaId { get; init; }

    [Range(0, 3, ErrorMessage = "Trạng thái chỉ nhận giá trị từ 0 đến 3.")]
    public byte? TrangThai { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 200, ErrorMessage = "Kích thước trang phải từ 1 đến 200.")]
    public int PageSize { get; init; } = 20;
}
