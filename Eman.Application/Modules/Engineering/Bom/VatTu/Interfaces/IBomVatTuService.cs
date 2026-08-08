using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Interfaces;

public interface IBomVatTuService
{
    Task<PagedResult<BomVatTuPhienBanDto>> LayDanhSachAsync(BoLocBomVatTuPhienBanRequest request, CancellationToken cancellationToken);
    Task<BomVatTuPhienBanDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BomVatTuPhienBanDto> TaoPhienBanAsync(TaoBomVatTuPhienBanRequest request, CancellationToken cancellationToken);
    Task<BomVatTuPhienBanDto> CapNhatPhienBanAsync(Guid id, CapNhatBomVatTuPhienBanRequest request, CancellationToken cancellationToken);
    Task<BomVatTuPhienBanDto> HieuLucAsync(Guid id, string rowVersion, string? updatedByMsnv, CancellationToken cancellationToken);
    Task<BomVatTuPhienBanDto> NgungHieuLucAsync(Guid id, string rowVersion, string? updatedByMsnv, CancellationToken cancellationToken);
    Task XoaPhienBanAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
    Task<BomVatTuChiTietDto> ThemChiTietAsync(Guid phienBanId, TaoBomVatTuChiTietRequest request, CancellationToken cancellationToken);
    Task<BomVatTuChiTietDto> CapNhatChiTietAsync(Guid id, CapNhatBomVatTuChiTietRequest request, CancellationToken cancellationToken);
    Task XoaChiTietAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
