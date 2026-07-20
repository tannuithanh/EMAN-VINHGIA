using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Interfaces;
using Eman.Domain.Modules.MasterData.BusinessPartners.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PhienBanBangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.PhienBanBangGia;

namespace Eman.Infrastructure.Repositories.MasterData.BusinessPartners.PhienBanBangGia;

public sealed class PhienBanBangGiaRepository(EmanDbContext dbContext)
    : IPhienBanBangGiaRepository
{
    public async Task<(IReadOnlyList<PhienBanBangGiaEntity> Items, int TotalCount)> LayDanhSachAsync(
        Guid? bangGiaId,
        TrangThaiPhienBanBangGia? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.PhienBanBangGias
            .AsNoTracking()
            .Include(entity => entity.BangGia)
            .AsQueryable();

        if (bangGiaId.HasValue)
        {
            query = query.Where(entity => entity.BangGiaId == bangGiaId.Value);
        }

        if (trangThai.HasValue)
        {
            query = query.Where(entity => entity.TrangThai == trangThai.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(entity => entity.BangGia.MaBangGia)
            .ThenByDescending(entity => entity.SoPhienBan)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<PhienBanBangGiaEntity?> LayTheoIdAsync(
        Guid id,
        bool theoDoi,
        CancellationToken cancellationToken)
    {
        var query = theoDoi
            ? dbContext.PhienBanBangGias.AsQueryable()
            : dbContext.PhienBanBangGias.AsNoTracking();

        return query
            .Include(entity => entity.BangGia)
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiSoPhienBanAsync(
        Guid bangGiaId,
        int soPhienBan,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.PhienBanBangGias.AnyAsync(
            entity => entity.BangGiaId == bangGiaId &&
                      entity.SoPhienBan == soPhienBan &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task<bool> CoKhoangThoiGianChongLapAsync(
        Guid bangGiaId,
        DateOnly tuNgay,
        DateOnly? denNgay,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
    {
        var ngayKetThuc = denNgay ?? DateOnly.MaxValue;

        return dbContext.PhienBanBangGias.AnyAsync(
            entity => entity.BangGiaId == bangGiaId &&
                      entity.TrangThai != TrangThaiPhienBanBangGia.Huy &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value) &&
                      entity.TuNgay <= ngayKetThuc &&
                      (!entity.DenNgay.HasValue || entity.DenNgay.Value >= tuNgay),
            cancellationToken);
    }

    public Task<bool> CoPhienBanDangHieuLucAsync(
        Guid bangGiaId,
        Guid? loaiTruId,
        CancellationToken cancellationToken)
        => dbContext.PhienBanBangGias.AnyAsync(
            entity => entity.BangGiaId == bangGiaId &&
                      entity.TrangThai == TrangThaiPhienBanBangGia.HieuLuc &&
                      (!loaiTruId.HasValue || entity.Id != loaiTruId.Value),
            cancellationToken);

    public Task ThemAsync(PhienBanBangGiaEntity entity, CancellationToken cancellationToken)
        => dbContext.PhienBanBangGias.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(PhienBanBangGiaEntity entity)
        => dbContext.PhienBanBangGias.Remove(entity);
}
