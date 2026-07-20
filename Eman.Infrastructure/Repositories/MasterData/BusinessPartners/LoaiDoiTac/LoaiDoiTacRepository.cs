using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using LoaiDoiTacEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.LoaiDoiTac;

namespace Eman.Infrastructure.Repositories.MasterData.BusinessPartners.LoaiDoiTac;

public sealed class LoaiDoiTacRepository(EmanDbContext dbContext) : ILoaiDoiTacRepository
{
    public async Task<(IReadOnlyList<LoaiDoiTacEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.LoaiDoiTacs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaLoaiDoiTac.Contains(tuKhoa) ||
                entity.TenLoaiDoiTac.Contains(tuKhoa));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.MaLoaiDoiTac)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<LoaiDoiTacEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.LoaiDoiTacs.AsQueryable()
            : dbContext.LoaiDoiTacs.AsNoTracking();

        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maLoaiDoiTac,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.LoaiDoiTacs.AnyAsync(
            entity => entity.MaLoaiDoiTac == maLoaiDoiTac &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.DoiTacKinhDoanhs.AnyAsync(
            entity => entity.LoaiDoiTacId == id,
            cancellationToken);

    public Task ThemAsync(LoaiDoiTacEntity entity, CancellationToken cancellationToken)
        => dbContext.LoaiDoiTacs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(LoaiDoiTacEntity entity)
        => dbContext.LoaiDoiTacs.Remove(entity);
}
