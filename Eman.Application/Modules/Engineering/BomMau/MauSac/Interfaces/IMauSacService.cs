using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;

public interface IMauSacService
{
    Task<PagedResult<MauSacDto>> LayDanhSachAsync(BoLocMauSacRequest request, CancellationToken cancellationToken);
    Task<MauSacDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<MauSacDto> TaoMoiAsync(TaoMauSacRequest request, CancellationToken cancellationToken);
    Task<MauSacDto> CapNhatAsync(long id, CapNhatMauSacRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
