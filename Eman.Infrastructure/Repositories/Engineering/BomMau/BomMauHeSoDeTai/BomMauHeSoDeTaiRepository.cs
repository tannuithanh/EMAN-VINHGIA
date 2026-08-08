using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauHeSoDeTai;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauHeSoDeTai;

public sealed class BomMauHeSoDeTaiRepository(EmanDbContext dbContext) : IBomMauHeSoDeTaiRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.BomMauHeSoDeTais.AsNoTracking());
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x => x.MaHe.Contains(tuKhoa) || x.MaDeTai.Contains(tuKhoa) || x.TenDeTai.Contains(tuKhoa) || x.TenBuoc.Contains(tuKhoa));
        }
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        if (request.HeSanPhamId.HasValue) query = query.Where(x => x.HeSanPhamId == request.HeSanPhamId.Value);
        if (request.DeTaiId.HasValue) query = query.Where(x => x.DeTaiId == request.DeTaiId.Value);
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
        var query = theoDoi ? dbContext.BomMauHeSoDeTais.AsQueryable() : dbContext.BomMauHeSoDeTais.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(long heSanPhamId, long deTaiId, long buocId, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.BomMauHeSoDeTais.AnyAsync(x => x.HeSanPhamId == heSanPhamId && x.DeTaiId == deTaiId && x.BuocId == buocId && (!loaiTruId.HasValue || x.Id != loaiTruId.Value), cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.BomMauHeSoDeTais.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.BomMauHeSoDeTais.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query.Include(x => x.HeSanPham).Include(x => x.DeTai).Include(x => x.Buoc);
}
