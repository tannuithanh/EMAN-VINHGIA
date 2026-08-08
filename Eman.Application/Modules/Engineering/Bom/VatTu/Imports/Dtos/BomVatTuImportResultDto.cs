namespace Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;

public sealed class BomVatTuImportResultDto
{
    public bool ThanhCong { get; init; }
    public string Message { get; init; } = string.Empty;
    public int TongSoBom { get; init; }
    public int SoBomDaImport { get; init; }
    public int SoBomBoQua { get; init; }
    public int TongSoDong { get; init; }
    public int SoDongDaImport { get; init; }
    public int SoDongBoQua { get; init; }
    public BomVatTuImportPreviewDto? XemTruoc { get; init; }
}
