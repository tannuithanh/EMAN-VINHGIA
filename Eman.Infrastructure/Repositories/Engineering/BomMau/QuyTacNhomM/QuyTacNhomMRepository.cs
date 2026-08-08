using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Interfaces;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.QuyTacNhomM;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.QuyTacNhomM;

public sealed class QuyTacNhomMRepository(EmanDbContext dbContext) : IQuyTacNhomMRepository
{
    public async Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocQuyTacNhomMRequest request, CancellationToken cancellationToken)
    {
        var query = ThemDuLieuLienQuan(dbContext.QuyTacNhomMs.AsNoTracking());

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var tuKhoa = request.Keyword.Trim();
            query = query.Where(x =>
                x.NhomM.MaNhomM.Contains(tuKhoa) ||
                x.NhomM.TenNhomM.Contains(tuKhoa) ||
                x.NhomM.PhamViBom.Contains(tuKhoa) ||
                x.HinhDang.MaHinhDang.Contains(tuKhoa) ||
                x.HinhDang.TenHinhDang.Contains(tuKhoa) ||
                (x.GhiChu != null && x.GhiChu.Contains(tuKhoa)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.PhamViBom))
        {
            var phamViBom = request.PhamViBom.Trim().ToUpperInvariant();
            query = query.Where(x => x.NhomM.PhamViBom == phamViBom);
        }

        if (request.HinhDangId.HasValue)
        {
            query = query.Where(x => x.HinhDangId == request.HinhDangId.Value);
        }

        if (request.NhomMId.HasValue)
        {
            query = query.Where(x => x.NhomMId == request.NhomMId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.NhomM.PhamViBom)
            .ThenBy(x => x.HinhDangId)
            .ThenBy(x => x.DienTichTu)
            .ThenBy(x => x.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.QuyTacNhomMs.AsQueryable() : dbContext.QuyTacNhomMs.AsNoTracking();
        return ThemDuLieuLienQuan(query).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiTrungAsync(
        long hinhDangId,
        long nhomMId,
        long? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.QuyTacNhomMs.AnyAsync(
            x =>
                x.HinhDangId == hinhDangId &&
                x.NhomMId == nhomMId &&
                (!loaiTruId.HasValue || x.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> TonTaiKhoangChongLanAsync(
        string phamViBom,
        long hinhDangId,
        decimal dienTichTu,
        decimal? dienTichDen,
        bool baoGomTu,
        bool baoGomDen,
        long? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.QuyTacNhomMs.AnyAsync(
            x =>
                x.NhomM.PhamViBom == phamViBom &&
                x.HinhDangId == hinhDangId &&
                (!loaiTruId.HasValue || x.Id != loaiTruId.Value) &&
                (
                    x.DienTichDen == null ||
                    x.DienTichDen > dienTichTu ||
                    (x.DienTichDen == dienTichTu && x.BaoGomDen && baoGomTu)
                ) &&
                (
                    dienTichDen == null ||
                    dienTichDen > x.DienTichTu ||
                    (dienTichDen == x.DienTichTu && baoGomDen && x.BaoGomTu)
                ),
            cancellationToken);

    public Task ThemAsync(Entity entity, CancellationToken cancellationToken)
        => dbContext.QuyTacNhomMs.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(Entity entity) => dbContext.QuyTacNhomMs.Remove(entity);

    private static IQueryable<Entity> ThemDuLieuLienQuan(IQueryable<Entity> query)
        => query
            .Include(x => x.HinhDang)
            .Include(x => x.NhomM);
}
