using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangChauInsert;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Interfaces;

public interface IBomMaHangChauInsertRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken);

    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachMaHangCoChauInsertAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken);

    Task<Entity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);

    Task<bool> TonTaiTrungAsync(
        long maHangId,
        Guid chauInsertId,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
