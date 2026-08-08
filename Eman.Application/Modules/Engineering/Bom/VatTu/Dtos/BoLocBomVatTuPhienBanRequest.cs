using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;

public sealed class BoLocBomVatTuPhienBanRequest
{
    public Guid? VatTuId { get; init; }
    public string? Keyword { get; init; }

    [Range(0, 2, ErrorMessage = "Trạng thái chỉ nhận giá trị từ 0 đến 2.")]
    public byte? TrangThai { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 200, ErrorMessage = "Kích thước trang phải từ 1 đến 200.")]
    public int PageSize { get; init; } = 20;
}
