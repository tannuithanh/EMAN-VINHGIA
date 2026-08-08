
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Dtos;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Interfaces;

public interface IDieuKienGiaoHangService
{
    Task<PagedResult<DieuKienGiaoHangDto>> LayDanhSachAsync(
        BoLocDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken);

    Task<DieuKienGiaoHangDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DieuKienGiaoHangDto> TaoMoiAsync(
        TaoDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken);

    Task<DieuKienGiaoHangDto> CapNhatAsync(
        Guid id,
        CapNhatDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken);

    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
