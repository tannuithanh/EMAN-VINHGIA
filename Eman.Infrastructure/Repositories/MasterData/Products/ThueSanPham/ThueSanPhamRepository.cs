using Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ThueSanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.ThueSanPham;

namespace Eman.Infrastructure.Repositories.MasterData.Products.ThueSanPham;

public sealed class ThueSanPhamRepository(EmanDbContext dbContext) : IThueSanPhamRepository
{
    public async Task<(IReadOnlyList<ThueSanPhamEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword, TrangThaiHoatDong? trangThai, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.ThueSanPhams.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaThue.Contains(tuKhoa) || entity.TenThue.Contains(tuKhoa));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.ThueSuat)
            .ThenBy(entity => entity.MaThue)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<ThueSanPhamEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.ThueSanPhams.AsQueryable() : dbContext.ThueSanPhams.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(string maThue, Guid? loaiTruId, CancellationToken cancellationToken)
        => dbContext.ThueSanPhams.AnyAsync(entity =>
            entity.MaThue == maThue && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(ThueSanPhamEntity entity, CancellationToken cancellationToken)
        => dbContext.ThueSanPhams.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(ThueSanPhamEntity entity) => dbContext.ThueSanPhams.Remove(entity);
}
