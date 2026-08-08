using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Products.SanPham.Dtos;

namespace Eman.Application.Modules.MasterData.Products.SanPham.Interfaces;

public interface ISanPhamService
{
    Task<PagedResult<SanPhamDto>> LayDanhSachAsync(
        BoLocSanPhamRequest request,
        CancellationToken cancellationToken);

    Task<SanPhamDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);

    Task<SanPhamDto> TaoMoiAsync(
        TaoSanPhamRequest request,
        CancellationToken cancellationToken);

    Task<SanPhamDto> CapNhatAsync(
        Guid id,
        CapNhatSanPhamRequest request,
        CancellationToken cancellationToken);

    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
