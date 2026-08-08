using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Products.SanPham.Dtos;

public sealed class BoLocSanPhamRequest
{
    public string? Keyword { get; init; }
    public Guid? DonViTinhId { get; init; }
    public Guid? NhomNangLucId { get; init; }
    public Guid? KhoMacDinhId { get; init; }
    public Guid? KhoTonId { get; init; }
    public Guid? XuongMacDinhId { get; init; }
    public Guid? ThueId { get; init; }
    public bool? LaBanThanhPham { get; init; }
    public string? NoiGiaoHang { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte? TrangThai { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 200, ErrorMessage = "Kích thước trang phải từ 1 đến 200.")]
    public int PageSize { get; init; } = 20;
}
