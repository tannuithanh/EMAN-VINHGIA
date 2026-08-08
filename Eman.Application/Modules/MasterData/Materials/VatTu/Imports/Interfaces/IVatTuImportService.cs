using Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;

namespace Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Interfaces;

public interface IVatTuImportService
{
    Task<VatTuImportFileDto> TaoTemplateAsync(CancellationToken cancellationToken = default);
    Task<VatTuImportPreviewDto> XemTruocAsync(
        Stream fileStream, string fileName, long fileSize,
        CancellationToken cancellationToken = default);
    Task<VatTuImportResultDto> ImportAsync(
        Stream fileStream, string fileName, long fileSize, string? createdByMsnv,
        CancellationToken cancellationToken = default);
}
