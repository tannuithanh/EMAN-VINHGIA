using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Dtos;

namespace Eman.Application.Modules.MasterData.Production.NhomNangLuc.Interfaces;

public interface INhomNangLucService
{
    Task<PagedResult<NhomNangLucDto>> LayDanhSachAsync(BoLocNhomNangLucRequest request, CancellationToken cancellationToken);
    Task<NhomNangLucDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<NhomNangLucDto> TaoMoiAsync(TaoNhomNangLucRequest request, CancellationToken cancellationToken);
    Task<NhomNangLucDto> CapNhatAsync(Guid id, CapNhatNhomNangLucRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
