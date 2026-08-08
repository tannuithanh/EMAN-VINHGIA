using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauBuoc;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Interfaces;

public interface IBomMauBuocRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauBuocRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string ma, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
