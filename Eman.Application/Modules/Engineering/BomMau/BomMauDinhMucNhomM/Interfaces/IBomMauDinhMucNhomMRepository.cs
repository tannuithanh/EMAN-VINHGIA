using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauDinhMucNhomM;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Interfaces;

public interface IBomMauDinhMucNhomMRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(long buocNhomMauId, long nhomMId, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
