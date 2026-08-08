using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauHeSoDeTai;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Interfaces;

public interface IBomMauHeSoDeTaiRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(long heSanPhamId, long deTaiId, long buocId, long? loaiTruId, CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
