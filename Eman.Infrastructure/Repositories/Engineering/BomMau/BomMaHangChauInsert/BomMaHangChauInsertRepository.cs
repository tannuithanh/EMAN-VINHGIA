using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangChauInsert;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMaHangChauInsert;

public sealed class BomMaHangChauInsertRepository(EmanDbContext dbContext) : IBomMaHangChauInsertRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
    {
        var query = TaoTruyVanLoc(request);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await ThemDuLieuLienQuan(query)
            .OrderBy(x => x.MaHang)
            .ThenBy(x => x.MaChauInsert)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachMaHangCoChauInsertAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
    {
        var query = TaoTruyVanLoc(request);
        var totalCount = await query
            .Select(x => x.MaHangId)
            .Distinct()
            .CountAsync(cancellationToken);

        var maHangIds = await query
            .GroupBy(x => new { x.MaHangId, x.MaHang })
            .OrderBy(nhom => nhom.Key.MaHang)
            .ThenBy(nhom => nhom.Key.MaHangId)
            .Select(nhom => nhom.Key.MaHangId)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        if (maHangIds.Count == 0)
        {
            return ([], totalCount);
        }

        var items = await ThemDuLieuLienQuan(query.Where(x => maHangIds.Contains(x.MaHangId)))
            .OrderBy(x => x.MaHang)
            .ThenBy(x => x.MaChauInsert)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.BomMaHangChauInserts.AsQueryable()
            : dbContext.BomMaHangChauInserts.AsNoTracking();

        return ThemDuLieuLienQuan(query)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(
        long maHangId,
        Guid chauInsertId,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.BomMaHangChauInserts.AnyAsync(
            x => x.MaHangId == maHangId
                 && x.ChauInsertId == chauInsertId
                 && (!loaiTruId.HasValue || x.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.BomMaHangChauInserts.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.BomMaHangChauInserts.Remove(entity);

    private IQueryable<Entity> TaoTruyVanLoc(BoLocBomMaHangChauInsertRequest request)
    {
        var query = dbContext.BomMaHangChauInserts.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x =>
                x.MaHang.Contains(tuKhoa)
                || x.MaChauInsert.Contains(tuKhoa)
                || (x.ChauInsert.TenChauInsert != null && x.ChauInsert.TenChauInsert.Contains(tuKhoa))
                || (x.GhiChu != null && x.GhiChu.Contains(tuKhoa)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.MaHangId.HasValue)
        {
            query = query.Where(x => x.MaHangId == request.MaHangId.Value);
        }

        if (request.ChauInsertId.HasValue)
        {
            query = query.Where(x => x.ChauInsertId == request.ChauInsertId.Value);
        }

        return query;
    }

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query
            .Include(x => x.MaHangNavigation)
            .Include(x => x.ChauInsert);
}
