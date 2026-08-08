using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.NhomM;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.NhomM;

public sealed class NhomMRepository(EmanDbContext dbContext) : INhomMRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocNhomMRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.NhomMs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(entity =>
                entity.MaNhomM.Contains(tuKhoa) ||
                entity.TenNhomM.Contains(tuKhoa) ||
                entity.PhamViBom.Contains(tuKhoa) ||
                (entity.MoTa != null && entity.MoTa.Contains(tuKhoa)));
        }

        if (!string.IsNullOrWhiteSpace(request.PhamViBom))
        {
            var phamViBom = request.PhamViBom.Trim().ToUpperInvariant();
            query = query.Where(entity => entity.PhamViBom == phamViBom);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(entity => entity.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.PhamViBom)
            .ThenBy(entity => entity.ThuTu)
            .ThenBy(entity => entity.MaNhomM)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.NhomMs.AsQueryable() : dbContext.NhomMs.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string phamViBom,
        string ma,
        long? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.NhomMs.AnyAsync(
            entity =>
                entity.PhamViBom == phamViBom &&
                entity.MaNhomM == ma &&
                (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.NhomMs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.NhomMs.Remove(entity);
}
