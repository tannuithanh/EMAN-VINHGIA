
using Eman.Domain.Common.Enums;
using DieuKienThanhToanEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienThanhToan;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Interfaces;

public interface IDieuKienThanhToanRepository
{
    Task<(IReadOnlyList<DieuKienThanhToanEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<DieuKienThanhToanEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(
        string maDieuKienThanhToan,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken);

    Task ThemAsync(DieuKienThanhToanEntity entity, CancellationToken cancellationToken);

    void Xoa(DieuKienThanhToanEntity entity);
}
