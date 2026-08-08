using Eman.Domain.Common.Enums;
using NhomNangLucEntity = Eman.Domain.Modules.MasterData.Production.Entities.NhomNangLuc;

namespace Eman.Application.Modules.MasterData.Production.NhomNangLuc.Interfaces;

public interface INhomNangLucRepository
{
    Task<(IReadOnlyList<NhomNangLucEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<NhomNangLucEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);

    Task<bool> TonTaiMaAsync(string maNhomNangLuc, Guid? loaiTruId, CancellationToken cancellationToken);

    Task ThemAsync(NhomNangLucEntity entity, CancellationToken cancellationToken);

    void Xoa(NhomNangLucEntity entity);
}
