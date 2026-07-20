using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Dtos;

namespace Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Interfaces;

public interface ILoaiDoiTacService
{
    Task<PagedResult<LoaiDoiTacDto>> LayDanhSachAsync(
        BoLocLoaiDoiTacRequest request,
        CancellationToken cancellationToken);

    Task<LoaiDoiTacDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<LoaiDoiTacDto> TaoMoiAsync(
        TaoLoaiDoiTacRequest request,
        CancellationToken cancellationToken);

    Task<LoaiDoiTacDto> CapNhatAsync(
        Guid id,
        CapNhatLoaiDoiTacRequest request,
        CancellationToken cancellationToken);

    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
