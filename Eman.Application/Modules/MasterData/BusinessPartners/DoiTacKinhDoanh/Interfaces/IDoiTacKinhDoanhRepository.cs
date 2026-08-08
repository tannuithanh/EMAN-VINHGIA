using DoiTacKinhDoanhEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DoiTacKinhDoanh;
using Eman.Domain.Common.Enums;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;

public interface IDoiTacKinhDoanhRepository
{
    Task<(IReadOnlyList<DoiTacKinhDoanhEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        Guid? loaiDoiTacId,
        bool? laNhaCungCap,
        Guid? dieuKienThanhToanId,
        Guid? dieuKienGiaoHangId,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<DoiTacKinhDoanhEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(
        string maDoiTac,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> CoBangGiaAsync(Guid id, CancellationToken cancellationToken);

    Task ThemAsync(DoiTacKinhDoanhEntity entity, CancellationToken cancellationToken);

    void Xoa(DoiTacKinhDoanhEntity entity);
}
