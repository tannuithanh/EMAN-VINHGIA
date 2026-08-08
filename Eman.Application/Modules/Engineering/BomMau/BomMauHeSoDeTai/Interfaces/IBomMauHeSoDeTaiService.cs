using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Interfaces;

public interface IBomMauHeSoDeTaiService
{
    Task<PagedResult<BomMauHeSoDeTaiDto>> LayDanhSachAsync(BoLocBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken);
    Task<BomMauHeSoDeTaiDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<BomMauHeSoDeTaiDto> TaoMoiAsync(TaoBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken);
    Task<BomMauHeSoDeTaiDto> CapNhatAsync(long id, CapNhatBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
