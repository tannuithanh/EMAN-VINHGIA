using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Dtos;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;

public interface IDoiTacKinhDoanhService
{
    Task<PagedResult<DoiTacKinhDoanhDto>> LayDanhSachAsync(
        BoLocDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken);

    Task<DoiTacKinhDoanhDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DoiTacKinhDoanhDto> TaoMoiAsync(
        TaoDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken);

    Task<DoiTacKinhDoanhDto> CapNhatAsync(
        Guid id,
        CapNhatDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken);

    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
