using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;

public interface IMaHangService
{
    Task<PagedResult<MaHangDto>> LayDanhSachAsync(BoLocMaHangRequest request, CancellationToken cancellationToken);
    Task<MaHangDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<MaHangDto> TaoMoiAsync(TaoMaHangRequest request, CancellationToken cancellationToken);
    Task<MaHangDto> CapNhatAsync(long id, CapNhatMaHangRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
