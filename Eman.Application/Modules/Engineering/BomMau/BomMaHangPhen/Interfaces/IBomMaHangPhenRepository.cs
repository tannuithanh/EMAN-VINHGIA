using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangPhen;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Interfaces;

public interface IBomMaHangPhenRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMaHangPhenRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(long maHangId, Guid? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
