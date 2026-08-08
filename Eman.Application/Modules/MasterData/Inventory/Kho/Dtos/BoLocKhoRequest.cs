using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Inventory.Kho.Dtos;

public sealed class BoLocKhoRequest
{
    public string? Keyword { get; init; }

    public bool? HangTon { get; init; }

    public bool? HangTru { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte? TrangThai { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 200, ErrorMessage = "Kích thước trang phải từ 1 đến 200.")]
    public int PageSize { get; init; } = 20;
}
