using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.ChauInsert;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.ChauInsert;

public sealed class ChauInsertRepository(EmanDbContext dbContext) : IChauInsertRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocChauInsertRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.ChauInserts.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(entity =>
                entity.MaChauInsert.Contains(tuKhoa) ||
                (entity.TenChauInsert != null && entity.TenChauInsert.Contains(tuKhoa)) ||
                (entity.MoTa != null && entity.MoTa.Contains(tuKhoa)));
        }
        if (request.IsActive.HasValue) query = query.Where(entity => entity.IsActive == request.IsActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaChauInsert)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.ChauInserts.AsQueryable() : dbContext.ChauInserts.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(string ma, Guid? loaiTruId, CancellationToken cancellationToken)
        => dbContext.ChauInserts.AnyAsync(entity => entity.MaChauInsert == ma && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.ChauInserts.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.ChauInserts.Remove(entity);
}
