using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.ChauInsert;

namespace Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Interfaces;

public interface IChauInsertRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocChauInsertRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string ma, Guid? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
