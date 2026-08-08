using Eman.Domain.Common.Enums;
using KhoEntity = Eman.Domain.Modules.MasterData.Inventory.Entities.Kho;

namespace Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;

public interface IKhoRepository
{
    Task<(IReadOnlyList<KhoEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        bool? hangTon,
        bool? hangTru,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<KhoEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(string maKho, Guid? loaiTruId, CancellationToken cancellationToken);

    Task ThemAsync(KhoEntity entity, CancellationToken cancellationToken);

    void Xoa(KhoEntity entity);
}
