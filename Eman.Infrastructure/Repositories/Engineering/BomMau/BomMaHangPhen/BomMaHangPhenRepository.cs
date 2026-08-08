using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangPhen;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMaHangPhen;

public sealed class BomMaHangPhenRepository(EmanDbContext dbContext) : IBomMaHangPhenRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMaHangPhenRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.BomMaHangPhens.AsNoTracking());
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x => x.MaHang.Contains(tuKhoa) || x.MaHangPhen.Contains(tuKhoa) || (x.GhiChu != null && x.GhiChu.Contains(tuKhoa)));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.MaHangId.HasValue) query = query.Where(x => x.MaHangId == request.MaHangId.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.MaHang)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.BomMaHangPhens.AsQueryable() : dbContext.BomMaHangPhens.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(long maHangId, Guid? loaiTruId, CancellationToken cancellationToken)
        => dbContext.BomMaHangPhens.AnyAsync(x => x.MaHangId == maHangId && (!loaiTruId.HasValue || x.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.BomMaHangPhens.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.BomMaHangPhens.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query.Include(x => x.MaHangNavigation);
}
