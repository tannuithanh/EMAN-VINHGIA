namespace Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;

/// <summary>
/// Tổng hợp kết quả xem trước file import sản phẩm.
/// </summary>
public sealed class SanPhamImportPreviewDto
{
    public int TongSoDong { get; init; }

    public int SoDongHopLe { get; init; }

    public int SoDongLoi { get; init; }

    public bool CoTheImport => SoDongHopLe > 0;

    public IReadOnlyList<SanPhamImportRowPreviewDto> DanhSach { get; init; } =
        Array.Empty<SanPhamImportRowPreviewDto>();
}
