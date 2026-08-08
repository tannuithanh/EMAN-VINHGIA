using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;

public interface IHinhDangService
{
    Task<PagedResult<HinhDangDto>> LayDanhSachAsync(BoLocHinhDangRequest request, CancellationToken cancellationToken);
    Task<HinhDangDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<HinhDangDto> TaoMoiAsync(TaoHinhDangRequest request, CancellationToken cancellationToken);
    Task<HinhDangDto> CapNhatAsync(long id, CapNhatHinhDangRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
