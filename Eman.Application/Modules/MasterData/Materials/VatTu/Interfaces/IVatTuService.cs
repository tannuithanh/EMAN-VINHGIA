using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;

namespace Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;

public interface IVatTuService
{
    Task<PagedResult<VatTuDto>> LayDanhSachAsync(BoLocVatTuRequest request, CancellationToken cancellationToken);
    Task<VatTuDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<VatTuDto> TaoMoiAsync(TaoVatTuRequest request, CancellationToken cancellationToken);
    Task<VatTuDto> CapNhatAsync(Guid id, CapNhatVatTuRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
