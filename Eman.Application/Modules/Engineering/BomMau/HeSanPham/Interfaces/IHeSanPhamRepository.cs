using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HeSanPham;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;

public interface IHeSanPhamRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocHeSanPhamRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string ma, long? loaiTruId, CancellationToken cancellationToken);
    Task<bool> TonTaiIdAsync(long id, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
