using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;

public sealed class BoLocBuocNhomTheoMauRequest
{
    public string? Keyword { get; init; }

    public bool? IsActive { get; init; }

    public long? HeSanPhamId { get; init; }

    /// <summary>
    /// Bộ lọc tương thích giao diện cũ; đề tài được lấy qua màu sắc.
    /// </summary>
    public long? DeTaiId { get; init; }

    public long? MauSacId { get; init; }

    public string? MaBuoc { get; init; }

    public long? MaHonHopId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Trang phải lớn hơn hoặc bằng 1.")]
    public int Page { get; init; } = 1;

    [Range(1, 200, ErrorMessage = "Kích thước trang phải từ 1 đến 200.")]
    public int PageSize { get; init; } = 20;
}
