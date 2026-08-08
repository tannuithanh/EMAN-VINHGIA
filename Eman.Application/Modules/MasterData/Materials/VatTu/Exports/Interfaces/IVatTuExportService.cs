using Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Dtos;

namespace Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Interfaces;

public interface IVatTuExportService
{
    Task<VatTuExportFileDto> XuatExcelAsync(
        BoLocXuatVatTuRequest request,
        CancellationToken cancellationToken);
}
