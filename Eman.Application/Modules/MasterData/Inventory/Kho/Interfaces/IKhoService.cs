using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Inventory.Kho.Dtos;

namespace Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;

public interface IKhoService
{
    Task<PagedResult<KhoDto>> LayDanhSachAsync(BoLocKhoRequest request, CancellationToken cancellationToken);
    Task<KhoDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<KhoDto> TaoMoiAsync(TaoKhoRequest request, CancellationToken cancellationToken);
    Task<KhoDto> CapNhatAsync(Guid id, CapNhatKhoRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
