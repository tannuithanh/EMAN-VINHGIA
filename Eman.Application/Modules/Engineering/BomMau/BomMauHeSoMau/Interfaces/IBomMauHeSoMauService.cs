using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Interfaces;

public interface IBomMauHeSoMauService
{
    Task<PagedResult<BomMauHeSoMauDto>> LayDanhSachAsync(BoLocBomMauHeSoMauRequest request, CancellationToken cancellationToken);
    Task<BomMauHeSoMauDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<BomMauHeSoMauDto> TaoMoiAsync(TaoBomMauHeSoMauRequest request, CancellationToken cancellationToken);
    Task<BomMauHeSoMauDto> CapNhatAsync(long id, CapNhatBomMauHeSoMauRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
