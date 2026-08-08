using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;

public sealed class BoLocBomMaHangChauInsertRequest
{
    public string? Keyword { get; init; }

    public bool? IsActive { get; init; }

    public long? MaHangId { get; init; }

    public Guid? ChauInsertId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 200, ErrorMessage = "Kích thước trang phải từ 1 đến 200.")]
    public int PageSize { get; init; } = 20;
}
