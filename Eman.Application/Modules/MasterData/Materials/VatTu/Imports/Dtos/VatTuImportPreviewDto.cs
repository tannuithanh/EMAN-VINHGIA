namespace Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;

public sealed class VatTuImportPreviewDto
{
    public int TongSoDong { get; init; }
    public int SoDongHopLe { get; init; }
    public int SoDongLoi { get; init; }
    public bool CoTheImport => SoDongHopLe > 0;
    public IReadOnlyList<VatTuImportRowPreviewDto> DanhSach { get; init; } =
        Array.Empty<VatTuImportRowPreviewDto>();
}
