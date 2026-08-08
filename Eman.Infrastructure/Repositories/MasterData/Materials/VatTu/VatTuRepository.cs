using Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PhanXuongEntity = Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Infrastructure.Repositories.MasterData.Materials.VatTu;

public sealed class VatTuRepository(EmanDbContext dbContext) : IVatTuRepository
{
    public async Task<(IReadOnlyList<VatTuEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword, Guid? donViTinhId, Guid? nhomVatTuId, Guid? coSoMuaVatTuId,
        Guid? nhaCungCapMacDinhId, Guid? thueVatId, Guid? khoLuuTruId, Guid? phanXuongId,
        PhamViSuDungVatTu? phamViSuDung, PhuongThucCungUngVatTu? phuongThucCungUng,
        TrangThaiHoatDong? trangThai, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = TaoTruyVanDanhSach(
            keyword,
            donViTinhId,
            nhomVatTuId,
            coSoMuaVatTuId,
            nhaCungCapMacDinhId,
            thueVatId,
            khoLuuTruId,
            phanXuongId,
            phamViSuDung,
            phuongThucCungUng,
            trangThai);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.MaVatTu)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<VatTuEntity>> LayDanhSachXuatAsync(
        string? keyword,
        Guid? donViTinhId,
        Guid? nhomVatTuId,
        Guid? coSoMuaVatTuId,
        Guid? nhaCungCapMacDinhId,
        Guid? thueVatId,
        Guid? khoLuuTruId,
        Guid? phanXuongId,
        PhamViSuDungVatTu? phamViSuDung,
        PhuongThucCungUngVatTu? phuongThucCungUng,
        TrangThaiHoatDong? trangThai,
        CancellationToken cancellationToken)
        => await TaoTruyVanDanhSach(
                keyword,
                donViTinhId,
                nhomVatTuId,
                coSoMuaVatTuId,
                nhaCungCapMacDinhId,
                thueVatId,
                khoLuuTruId,
                phanXuongId,
                phamViSuDung,
                phuongThucCungUng,
                trangThai)
            .OrderBy(item => item.MaVatTu)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

    public Task<VatTuEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
    {
        var query = theoDoi ? dbContext.VatTus.AsQueryable() : dbContext.VatTus.AsNoTracking();
        return ThemDanhMucLienQuan(query).AsSplitQuery()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public Task<bool> TonTaiMaAsync(string maVatTu, Guid? loaiTruId, CancellationToken cancellationToken)
        => dbContext.VatTus.AnyAsync(item => item.MaVatTu == maVatTu
            && (!loaiTruId.HasValue || item.Id != loaiTruId.Value), cancellationToken);

    public async Task<IReadOnlyList<PhanXuongEntity>> LayPhanXuongsHoatDongTheoIdsAsync(
        IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
        => await dbContext.PhanXuongs.AsNoTracking()
            .Where(item => ids.Contains(item.Id) && item.TrangThai == TrangThaiHoatDong.HoatDong)
            .ToListAsync(cancellationToken);

    public Task ThemAsync(VatTuEntity entity, CancellationToken cancellationToken)
        => dbContext.VatTus.AddAsync(entity, cancellationToken).AsTask();

    public void Xoa(VatTuEntity entity) => dbContext.VatTus.Remove(entity);

    private IQueryable<VatTuEntity> TaoTruyVanDanhSach(
        string? keyword,
        Guid? donViTinhId,
        Guid? nhomVatTuId,
        Guid? coSoMuaVatTuId,
        Guid? nhaCungCapMacDinhId,
        Guid? thueVatId,
        Guid? khoLuuTruId,
        Guid? phanXuongId,
        PhamViSuDungVatTu? phamViSuDung,
        PhuongThucCungUngVatTu? phuongThucCungUng,
        TrangThaiHoatDong? trangThai)
    {
        var query = ThemDanhMucLienQuan(dbContext.VatTus.AsNoTracking()).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(item => item.MaVatTu.Contains(tuKhoa)
                || item.TenVatTu.Contains(tuKhoa)
                || (item.TenTiengAnh != null && item.TenTiengAnh.Contains(tuKhoa))
                || (item.QuyCachDongGoi != null && item.QuyCachDongGoi.Contains(tuKhoa))
                || (item.MucDichSuDung != null && item.MucDichSuDung.Contains(tuKhoa)));
        }

        if (donViTinhId.HasValue) query = query.Where(item => item.DonViTinhId == donViTinhId.Value);
        if (nhomVatTuId.HasValue) query = query.Where(item => item.NhomVatTuId == nhomVatTuId.Value);
        if (coSoMuaVatTuId.HasValue) query = query.Where(item => item.CoSoMuaVatTuId == coSoMuaVatTuId.Value);
        if (nhaCungCapMacDinhId.HasValue) query = query.Where(item => item.NhaCungCapMacDinhId == nhaCungCapMacDinhId.Value);
        if (thueVatId.HasValue) query = query.Where(item => item.ThueVatId == thueVatId.Value);
        if (khoLuuTruId.HasValue) query = query.Where(item => item.KhoLuuTruId == khoLuuTruId.Value);

        if (phanXuongId.HasValue)
        {
            query = query.Where(item => item.PhamViSuDung == PhamViSuDungVatTu.TatCaPhanXuong
                || item.PhanXuongs.Any(link => link.PhanXuongId == phanXuongId.Value));
        }

        if (phamViSuDung.HasValue) query = query.Where(item => item.PhamViSuDung == phamViSuDung.Value);
        if (phuongThucCungUng.HasValue) query = query.Where(item => item.PhuongThucCungUng == phuongThucCungUng.Value);
        if (trangThai.HasValue) query = query.Where(item => item.TrangThai == trangThai.Value);

        return query;
    }

    private static IQueryable<VatTuEntity> ThemDanhMucLienQuan(IQueryable<VatTuEntity> query)
        => query.Include(item => item.DonViTinh)
            .Include(item => item.NhomVatTu)
            .Include(item => item.CoSoMuaVatTu)
            .Include(item => item.NhaCungCapMacDinh)
            .Include(item => item.ThueVat)
            .Include(item => item.KhoLuuTru)
            .Include(item => item.PhanXuongs).ThenInclude(link => link.PhanXuong);
}
