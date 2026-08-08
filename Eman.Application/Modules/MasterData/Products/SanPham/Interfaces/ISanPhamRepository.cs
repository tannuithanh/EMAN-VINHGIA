using Eman.Domain.Common.Enums;
using SanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.SanPham;

namespace Eman.Application.Modules.MasterData.Products.SanPham.Interfaces;

public interface ISanPhamRepository
{
    Task<(IReadOnlyList<SanPhamEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        Guid? donViTinhId,
        Guid? nhomNangLucId,
        Guid? khoMacDinhId,
        Guid? khoTonId,
        Guid? xuongMacDinhId,
        Guid? thueId,
        bool? laBanThanhPham,
        string? noiGiaoHang,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<SanPhamEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(
        string maSanPham,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> TonTaiIdAsync(Guid id, CancellationToken cancellationToken);

    Task ThemAsync(SanPhamEntity entity, CancellationToken cancellationToken);

    void Xoa(SanPhamEntity entity);
}
