using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.DeTai;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;

public interface IDeTaiRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocDeTaiRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(long heSanPhamId, string maDeTai, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
