using PhienBanBangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.PhienBanBangGia;
using Eman.Domain.Modules.MasterData.BusinessPartners.Enums;

namespace Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Interfaces;

public interface IPhienBanBangGiaRepository
{
    Task<(IReadOnlyList<PhienBanBangGiaEntity> Items, int TotalCount)> LayDanhSachAsync(
        Guid? bangGiaId,
        TrangThaiPhienBanBangGia? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<PhienBanBangGiaEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken);

    Task<bool> TonTaiSoPhienBanAsync(
        Guid bangGiaId,
        int soPhienBan,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> CoKhoangThoiGianChongLapAsync(
        Guid bangGiaId,
        DateOnly tuNgay,
        DateOnly? denNgay,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> CoPhienBanDangHieuLucAsync(
        Guid bangGiaId,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task ThemAsync(PhienBanBangGiaEntity entity, CancellationToken cancellationToken);

    void Xoa(PhienBanBangGiaEntity entity);
}
