using Eman.Domain.Common.Enums;
using CoSoMuaVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.CoSoMuaVatTu;

namespace Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Interfaces;

public interface ICoSoMuaVatTuRepository
{
    Task<(IReadOnlyList<CoSoMuaVatTuEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    Task<CoSoMuaVatTuEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiMaAsync(string maCoSoMuaVatTu, Guid? loaiTruId, CancellationToken cancellationToken);
    Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken);
    Task ThemAsync(CoSoMuaVatTuEntity entity, CancellationToken cancellationToken);
    void Xoa(CoSoMuaVatTuEntity entity);
}
