namespace Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;

public sealed class BomVatTuImportPreviewDto
{
    public int TongSoDong { get; init; }
    public int SoDongHopLe { get; init; }
    public int SoDongLoi { get; init; }
    public int TongSoBom { get; init; }
    public int SoBomCoTheImport { get; init; }
    public int SoBomLoi { get; init; }
    public bool CoTheImport => SoBomCoTheImport > 0;
    public IReadOnlyList<BomVatTuImportGroupPreviewDto> DanhSachBom { get; init; } =
        Array.Empty<BomVatTuImportGroupPreviewDto>();
    public IReadOnlyList<BomVatTuImportRowPreviewDto> DanhSach { get; init; } =
        Array.Empty<BomVatTuImportRowPreviewDto>();
}
