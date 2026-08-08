using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Interfaces;

public interface IBomMaHangPhenService
{
    Task<PagedResult<BomMaHangPhenDto>> LayDanhSachAsync(
        BoLocBomMaHangPhenRequest request,
        CancellationToken cancellationToken);

    Task<PagedResult<MaHangCoPhenDto>> LayDanhSachMaHangCoPhenAsync(
        BoLocBomMaHangPhenRequest request,
        CancellationToken cancellationToken);

    Task<BomMaHangPhenDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BomMaHangPhenDto> TaoMoiAsync(TaoBomMaHangPhenRequest request, CancellationToken cancellationToken);
    Task<BomMaHangPhenDto> CapNhatAsync(Guid id, CapNhatBomMaHangPhenRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
