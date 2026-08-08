using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Interfaces;

public interface IChauInsertService
{
    Task<PagedResult<ChauInsertDto>> LayDanhSachAsync(BoLocChauInsertRequest request, CancellationToken cancellationToken);
    Task<ChauInsertDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ChauInsertDto> TaoMoiAsync(TaoChauInsertRequest request, CancellationToken cancellationToken);
    Task<ChauInsertDto> CapNhatAsync(Guid id, CapNhatChauInsertRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
