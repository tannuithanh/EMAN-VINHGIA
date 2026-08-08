using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.NhomM;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;

public interface INhomMRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocNhomMRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string phamViBom, string ma, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
