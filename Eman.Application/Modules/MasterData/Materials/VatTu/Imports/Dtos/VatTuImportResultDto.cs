namespace Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;

public sealed class VatTuImportResultDto
{
    public bool ThanhCong { get; init; }
    public string Message { get; init; } = string.Empty;
    public int TongSoDong { get; init; }
    public int SoDongDaImport { get; init; }
    public int SoDongBoQua { get; init; }
    public VatTuImportPreviewDto? XemTruoc { get; init; }
}
