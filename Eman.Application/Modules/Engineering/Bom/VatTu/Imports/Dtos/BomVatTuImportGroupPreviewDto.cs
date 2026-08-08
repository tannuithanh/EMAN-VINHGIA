namespace Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;

public sealed class BomVatTuImportGroupPreviewDto
{
    public string MaVatTuDauRa { get; init; } = string.Empty;
    public string? TenVatTuDauRa { get; init; }
    public string? MaDonViTinhDauRa { get; init; }
    public int SoPhienBanDuKien { get; init; }
    public int TongSoThanhPhan { get; init; }
    public int SoDongLoi { get; init; }
    public bool CoTheImport { get; init; }
    public IReadOnlyList<string> Loi { get; init; } = Array.Empty<string>();
}
