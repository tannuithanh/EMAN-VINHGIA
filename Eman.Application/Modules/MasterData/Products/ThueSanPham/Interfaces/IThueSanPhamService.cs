using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Dtos;

namespace Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;

public interface IThueSanPhamService
{
    Task<PagedResult<ThueSanPhamDto>> LayDanhSachAsync(BoLocThueSanPhamRequest request, CancellationToken cancellationToken);
    Task<ThueSanPhamDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ThueSanPhamDto> TaoMoiAsync(TaoThueSanPhamRequest request, CancellationToken cancellationToken);
    Task<ThueSanPhamDto> CapNhatAsync(Guid id, CapNhatThueSanPhamRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
