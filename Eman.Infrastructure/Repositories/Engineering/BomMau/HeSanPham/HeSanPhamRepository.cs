using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HeSanPham;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.HeSanPham;

public sealed class HeSanPhamRepository(EmanDbContext dbContext) : IHeSanPhamRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocHeSanPhamRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.HeSanPhams.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(entity =>
                entity.MaHe.Contains(tuKhoa) ||
                entity.TenHe.Contains(tuKhoa) ||
                (entity.MoTa != null && entity.MoTa.Contains(tuKhoa)));
        }
        if (request.IsActive.HasValue) query = query.Where(entity => entity.IsActive == request.IsActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaHe)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.HeSanPhams.AsQueryable() : dbContext.HeSanPhams.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(string ma, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.HeSanPhams.AnyAsync(entity => entity.MaHe == ma && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value), cancellationToken);

    public Task<bool> TonTaiIdAsync(long id, CancellationToken cancellationToken)
        => dbContext.HeSanPhams.AnyAsync(entity => entity.Id == id, cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.HeSanPhams.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.HeSanPhams.Remove(entity);
}
