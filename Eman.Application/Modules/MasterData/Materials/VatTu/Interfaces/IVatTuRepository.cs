using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using PhanXuongEntity = Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;

public interface IVatTuRepository
{
    Task<(IReadOnlyList<VatTuEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        Guid? donViTinhId,
        Guid? nhomVatTuId,
        Guid? coSoMuaVatTuId,
        Guid? nhaCungCapMacDinhId,
        Guid? thueVatId,
        Guid? khoLuuTruId,
        Guid? phanXuongId,
        PhamViSuDungVatTu? phamViSuDung,
        PhuongThucCungUngVatTu? phuongThucCungUng,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<VatTuEntity>> LayDanhSachXuatAsync(
        string? keyword,
        Guid? donViTinhId,
        Guid? nhomVatTuId,
        Guid? coSoMuaVatTuId,
        Guid? nhaCungCapMacDinhId,
        Guid? thueVatId,
        Guid? khoLuuTruId,
        Guid? phanXuongId,
        PhamViSuDungVatTu? phamViSuDung,
        PhuongThucCungUngVatTu? phuongThucCungUng,
        TrangThaiHoatDong? trangThai,
        CancellationToken cancellationToken);

    Task<VatTuEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string maVatTu, Guid? loaiTruId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PhanXuongEntity>> LayPhanXuongsHoatDongTheoIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken);
    Task ThemAsync(VatTuEntity entity, CancellationToken cancellationToken);
    void Xoa(VatTuEntity entity);
}
