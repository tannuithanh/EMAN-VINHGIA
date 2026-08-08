using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Dtos;

namespace Eman.Application.Modules.MasterData.Production.PhanXuong.Interfaces;

public interface IPhanXuongService
{
    Task<PagedResult<PhanXuongDto>> LayDanhSachAsync(BoLocPhanXuongRequest request, CancellationToken cancellationToken);
    Task<PhanXuongDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PhanXuongDto> TaoMoiAsync(TaoPhanXuongRequest request, CancellationToken cancellationToken);
    Task<PhanXuongDto> CapNhatAsync(Guid id, CapNhatPhanXuongRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
