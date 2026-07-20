using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Dtos;

namespace Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Interfaces;

public interface IBangGiaService
{
    Task<PagedResult<BangGiaDto>> LayDanhSachAsync(
        BoLocBangGiaRequest request,
        CancellationToken cancellationToken);

    Task<BangGiaDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<BangGiaDto> TaoMoiAsync(
        TaoBangGiaRequest request,
        CancellationToken cancellationToken);

    Task<BangGiaDto> CapNhatAsync(
        Guid id,
        CapNhatBangGiaRequest request,
        CancellationToken cancellationToken);

    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
