using Eman.Domain.Common.Enums;
using LoaiDoiTacEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.LoaiDoiTac;

namespace Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Interfaces;

public interface ILoaiDoiTacRepository
{
    Task<(IReadOnlyList<LoaiDoiTacEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<LoaiDoiTacEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(
        string maLoaiDoiTac,
        Guid? loaiTruId,
        CancellationToken cancellationToken);

    Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken);

    Task ThemAsync(LoaiDoiTacEntity entity, CancellationToken cancellationToken);

    void Xoa(LoaiDoiTacEntity entity);
}
