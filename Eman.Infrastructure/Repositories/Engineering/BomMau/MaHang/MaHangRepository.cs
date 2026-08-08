using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MaHang;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.MaHang;

public sealed class MaHangRepository(EmanDbContext dbContext) : IMaHangRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocMaHangRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.MaHangs.AsNoTracking());

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x =>
                x.MaHangCode.Contains(tuKhoa) ||
                (x.MoTa != null && x.MoTa.Contains(tuKhoa)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.HinhDangBomThoId.HasValue)
        {
            query = query.Where(x => x.HinhDangBomThoId == request.HinhDangBomThoId.Value);
        }

        if (request.HinhDangBomMauId.HasValue)
        {
            query = query.Where(x => x.HinhDangBomMauId == request.HinhDangBomMauId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.LoaiMaHang))
        {
            var loaiMaHang = request.LoaiMaHang.Trim();
            query = query.Where(x => x.LoaiMaHang == loaiMaHang);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.MaHangCode)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.MaHangs.AsQueryable() : dbContext.MaHangs.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(string maHang, long? loaiTruId, CancellationToken cancellationToken)
        => dbContext.MaHangs.AnyAsync(
            x => x.MaHangCode == maHang && (!loaiTruId.HasValue || x.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.MaHangs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.MaHangs.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query
            .Include(x => x.HinhDangBomTho)
            .Include(x => x.HinhDangBomMau);
}
