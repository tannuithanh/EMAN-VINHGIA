using Eman.Domain.Common.Enums;
using NhomVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.NhomVatTu;

namespace Eman.Application.Modules.MasterData.Materials.NhomVatTu.Interfaces;

public interface INhomVatTuRepository
{
    Task<(IReadOnlyList<NhomVatTuEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<NhomVatTuEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string maNhomVatTu, Guid? loaiTruId, CancellationToken cancellationToken);
    Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken);
    Task ThemAsync(NhomVatTuEntity entity, CancellationToken cancellationToken);
    void Xoa(NhomVatTuEntity entity);
}
