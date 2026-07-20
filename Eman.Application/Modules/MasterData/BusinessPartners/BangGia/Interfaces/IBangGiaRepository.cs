using BangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.BangGia;
using Eman.Domain.Common.Enums;

namespace Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Interfaces;

public interface IBangGiaRepository
{
    Task<(IReadOnlyList<BangGiaEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        Guid? doiTacKinhDoanhId,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<BangGiaEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(
        string maBangGia,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> CoPhienBanAsync(Guid id, CancellationToken cancellationToken);

    Task ThemAsync(BangGiaEntity entity, CancellationToken cancellationToken);

    void Xoa(BangGiaEntity entity);
}
