using Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DonViTinhEntity = Eman.Domain.Modules.MasterData.Common.Entities.DonViTinh;

namespace Eman.Infrastructure.Repositories.MasterData.Common.DonViTinh;

public sealed class DonViTinhRepository(EmanDbContext dbContext) : IDonViTinhRepository
{
    public async Task<(IReadOnlyList<DonViTinhEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.DonViTinhs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaDonViTinh.Contains(tuKhoa)
                || entity.TenDonViTinh.Contains(tuKhoa)
                || (entity.KyHieu != null && entity.KyHieu.Contains(tuKhoa))
                || (entity.MoTa != null && entity.MoTa.Contains(tuKhoa)));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaDonViTinh)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<DonViTinhEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.DonViTinhs.AsQueryable()
            : dbContext.DonViTinhs.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maDonViTinh,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.DonViTinhs.AnyAsync(entity =>
            entity.MaDonViTinh == maDonViTinh
            && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(DonViTinhEntity entity, CancellationToken cancellationToken)
        => dbContext.DonViTinhs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(DonViTinhEntity entity) => dbContext.DonViTinhs.Remove(entity);
}
