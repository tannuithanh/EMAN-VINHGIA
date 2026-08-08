using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Interfaces;

public interface IQuyTacNhomMService
{
    Task<PagedResult<QuyTacNhomMDto>> LayDanhSachAsync(BoLocQuyTacNhomMRequest request, CancellationToken cancellationToken);
    Task<QuyTacNhomMDto> LayTheoIdAsync(long id, CancellationToken cancellationToken);
    Task<QuyTacNhomMDto> TaoMoiAsync(TaoQuyTacNhomMRequest request, CancellationToken cancellationToken);
    Task<QuyTacNhomMDto> CapNhatAsync(long id, CapNhatQuyTacNhomMRequest request, CancellationToken cancellationToken);
    Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken);
}
