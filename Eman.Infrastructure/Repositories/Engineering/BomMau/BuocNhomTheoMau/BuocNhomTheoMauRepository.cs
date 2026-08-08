using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BuocNhomTheoMau;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BuocNhomTheoMau;

public sealed class BuocNhomTheoMauRepository(EmanDbContext dbContext)
    : IBuocNhomTheoMauRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(
        BoLocBuocNhomTheoMauRequest request,
        CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.BuocNhomTheoMaus.AsNoTracking());

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x =>
                x.MaBuoc.Contains(tuKhoa)
                || x.TenBuoc.Contains(tuKhoa)
                || x.MaHonHop.Contains(tuKhoa)
                || (x.GhiChu != null && x.GhiChu.Contains(tuKhoa)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.HeSanPhamId.HasValue)
        {
            query = query.Where(x => x.HeSanPhamId == request.HeSanPhamId.Value);
        }

        if (request.DeTaiId.HasValue)
        {
            query = query.Where(x => x.MauSac.DeTaiId == request.DeTaiId.Value);
        }

        if (request.MauSacId.HasValue)
        {
            query = query.Where(x => x.MauSacId == request.MauSacId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.MaBuoc))
        {
            var maBuoc = request.MaBuoc.Trim();
            query = query.Where(x => x.MaBuoc == maBuoc);
        }

        if (request.MaHonHopId.HasValue)
        {
            query = query.Where(x => x.MaHonHopId == request.MaHonHopId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.HeSanPham.MaHe)
            .ThenBy(x => x.MauSac.MaMau)
            .ThenBy(x => x.MaBuoc)
            .ThenBy(x => x.MaHonHopId)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(
        long id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.BuocNhomTheoMaus.AsQueryable()
            : dbContext.BuocNhomTheoMaus.AsNoTracking();

        return ThemDuLieuLienQuan(query)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(
        long heSanPhamId,
        long mauSacId,
        string maBuoc,
        long maHonHopId,
        long? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.BuocNhomTheoMaus.AnyAsync(
            x => x.HeSanPhamId == heSanPhamId
                 && x.MauSacId == mauSacId
                 && x.MaBuoc == maBuoc
                 && x.MaHonHopId == maHonHopId
                 && (!loaiTruId.HasValue || x.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.BuocNhomTheoMaus.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity)
        => dbContext.BuocNhomTheoMaus.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query
            .Include(x => x.HeSanPham)
            .Include(x => x.MauSac)
                .ThenInclude(x => x.DeTai);
}
