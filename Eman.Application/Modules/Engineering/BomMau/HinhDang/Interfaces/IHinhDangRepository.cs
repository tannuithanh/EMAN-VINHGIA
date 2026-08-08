using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HinhDang;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;

public interface IHinhDangRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocHinhDangRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string ma, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
