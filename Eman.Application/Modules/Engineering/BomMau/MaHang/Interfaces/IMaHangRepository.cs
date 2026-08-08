using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MaHang;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;

public interface IMaHangRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocMaHangRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(string maHang, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
