using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Dtos;

namespace Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Interfaces;

public interface IPhienBanBangGiaService
{
    Task<PagedResult<PhienBanBangGiaDto>> LayDanhSachAsync(
        BoLocPhienBanBangGiaRequest request,
        CancellationToken cancellationToken);

    Task<PhienBanBangGiaDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PhienBanBangGiaDto> TaoMoiAsync(
        TaoPhienBanBangGiaRequest request,
        CancellationToken cancellationToken);

    Task<PhienBanBangGiaDto> CapNhatAsync(
        Guid id,
        CapNhatPhienBanBangGiaRequest request,
        CancellationToken cancellationToken);

    Task<PhienBanBangGiaDto> HieuLucAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken);

    Task<PhienBanBangGiaDto> HetHieuLucAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken);

    Task<PhienBanBangGiaDto> HuyAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken);

    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
