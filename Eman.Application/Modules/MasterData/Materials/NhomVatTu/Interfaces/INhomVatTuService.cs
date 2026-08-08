using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Dtos;

namespace Eman.Application.Modules.MasterData.Materials.NhomVatTu.Interfaces;

public interface INhomVatTuService
{
    Task<PagedResult<NhomVatTuDto>> LayDanhSachAsync(BoLocNhomVatTuRequest request, CancellationToken cancellationToken);
    Task<NhomVatTuDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<NhomVatTuDto> TaoMoiAsync(TaoNhomVatTuRequest request, CancellationToken cancellationToken);
    Task<NhomVatTuDto> CapNhatAsync(Guid id, CapNhatNhomVatTuRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
