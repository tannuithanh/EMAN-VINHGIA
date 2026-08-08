
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DieuKienThanhToanEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienThanhToan;

namespace Eman.Infrastructure.Repositories.MasterData.BusinessPartners.DieuKienThanhToan;

public sealed class DieuKienThanhToanRepository(EmanDbContext dbContext)
    : IDieuKienThanhToanRepository
{
    public async Task<(IReadOnlyList<DieuKienThanhToanEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.DieuKienThanhToans.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaDieuKienThanhToan.Contains(tuKhoa) ||
                entity.TenDieuKienThanhToan.Contains(tuKhoa));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.MaDieuKienThanhToan)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<DieuKienThanhToanEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.DieuKienThanhToans.AsQueryable()
            : dbContext.DieuKienThanhToans.AsNoTracking();

        return query.SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maDieuKienThanhToan,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.DieuKienThanhToans.AnyAsync(
            entity => entity.MaDieuKienThanhToan == maDieuKienThanhToan &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.DoiTacKinhDoanhs.AnyAsync(
            entity => entity.DieuKienThanhToanId == id,
            cancellationToken);

    public Task ThemAsync(
        DieuKienThanhToanEntity entity,
        CancellationToken cancellationToken)
        => dbContext.DieuKienThanhToans.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(DieuKienThanhToanEntity entity)
        => dbContext.DieuKienThanhToans.Remove(entity);
}
