using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CoSoMuaVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.CoSoMuaVatTu;

namespace Eman.Infrastructure.Repositories.MasterData.Materials.CoSoMuaVatTu;

public sealed class CoSoMuaVatTuRepository(EmanDbContext dbContext) : ICoSoMuaVatTuRepository
{
    public async Task<(IReadOnlyList<CoSoMuaVatTuEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword, TrangThaiHoatDong? trangThai, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.CoSoMuaVatTus.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(item => item.MaCoSoMuaVatTu.Contains(tuKhoa)
                || item.TenCoSoMuaVatTu.Contains(tuKhoa)
                || (item.MoTa != null && item.MoTa.Contains(tuKhoa)));
        }
        if (trangThai.HasValue) query = query.Where(item => item.TrangThai == trangThai.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.MaCoSoMuaVatTu)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }
    public Task<CoSoMuaVatTuEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
        => (theoDoi ? dbContext.CoSoMuaVatTus.AsQueryable() : dbContext.CoSoMuaVatTus.AsNoTracking())
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
    public Task<bool> TonTaiMaAsync(string maCoSoMuaVatTu, Guid? loaiTruId, CancellationToken cancellationToken)
        => dbContext.CoSoMuaVatTus.AnyAsync(item => item.MaCoSoMuaVatTu == maCoSoMuaVatTu
            && (!loaiTruId.HasValue || item.Id != loaiTruId.Value), cancellationToken);
    public Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.VatTus.AnyAsync(item => item.CoSoMuaVatTuId == id, cancellationToken);
    public Task ThemAsync(CoSoMuaVatTuEntity entity, CancellationToken cancellationToken)
        => dbContext.CoSoMuaVatTus.AddAsync(entity, cancellationToken).AsTask();
    public void Xoa(CoSoMuaVatTuEntity entity) => dbContext.CoSoMuaVatTus.Remove(entity);
}
