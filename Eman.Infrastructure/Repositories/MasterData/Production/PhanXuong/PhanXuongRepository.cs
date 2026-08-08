using Eman.Application.Modules.MasterData.Production.PhanXuong.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PhanXuongEntity = Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong;

namespace Eman.Infrastructure.Repositories.MasterData.Production.PhanXuong;

public sealed class PhanXuongRepository(EmanDbContext dbContext) : IPhanXuongRepository
{
    public async Task<(IReadOnlyList<PhanXuongEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.PhanXuongs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaPhanXuong.Contains(tuKhoa)
                || entity.TenPhanXuong.Contains(tuKhoa)
                || (entity.MoTa != null && entity.MoTa.Contains(tuKhoa)));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaPhanXuong)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<PhanXuongEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.PhanXuongs.AsQueryable()
            : dbContext.PhanXuongs.AsNoTracking();

        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string ma,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.PhanXuongs.AnyAsync(entity =>
            entity.MaPhanXuong == ma && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(PhanXuongEntity entity, CancellationToken cancellationToken)
        => dbContext.PhanXuongs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(PhanXuongEntity entity) => dbContext.PhanXuongs.Remove(entity);
}
