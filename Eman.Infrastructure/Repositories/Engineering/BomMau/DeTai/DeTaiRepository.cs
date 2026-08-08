using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.DeTai;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.DeTai;

public sealed class DeTaiRepository(EmanDbContext dbContext) : IDeTaiRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocDeTaiRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.DeTais.AsNoTracking());
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x => x.MaDeTai.Contains(tuKhoa) || x.TenDeTai.Contains(tuKhoa) || (x.MoTa != null && x.MoTa.Contains(tuKhoa)));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.HeSanPhamId.HasValue) query = query.Where(x => x.HeSanPhamId == request.HeSanPhamId.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.MaDeTai)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.DeTais.AsQueryable() : dbContext.DeTais.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(long heSanPhamId, string maDeTai, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.DeTais.AnyAsync(x => x.HeSanPhamId == heSanPhamId && x.MaDeTai == maDeTai && (!loaiTruId.HasValue || x.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.DeTais.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.DeTais.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query.Include(x => x.HeSanPham);
}
