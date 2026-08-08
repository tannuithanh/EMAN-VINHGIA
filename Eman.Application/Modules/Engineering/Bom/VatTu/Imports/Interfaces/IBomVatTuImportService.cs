using Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Interfaces;

public interface IBomVatTuImportService
{
    Task<BomVatTuImportFileDto> TaoTemplateAsync(CancellationToken cancellationToken = default);

    Task<BomVatTuImportPreviewDto> XemTruocAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        CancellationToken cancellationToken = default);

    Task<BomVatTuImportResultDto> ImportAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        string? createdByMsnv,
        CancellationToken cancellationToken = default);
}
