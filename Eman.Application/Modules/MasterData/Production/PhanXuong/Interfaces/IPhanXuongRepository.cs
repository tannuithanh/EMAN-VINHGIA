using Eman.Domain.Common.Enums;
using PhanXuongEntity = Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong;

namespace Eman.Application.Modules.MasterData.Production.PhanXuong.Interfaces;

public interface IPhanXuongRepository
{
    Task<(IReadOnlyList<PhanXuongEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<PhanXuongEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(string ma, Guid? loaiTruId, CancellationToken cancellationToken);

    Task ThemAsync(PhanXuongEntity entity, CancellationToken cancellationToken);

    void Xoa(PhanXuongEntity entity);
}
