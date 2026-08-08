using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HinhDang;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.HinhDang;

public sealed class HinhDangRepository(EmanDbContext dbContext) : IHinhDangRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocHinhDangRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.HinhDangs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(entity =>
                entity.MaHinhDang.Contains(tuKhoa) ||
                entity.TenHinhDang.Contains(tuKhoa) ||
                (entity.MoTa != null && entity.MoTa.Contains(tuKhoa)));
        }
        if (request.IsActive.HasValue) query = query.Where(entity => entity.IsActive == request.IsActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaHinhDang)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.HinhDangs.AsQueryable() : dbContext.HinhDangs.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(string ma, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.HinhDangs.AnyAsync(entity => entity.MaHinhDang == ma && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.HinhDangs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.HinhDangs.Remove(entity);
}
