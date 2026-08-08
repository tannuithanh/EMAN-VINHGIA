using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;

public interface INhomMService
{
    Task<PagedResult<NhomMDto>> LayDanhSachAsync(BoLocNhomMRequest request, CancellationToken cancellationToken);
    Task<NhomMDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<NhomMDto> TaoMoiAsync(TaoNhomMRequest request, CancellationToken cancellationToken);
    Task<NhomMDto> CapNhatAsync(long id, CapNhatNhomMRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
