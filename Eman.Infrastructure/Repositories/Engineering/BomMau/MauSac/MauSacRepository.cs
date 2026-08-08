using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MauSac;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.MauSac;

public sealed class MauSacRepository(EmanDbContext dbContext) : IMauSacRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocMauSacRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.MauSacs.AsNoTracking());
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x => x.MaMau.Contains(tuKhoa)
                || x.TenMau.Contains(tuKhoa)
                || (x.MaCotTho != null && x.MaCotTho.Contains(tuKhoa))
                || (x.MoTa != null && x.MoTa.Contains(tuKhoa)));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.HeSanPhamId.HasValue) query = query.Where(x => x.HeSanPhamId == request.HeSanPhamId.Value);
        if (request.DeTaiId.HasValue) query = query.Where(x => x.DeTaiId == request.DeTaiId.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.HeSanPham.MaHe)
            .ThenBy(x => x.DeTai.MaDeTai)
            .ThenBy(x => x.MaMau)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.MauSacs.AsQueryable() : dbContext.MauSacs.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(long heSanPhamId, long deTaiId, string maMau, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.MauSacs.AnyAsync(
            x => x.HeSanPhamId == heSanPhamId
                 && x.DeTaiId == deTaiId
                 && x.MaMau == maMau
                 && (!loaiTruId.HasValue || x.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.MauSacs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.MauSacs.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query.Include(x => x.HeSanPham).Include(x => x.DeTai);
}
