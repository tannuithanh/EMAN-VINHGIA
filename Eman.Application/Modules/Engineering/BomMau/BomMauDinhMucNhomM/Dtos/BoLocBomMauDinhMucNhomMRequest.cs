using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;

public sealed class BoLocBomMauDinhMucNhomMRequest
{
    public string? Keyword { get; init; }

    public bool? IsActive { get; init; }

    public long? BuocNhomMauId { get; init; }

    public long? NhomMId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 200, ErrorMessage = "Kích thước trang phải từ 1 đến 200.")]
    public int PageSize { get; init; } = 20;
}
