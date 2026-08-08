using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Interfaces;

public interface IBuocNhomTheoMauService
{
    Task<PagedResult<BuocNhomTheoMauDto>> LayDanhSachAsync(BoLocBuocNhomTheoMauRequest request, CancellationToken cancellationToken);
    Task<BuocNhomTheoMauDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<BuocNhomTheoMauDto> TaoMoiAsync(TaoBuocNhomTheoMauRequest request, CancellationToken cancellationToken);
    Task<BuocNhomTheoMauDto> CapNhatAsync(long id, CapNhatBuocNhomTheoMauRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
