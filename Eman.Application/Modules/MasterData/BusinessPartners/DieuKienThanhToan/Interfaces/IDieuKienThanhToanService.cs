
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Dtos;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Interfaces;

public interface IDieuKienThanhToanService
{
    Task<PagedResult<DieuKienThanhToanDto>> LayDanhSachAsync(
        BoLocDieuKienThanhToanRequest request,
        CancellationToken cancellationToken);

    Task<DieuKienThanhToanDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DieuKienThanhToanDto> TaoMoiAsync(
        TaoDieuKienThanhToanRequest request,
        CancellationToken cancellationToken);

    Task<DieuKienThanhToanDto> CapNhatAsync(
        Guid id,
        CapNhatDieuKienThanhToanRequest request,
        CancellationToken cancellationToken);

    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
