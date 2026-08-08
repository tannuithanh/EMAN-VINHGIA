using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;

public interface IDeTaiService
{
    Task<PagedResult<DeTaiDto>> LayDanhSachAsync(BoLocDeTaiRequest request, CancellationToken cancellationToken);
    Task<DeTaiDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<DeTaiDto> TaoMoiAsync(TaoDeTaiRequest request, CancellationToken cancellationToken);
    Task<DeTaiDto> CapNhatAsync(long id, CapNhatDeTaiRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
