using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Dtos;

namespace Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;

public interface IDonViTinhService
{
    Task<PagedResult<DonViTinhDto>> LayDanhSachAsync(BoLocDonViTinhRequest request, CancellationToken cancellationToken);
    Task<DonViTinhDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<DonViTinhDto> TaoMoiAsync(TaoDonViTinhRequest request, CancellationToken cancellationToken);
    Task<DonViTinhDto> CapNhatAsync(Guid id, CapNhatDonViTinhRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
