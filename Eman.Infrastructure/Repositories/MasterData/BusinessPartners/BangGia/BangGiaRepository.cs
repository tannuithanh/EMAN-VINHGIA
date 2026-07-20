using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.BangGia;

namespace Eman.Infrastructure.Repositories.MasterData.BusinessPartners.BangGia;

public sealed class BangGiaRepository(EmanDbContext dbContext) : IBangGiaRepository
{
    public async Task<(IReadOnlyList<BangGiaEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        Guid? doiTacKinhDoanhId,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.BangGias
            .AsNoTracking()
            .Include(entity => entity.DoiTacKinhDoanh)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaBangGia.Contains(tuKhoa) ||
                entity.TenBangGia.Contains(tuKhoa) ||
                entity.DoiTacKinhDoanh.MaDoiTac.Contains(tuKhoa) ||
                entity.DoiTacKinhDoanh.TenDoiTac.Contains(tuKhoa));
        }

        if (doiTacKinhDoanhId.HasValue)
        {
            query = query.Where(entity =>
                entity.DoiTacKinhDoanhId == doiTacKinhDoanhId.Value);
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.MaBangGia)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<BangGiaEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.BangGias.AsQueryable()
            : dbContext.BangGias.AsNoTracking();

        return query
            .Include(entity => entity.DoiTacKinhDoanh)
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maBangGia,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.BangGias.AnyAsync(
            entity => entity.MaBangGia == maBangGia &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> CoPhienBanAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.PhienBanBangGias.AnyAsync(
            entity => entity.BangGiaId == id,
            cancellationToken);

    public Task ThemAsync(BangGiaEntity entity, CancellationToken cancellationToken)
        => dbContext.BangGias.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(BangGiaEntity entity)
        => dbContext.BangGias.Remove(entity);
}
