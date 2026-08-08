using Eman.Domain.Common.Enums;
using ThueSanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.ThueSanPham;

namespace Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;

public interface IThueSanPhamRepository
{
    Task<(IReadOnlyList<ThueSanPhamEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<ThueSanPhamEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(string maThue, Guid? loaiTruId, CancellationToken cancellationToken);

    Task ThemAsync(ThueSanPhamEntity entity, CancellationToken cancellationToken);

    void Xoa(ThueSanPhamEntity entity);
}
