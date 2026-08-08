using Eman.Application.Modules.Engineering.Bom.VatTu.Interfaces;
using Eman.Application.Modules.Engineering.Bom.VatTu.Models;
using Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;
using Eman.Domain.Modules.Engineering.Bom.VatTu.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.VatTu;

public sealed class BomVatTuRepository(EmanDbContext dbContext) : IBomVatTuRepository
{
    public async Task<(IReadOnlyList<BomVatTuPhienBan> Items, int TotalCount)> LayDanhSachPhienBanAsync(
        Guid? vatTuId,
        string? keyword,
        TrangThaiBomVatTuPhienBan? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.BomVatTuPhienBans.AsNoTracking()).AsQueryable();

        if (vatTuId.HasValue)
        {
            query = query.Where(entity => entity.VatTuId == vatTuId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.VatTu.MaVatTu.Contains(tuKhoa) ||
                entity.VatTu.TenVatTu.Contains(tuKhoa) ||
                (entity.GhiChu != null && entity.GhiChu.Contains(tuKhoa)));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.VatTu.MaVatTu)
            .ThenByDescending(entity => entity.SoPhienBan)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<BomVatTuPhienBan?> LayPhienBanTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.BomVatTuPhienBans.AsQueryable()
            : dbContext.BomVatTuPhienBans.AsNoTracking();

        return ThemDuLieuLienQuan(query).AsSplitQuery()
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<BomVatTuChiTiet?> LayChiTietTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.BomVatTuChiTiets.AsQueryable()
            : dbContext.BomVatTuChiTiets.AsNoTracking();

        return query
            .Include(entity => entity.BomVatTuPhienBan)
                .ThenInclude(entity => entity.VatTu)
            .Include(entity => entity.VatTuThanhPhan)
                .ThenInclude(entity => entity.DonViTinh)
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiSoPhienBanAsync(
        Guid vatTuId,
        int soPhienBan,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.BomVatTuPhienBans.AnyAsync(entity =>
            entity.VatTuId == vatTuId &&
            entity.SoPhienBan == soPhienBan &&
            (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> CoPhienBanHieuLucAsync(
        Guid vatTuId,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.BomVatTuPhienBans.AnyAsync(entity =>
            entity.VatTuId == vatTuId &&
            entity.TrangThai == TrangThaiBomVatTuPhienBan.HieuLuc &&
            (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> TonTaiThanhPhanAsync(
        Guid phienBanId,
        Guid vatTuThanhPhanId,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.BomVatTuChiTiets.AnyAsync(entity =>
            entity.BomVatTuPhienBanId == phienBanId &&
            entity.VatTuThanhPhanId == vatTuThanhPhanId &&
            (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public async Task<IReadOnlyList<QuanHeBomVatTu>> LayQuanHeBomHieuLucAsync(
        CancellationToken cancellationToken)
    {
        var items = await dbContext.BomVatTuChiTiets
            .AsNoTracking()
            .Where(entity => entity.BomVatTuPhienBan.TrangThai == TrangThaiBomVatTuPhienBan.HieuLuc)
            .Select(entity => new
            {
                VatTuDauRaId = entity.BomVatTuPhienBan.VatTuId,
                entity.VatTuThanhPhanId
            })
            .ToListAsync(cancellationToken);

        return items
            .Select(item => new QuanHeBomVatTu(item.VatTuDauRaId, item.VatTuThanhPhanId))
            .ToList();
    }

    public Task ThemPhienBanAsync(BomVatTuPhienBan entity, CancellationToken cancellationToken)
        => dbContext.BomVatTuPhienBans.AddAsync(entity, cancellationToken).AsTask();

    public Task ThemChiTietAsync(BomVatTuChiTiet entity, CancellationToken cancellationToken)
        => dbContext.BomVatTuChiTiets.AddAsync(entity, cancellationToken).AsTask();

    public void XoaPhienBan(BomVatTuPhienBan entity)
    {
        if (entity.ChiTiets.Count > 0)
        {
            dbContext.BomVatTuChiTiets.RemoveRange(entity.ChiTiets);
        }
        dbContext.BomVatTuPhienBans.Remove(entity);
    }

    public void XoaChiTiet(BomVatTuChiTiet entity)
        => dbContext.BomVatTuChiTiets.Remove(entity);

    private static IQueryable<BomVatTuPhienBan> ThemDuLieuLienQuan(IQueryable<BomVatTuPhienBan> query)
        => query
            .Include(entity => entity.VatTu)
                .ThenInclude(entity => entity.DonViTinh)
            .Include(entity => entity.ChiTiets)
                .ThenInclude(entity => entity.VatTuThanhPhan)
                    .ThenInclude(entity => entity.DonViTinh);
}
