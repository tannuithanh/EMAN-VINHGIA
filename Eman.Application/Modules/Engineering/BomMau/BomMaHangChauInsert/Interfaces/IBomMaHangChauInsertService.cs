using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Interfaces;

public interface IBomMaHangChauInsertService
{
    Task<PagedResult<BomMaHangChauInsertDto>> LayDanhSachAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken);

    Task<PagedResult<MaHangCoChauInsertDto>> LayDanhSachMaHangCoChauInsertAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken);

    Task<BomMaHangChauInsertDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BomMaHangChauInsertDto> TaoMoiAsync(TaoBomMaHangChauInsertRequest request, CancellationToken cancellationToken);
    Task<BomMaHangChauInsertDto> CapNhatAsync(Guid id, CapNhatBomMaHangChauInsertRequest request, CancellationToken cancellationToken);
    Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken);
}
