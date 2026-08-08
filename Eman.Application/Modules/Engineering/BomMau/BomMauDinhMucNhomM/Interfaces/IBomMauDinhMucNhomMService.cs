using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Interfaces;

public interface IBomMauDinhMucNhomMService
{
    Task<PagedResult<BomMauDinhMucNhomMDto>> LayDanhSachAsync(BoLocBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken);
    Task<BomMauDinhMucNhomMDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<BomMauDinhMucNhomMDto> TaoMoiAsync(TaoBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken);
    Task<BomMauDinhMucNhomMDto> CapNhatAsync(long id, CapNhatBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
