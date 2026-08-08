using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Dtos;

namespace Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Interfaces;

public interface ICoSoMuaVatTuService
{
    Task<PagedResult<CoSoMuaVatTuDto>> LayDanhSachAsync(BoLocCoSoMuaVatTuRequest request, CancellationToken cancellationToken);
    Task<CoSoMuaVatTuDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CoSoMuaVatTuDto> TaoMoiAsync(TaoCoSoMuaVatTuRequest request, CancellationToken cancellationToken);
    Task<CoSoMuaVatTuDto> CapNhatAsync(Guid id, CapNhatCoSoMuaVatTuRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
