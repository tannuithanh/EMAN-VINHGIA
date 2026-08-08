using Eman.Application.Modules.MasterData.Products.SanPham.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.SanPham;

namespace Eman.Infrastructure.Repositories.MasterData.Products.SanPham;

public sealed class SanPhamRepository(EmanDbContext dbContext) : ISanPhamRepository
{
    public async Task<(IReadOnlyList<SanPhamEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword,
        Guid? donViTinhId,
        Guid? nhomNangLucId,
        Guid? khoMacDinhId,
        Guid? khoTonId,
        Guid? xuongMacDinhId,
        Guid? thueId,
        bool? laBanThanhPham,
        string? noiGiaoHang,
        TrangThaiHoatDong? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = ThemDanhMucLienQuan(
            dbContext.SanPhams.AsNoTracking()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(entity =>
                entity.MaSanPham.Contains(tuKhoa) ||
                entity.MoTaTiengViet.Contains(tuKhoa) ||
                (entity.MoTaTiengAnh != null && entity.MoTaTiengAnh.Contains(tuKhoa)) ||
                (entity.NoiGiaoHang != null && entity.NoiGiaoHang.Contains(tuKhoa)));
        }

        if (donViTinhId.HasValue)
        {
            query = query.Where(entity => entity.DonViTinhId == donViTinhId.Value);
        }

        if (nhomNangLucId.HasValue)
        {
            query = query.Where(entity => entity.NhomNangLucId == nhomNangLucId.Value);
        }

        if (khoMacDinhId.HasValue)
        {
            query = query.Where(entity => entity.KhoMacDinhId == khoMacDinhId.Value);
        }

        if (khoTonId.HasValue)
        {
            query = query.Where(entity => entity.KhoTonId == khoTonId.Value);
        }

        if (xuongMacDinhId.HasValue)
        {
            query = query.Where(entity => entity.XuongMacDinhId == xuongMacDinhId.Value);
        }

        if (thueId.HasValue)
        {
            query = query.Where(entity => entity.ThueId == thueId.Value);
        }

        if (laBanThanhPham.HasValue)
        {
            query = query.Where(entity => entity.LaBanThanhPham == laBanThanhPham.Value);
        }

        if (!string.IsNullOrWhiteSpace(noiGiaoHang))
        {
            var noiGiao = noiGiaoHang.Trim();
            query = query.Where(entity =>
                entity.NoiGiaoHang != null && entity.NoiGiaoHang.Contains(noiGiao));
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.MaSanPham)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<SanPhamEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.SanPhams.AsQueryable()
            : dbContext.SanPhams.AsNoTracking();

        return ThemDanhMucLienQuan(query)
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(
        string maSanPham,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.SanPhams.AnyAsync(
            entity => entity.MaSanPham == maSanPham &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> TonTaiIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.SanPhams.AnyAsync(entity => entity.Id == id, cancellationToken);

    public Task ThemAsync(SanPhamEntity entity, CancellationToken cancellationToken)
        => dbContext.SanPhams.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(SanPhamEntity entity)
        => dbContext.SanPhams.Remove(entity);

    private static IQueryable<SanPhamEntity> ThemDanhMucLienQuan(
        IQueryable<SanPhamEntity> query)
        => query
            .Include(entity => entity.DonViTinh)
            .Include(entity => entity.NhomNangLuc)
            .Include(entity => entity.KhoMacDinh)
            .Include(entity => entity.KhoTon)
            .Include(entity => entity.XuongMacDinh)
            .Include(entity => entity.Thue);
}
