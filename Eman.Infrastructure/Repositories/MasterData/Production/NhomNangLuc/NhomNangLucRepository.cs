using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NhomNangLucEntity = Eman.Domain.Modules.MasterData.Production.Entities.NhomNangLuc;

namespace Eman.Infrastructure.Repositories.MasterData.Production.NhomNangLuc;

public sealed class NhomNangLucRepository(EmanDbContext dbContext) : INhomNangLucRepository
{
    public async Task<(IReadOnlyList<NhomNangLucEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword, TrangThaiHoatDong? trangThai, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.NhomNangLucs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaNhomNangLuc.Contains(tuKhoa) || entity.TenNhomNangLuc.Contains(tuKhoa));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(entity => entity.MaNhomNangLuc)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<NhomNangLucEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.NhomNangLucs.AsQueryable() : dbContext.NhomNangLucs.AsNoTracking();
        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(string maNhomNangLuc, Guid? loaiTruId, CancellationToken cancellationToken)
        => dbContext.NhomNangLucs.AnyAsync(entity =>
            entity.MaNhomNangLuc == maNhomNangLuc && (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(NhomNangLucEntity entity, CancellationToken cancellationToken)
        => dbContext.NhomNangLucs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(NhomNangLucEntity entity) => dbContext.NhomNangLucs.Remove(entity);
}
