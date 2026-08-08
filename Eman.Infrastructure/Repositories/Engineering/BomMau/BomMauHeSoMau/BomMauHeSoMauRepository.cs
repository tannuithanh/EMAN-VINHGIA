using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauHeSoMau;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauHeSoMau;

public sealed class BomMauHeSoMauRepository(EmanDbContext dbContext) : IBomMauHeSoMauRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauHeSoMauRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.BomMauHeSoMaus.AsNoTracking());
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x => x.MaHe.Contains(tuKhoa) || x.MaDeTai.Contains(tuKhoa) || x.MaMau.Contains(tuKhoa) || x.TenMau.Contains(tuKhoa) || x.TenBuoc.Contains(tuKhoa));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.HeSanPhamId.HasValue) query = query.Where(x => x.HeSanPhamId == request.HeSanPhamId.Value);
        if (request.DeTaiId.HasValue) query = query.Where(x => x.DeTaiId == request.DeTaiId.Value);
        if (request.MauSacId.HasValue) query = query.Where(x => x.MauSacId == request.MauSacId.Value);
        if (request.BuocId.HasValue) query = query.Where(x => x.BuocId == request.BuocId.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.HeSanPhamId)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.BomMauHeSoMaus.AsQueryable() : dbContext.BomMauHeSoMaus.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(long heSanPhamId, long deTaiId, long mauSacId, long buocId, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.BomMauHeSoMaus.AnyAsync(x => x.HeSanPhamId == heSanPhamId && x.DeTaiId == deTaiId && x.MauSacId == mauSacId && x.BuocId == buocId && (!loaiTruId.HasValue || x.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.BomMauHeSoMaus.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.BomMauHeSoMaus.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query.Include(x => x.HeSanPham).Include(x => x.DeTai).Include(x => x.MauSac).Include(x => x.Buoc);
}
