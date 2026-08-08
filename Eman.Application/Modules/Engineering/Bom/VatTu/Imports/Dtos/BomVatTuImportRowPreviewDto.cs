namespace Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;

public sealed class BomVatTuImportRowPreviewDto
{
    public int Dong { get; init; }
    public string? MaVatTuDauRa { get; init; }
    public string? TenVatTuDauRa { get; init; }
    public string? MaDonViTinhDauRa { get; init; }
    public string? MaVatTuThanhPhan { get; init; }
    public string? TenVatTuThanhPhan { get; init; }
    public decimal? SoLuong { get; init; }
    public string? MaDonViTinhThanhPhan { get; init; }
    public string? GhiChu { get; init; }
    public bool HopLe => Loi.Count == 0;
    public IReadOnlyList<string> Loi { get; init; } = Array.Empty<string>();
}
