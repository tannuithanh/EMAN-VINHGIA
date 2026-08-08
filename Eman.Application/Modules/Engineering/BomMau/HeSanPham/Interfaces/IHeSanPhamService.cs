using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;

public interface IHeSanPhamService
{
    Task<PagedResult<HeSanPhamDto>> LayDanhSachAsync(BoLocHeSanPhamRequest request, CancellationToken cancellationToken);
    Task<HeSanPhamDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<HeSanPhamDto> TaoMoiAsync(TaoHeSanPhamRequest request, CancellationToken cancellationToken);
    Task<HeSanPhamDto> CapNhatAsync(long id, CapNhatHeSanPhamRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, CancellationToken cancellationToken);
}
