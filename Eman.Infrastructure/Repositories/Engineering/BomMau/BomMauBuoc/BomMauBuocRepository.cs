using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauBuoc;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauBuoc;

public sealed class BomMauBuocRepository(EmanDbContext dbContext) : IBomMauBuocRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauBuocRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.BomMauBuocs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(entity =>
                entity.MaBuoc.Contains(tuKhoa) ||
                entity.TenBuoc.Contains(tuKhoa));
        }
        if (request.IsActive.HasValue) query = query.Where(entity => entity.IsActive == request.IsActive.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaBuoc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.BomMauBuocs.AsQueryable() : dbContext.BomMauBuocs.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(string ma, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.BomMauBuocs.AnyAsync(entity => entity.MaBuoc == ma && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.BomMauBuocs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.BomMauBuocs.Remove(entity);
}
