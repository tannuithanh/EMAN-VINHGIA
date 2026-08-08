using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BuocNhomTheoMau;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Interfaces;

public interface IBuocNhomTheoMauRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(
        BoLocBuocNhomTheoMauRequest request,
        CancellationToken cancellationToken);

    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);

    Task<bool> TonTaiTrungAsync(
        long heSanPhamId,
        long mauSacId,
        string maBuoc,
        long maHonHopId,
        long? loaiTruId,
        CancellationToken cancellationToken);

    Task ThemAsync(Entity entity, CancellationToken cancellationToken);

    void Xoa(Entity entity);
}
