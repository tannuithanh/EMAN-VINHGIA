
using Eman.Domain.Common.Enums;
using DieuKienGiaoHangEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienGiaoHang;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Interfaces;

public interface IDieuKienGiaoHangRepository
{
    Task<(IReadOnlyList<DieuKienGiaoHangEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<DieuKienGiaoHangEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(
        string maDieuKienGiaoHang,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken);

    Task ThemAsync(DieuKienGiaoHangEntity entity, CancellationToken cancellationToken);

    void Xoa(DieuKienGiaoHangEntity entity);
}
