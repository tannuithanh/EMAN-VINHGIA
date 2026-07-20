using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DoiTacKinhDoanhEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DoiTacKinhDoanh;

namespace Eman.Infrastructure.Repositories.MasterData.BusinessPartners.DoiTacKinhDoanh;

public sealed class DoiTacKinhDoanhRepository(EmanDbContext dbContext)
    : IDoiTacKinhDoanhRepository
{
    public async Task<(IReadOnlyList<DoiTacKinhDoanhEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        Guid? loaiDoiTacId,
        bool? laNhaCungCap,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.DoiTacKinhDoanhs
            .AsNoTracking()
            .Include(entity => entity.LoaiDoiTac)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaDoiTac.Contains(tuKhoa) ||
                entity.TenDoiTac.Contains(tuKhoa) ||
                (entity.MaSoThue != null && entity.MaSoThue.Contains(tuKhoa)));
        }

        if (loaiDoiTacId.HasValue)
        {
            query = query.Where(entity => entity.LoaiDoiTacId == loaiDoiTacId.Value);
        }

        if (laNhaCungCap.HasValue)
        {
            query = query.Where(entity => entity.LaNhaCungCap == laNhaCungCap.Value);
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.MaDoiTac)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<DoiTacKinhDoanhEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.DoiTacKinhDoanhs.AsQueryable()
            : dbContext.DoiTacKinhDoanhs.AsNoTracking();

        return query
            .Include(entity => entity.LoaiDoiTac)
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maDoiTac,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.DoiTacKinhDoanhs.AnyAsync(
            entity => entity.MaDoiTac == maDoiTac &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> CoBangGiaAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.BangGias.AnyAsync(
            entity => entity.DoiTacKinhDoanhId == id,
            cancellationToken);

    public Task ThemAsync(DoiTacKinhDoanhEntity entity, CancellationToken cancellationToken)
        => dbContext.DoiTacKinhDoanhs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(DoiTacKinhDoanhEntity entity)
        => dbContext.DoiTacKinhDoanhs.Remove(entity);
}
