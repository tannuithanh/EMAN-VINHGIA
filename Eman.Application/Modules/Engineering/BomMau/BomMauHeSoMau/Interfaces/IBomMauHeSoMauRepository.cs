using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauHeSoMau;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Interfaces;

public interface IBomMauHeSoMauRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauHeSoMauRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(long heSanPhamId, long deTaiId, long mauSacId, long buocId, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
