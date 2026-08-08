using Eman.Domain.Common.Enums;
using DonViTinhEntity = Eman.Domain.Modules.MasterData.Common.Entities.DonViTinh;

namespace Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;

public interface IDonViTinhRepository
{
    Task<(IReadOnlyList<DonViTinhEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<DonViTinhEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(string ma, Guid? loaiTruId, CancellationToken cancellationToken);

    Task ThemAsync(DonViTinhEntity entity, CancellationToken cancellationToken);

    void Xoa(DonViTinhEntity entity);
}
