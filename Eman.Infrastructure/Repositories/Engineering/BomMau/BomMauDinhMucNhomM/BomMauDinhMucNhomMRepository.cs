using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauDinhMucNhomM;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauDinhMucNhomM;

public sealed class BomMauDinhMucNhomMRepository(EmanDbContext dbContext) : IBomMauDinhMucNhomMRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.BomMauDinhMucNhomMs.AsNoTracking());
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x => x.MaNhomM.Contains(tuKhoa) || x.BuocNhomMau.TenBuoc.Contains(tuKhoa) || x.BuocNhomMau.MaHonHop.Contains(tuKhoa));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.BuocNhomMauId.HasValue) query = query.Where(x => x.BuocNhomMauId == request.BuocNhomMauId.Value);
        if (request.NhomMId.HasValue) query = query.Where(x => x.NhomMId == request.NhomMId.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.BuocNhomMauId)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.BomMauDinhMucNhomMs.AsQueryable() : dbContext.BomMauDinhMucNhomMs.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(long buocNhomMauId, long nhomMId, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.BomMauDinhMucNhomMs.AnyAsync(x => x.BuocNhomMauId == buocNhomMauId && x.NhomMId == nhomMId && (!loaiTruId.HasValue || x.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.BomMauDinhMucNhomMs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.BomMauDinhMucNhomMs.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query.Include(x => x.BuocNhomMau).Include(x => x.NhomM);
}
