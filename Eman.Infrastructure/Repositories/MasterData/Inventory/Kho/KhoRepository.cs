using Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using KhoEntity = Eman.Domain.Modules.MasterData.Inventory.Entities.Kho;

namespace Eman.Infrastructure.Repositories.MasterData.Inventory.Kho;

public sealed class KhoRepository(EmanDbContext dbContext) : IKhoRepository
{
    public async Task<(IReadOnlyList<KhoEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        bool? hangTon,
        bool? hangTru,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Khos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaKho.Contains(tuKhoa)
                || entity.TenKho.Contains(tuKhoa)
                || (entity.MoTa != null && entity.MoTa.Contains(tuKhoa)));
        }

        if (hangTon.HasValue)
        {
            query = query.Where(entity => entity.HangTon == hangTon.Value);
        }

        if (hangTru.HasValue)
        {
            query = query.Where(entity => entity.HangTru == hangTru.Value);
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaKho)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<KhoEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.Khos.AsQueryable() : dbContext.Khos.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maKho,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.Khos.AnyAsync(entity =>
            entity.MaKho == maKho
            && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(KhoEntity entity, CancellationToken cancellationToken)
        => dbContext.Khos.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(KhoEntity entity) => dbContext.Khos.Remove(entity);
}
