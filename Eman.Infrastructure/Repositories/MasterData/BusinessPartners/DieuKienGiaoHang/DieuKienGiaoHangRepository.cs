
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DieuKienGiaoHangEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienGiaoHang;

namespace Eman.Infrastructure.Repositories.MasterData.BusinessPartners.DieuKienGiaoHang;

public sealed class DieuKienGiaoHangRepository(EmanDbContext dbContext)
    : IDieuKienGiaoHangRepository
{
    public async Task<(IReadOnlyList<DieuKienGiaoHangEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.DieuKienGiaoHangs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaDieuKienGiaoHang.Contains(tuKhoa) ||
                entity.TenDieuKienGiaoHang.Contains(tuKhoa));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.MaDieuKienGiaoHang)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<DieuKienGiaoHangEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.DieuKienGiaoHangs.AsQueryable()
            : dbContext.DieuKienGiaoHangs.AsNoTracking();

        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maDieuKienGiaoHang,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.DieuKienGiaoHangs.AnyAsync(
            entity => entity.MaDieuKienGiaoHang == maDieuKienGiaoHang &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.DoiTacKinhDoanhs.AnyAsync(
            entity => entity.DieuKienGiaoHangId == id,
            cancellationToken);

    public Task ThemAsync(
        DieuKienGiaoHangEntity entity,
        CancellationToken cancellationToken)
        => dbContext.DieuKienGiaoHangs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(DieuKienGiaoHangEntity entity)
        => dbContext.DieuKienGiaoHangs.Remove(entity);
}
