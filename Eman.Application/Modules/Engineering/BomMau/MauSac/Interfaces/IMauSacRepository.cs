using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MauSac;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;

public interface IMauSacRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocMauSacRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(long heSanPhamId, long deTaiId, string maMau, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
