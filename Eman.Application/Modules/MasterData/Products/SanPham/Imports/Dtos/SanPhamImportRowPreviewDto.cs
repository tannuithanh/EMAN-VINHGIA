namespace Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;

/// <summary>
/// Kết quả kiểm tra một dòng trong file import sản phẩm.
/// </summary>
public sealed class SanPhamImportRowPreviewDto
{
    public int Dong { get; init; }

    public string? MaSanPham { get; init; }

    public string? MoTaTiengViet { get; init; }

    public string? MaDonViTinh { get; init; }

    public string? MaNhomNangLuc { get; init; }

    public string? MaKhoMacDinh { get; init; }

    public string? MaKhoTon { get; init; }

    public string? MaXuongMacDinh { get; init; }

    public string? MaThue { get; init; }

    public int? BanThanhPham { get; init; }

    public string? NoiGiaoHang { get; init; }

    public bool HopLe => Loi.Count == 0;

    public IReadOnlyList<string> Loi { get; init; } = Array.Empty<string>();
}
