using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Interfaces;

public interface IBomMauBuocService
{
    Task<PagedResult<BomMauBuocDto>> LayDanhSachAsync(BoLocBomMauBuocRequest request, CancellationToken cancellationToken);
    Task<BomMauBuocDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<BomMauBuocDto> TaoMoiAsync(TaoBomMauBuocRequest request, CancellationToken cancellationToken);
    Task<BomMauBuocDto> CapNhatAsync(long id, CapNhatBomMauBuocRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
