using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Interfaces;
using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Models;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Repositories.Engineering.Bom.TinhToan.Mau;

/// <summary>
/// Truy vấn đọc chuyên biệt cho chức năng tính B.O.M màu.
/// Không chứa công thức hoặc quy tắc nghiệp vụ.
/// </summary>
public sealed class TraCuuTinhBomMauRepository(
    EmanDbContext dbContext) : ITraCuuTinhBomMauRepository
{
    public async Task<HeVaDeTaiTraCuuBomMau?> LayHeVaDeTaiAsync(
        string maHe,
        CancellationToken cancellationToken)
    {
        var he = await dbContext.HeSanPhams
            .AsNoTracking()
            .Where(item => item.MaHe == maHe)
            .Select(item => new
            {
                item.Id,
                item.MaHe,
                item.TenHe,
                item.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (he is null)
        {
            return null;
        }

        var deTais = await dbContext.DeTais
            .AsNoTracking()
            .Where(item => item.HeSanPhamId == he.Id)
            .OrderBy(item => item.MaDeTai)
            .Select(item => new DeTaiTraCuuBomMau
            {
                Id = item.Id,
                MaDeTai = item.MaDeTai,
                TenDeTai = item.TenDeTai,
                IsActive = item.IsActive
            })
            .ToListAsync(cancellationToken);

        return new HeVaDeTaiTraCuuBomMau
        {
            HeSanPhamId = he.Id,
            MaHe = he.MaHe,
            TenHe = he.TenHe,
            IsActive = he.IsActive,
            DeTais = deTais
        };
    }

    public Task<MauSacTraCuuBomMau?> LayMauSacAsync(
        long heSanPhamId,
        long deTaiId,
        string maMau,
        CancellationToken cancellationToken)
        => dbContext.MauSacs
            .AsNoTracking()
            .Where(item =>
                item.HeSanPhamId == heSanPhamId &&
                item.DeTaiId == deTaiId &&
                item.MaMau == maMau)
            .Select(item => new MauSacTraCuuBomMau
            {
                Id = item.Id,
                MaMau = item.MaMau,
                TenMau = item.TenMau,
                MaCotTho = item.MaCotTho,
                IsActive = item.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);

    public Task<MaHangTraCuuBomMau?> LayMaHangAsync(
        string maHang,
        CancellationToken cancellationToken)
        => dbContext.MaHangs
            .AsNoTracking()
            .Where(item => item.MaHangCode == maHang)
            .Select(item => new MaHangTraCuuBomMau
            {
                Id = item.Id,
                MaHang = item.MaHangCode,
                LoaiMaHang = item.LoaiMaHang,
                DienTich = item.DienTich,
                HinhDangBomMauId = item.HinhDangBomMauId,
                MaHinhDangBomMau = item.HinhDangBomMau == null
                    ? null
                    : item.HinhDangBomMau.MaHinhDang,
                TenHinhDangBomMau = item.HinhDangBomMau == null
                    ? null
                    : item.HinhDangBomMau.TenHinhDang,
                IsActive = item.IsActive
            })
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<QuyTacNhomMTraCuuBomMau>> LayCacQuyTacNhomMAsync(
        long hinhDangBomMauId,
        CancellationToken cancellationToken)
        => await dbContext.QuyTacNhomMs
            .AsNoTracking()
            .Where(item =>
                item.HinhDangId == hinhDangBomMauId &&
                item.IsActive &&
                item.NhomM.IsActive &&
                item.NhomM.PhamViBom == "BOM_MAU")
            .OrderBy(item => item.DienTichTu)
            .Select(item => new QuyTacNhomMTraCuuBomMau
            {
                Id = item.Id,
                NhomMId = item.NhomMId,
                MaNhomM = item.NhomM.MaNhomM,
                TenNhomM = item.NhomM.TenNhomM,
                DienTichTu = item.DienTichTu,
                DienTichDen = item.DienTichDen,
                BaoGomTu = item.BaoGomTu,
                BaoGomDen = item.BaoGomDen
            })
            .ToListAsync(cancellationToken);

    public async Task<GoiDuLieuBuocTraCuuBomMau> LayGoiDuLieuBuocAsync(
        long heSanPhamId,
        long deTaiId,
        long mauSacId,
        long nhomMId,
        CancellationToken cancellationToken)
    {
        var buocNhoms = await dbContext.BuocNhomTheoMaus
            .AsNoTracking()
            .Where(item =>
                item.HeSanPhamId == heSanPhamId &&
                item.MauSacId == mauSacId &&
                item.IsActive)
            .OrderBy(item => item.Id)
            .Select(item => new BuocNhomTraCuuBomMau
            {
                Id = item.Id,
                MaBuoc = item.MaBuoc,
                TenBuoc = item.TenBuoc,
                MaHonHopId = item.MaHonHopId,
                MaHonHop = item.MaHonHop
            })
            .ToListAsync(cancellationToken);

        if (buocNhoms.Count == 0)
        {
            return new GoiDuLieuBuocTraCuuBomMau();
        }

        var maBuocs = buocNhoms
            .Select(item => item.MaBuoc)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var buocNhomIds = buocNhoms
            .Select(item => item.Id)
            .ToList();

        var buocs = await dbContext.BomMauBuocs
            .AsNoTracking()
            .Where(item => item.IsActive && maBuocs.Contains(item.MaBuoc))
            .Select(item => new BuocDanhMucTraCuuBomMau
            {
                Id = item.Id,
                MaBuoc = item.MaBuoc,
                TenBuoc = item.TenBuoc
            })
            .ToListAsync(cancellationToken);

        var buocIds = buocs
            .Select(item => item.Id)
            .ToList();

        var dinhMucs = await dbContext.BomMauDinhMucNhomMs
            .AsNoTracking()
            .Where(item =>
                item.IsActive &&
                item.NhomMId == nhomMId &&
                buocNhomIds.Contains(item.BuocNhomMauId))
            .Select(item => new DinhMucTraCuuBomMau
            {
                Id = item.Id,
                BuocNhomMauId = item.BuocNhomMauId,
                DinhMuc = item.DinhMuc
            })
            .ToListAsync(cancellationToken);

        IReadOnlyList<HeSoDeTaiTraCuuBomMau> heSoDeTais = buocIds.Count == 0
            ? Array.Empty<HeSoDeTaiTraCuuBomMau>()
            : await dbContext.BomMauHeSoDeTais
                .AsNoTracking()
                .Where(item =>
                    item.IsActive &&
                    item.HeSanPhamId == heSanPhamId &&
                    item.DeTaiId == deTaiId &&
                    buocIds.Contains(item.BuocId))
                .Select(item => new HeSoDeTaiTraCuuBomMau
                {
                    Id = item.Id,
                    BuocId = item.BuocId,
                    HeSo = item.HeSo
                })
                .ToListAsync(cancellationToken);

        IReadOnlyList<HeSoMauTraCuuBomMau> heSoMaus = buocIds.Count == 0
            ? Array.Empty<HeSoMauTraCuuBomMau>()
            : await dbContext.BomMauHeSoMaus
                .AsNoTracking()
                .Where(item =>
                    item.IsActive &&
                    item.HeSanPhamId == heSanPhamId &&
                    item.DeTaiId == deTaiId &&
                    item.MauSacId == mauSacId &&
                    buocIds.Contains(item.BuocId))
                .Select(item => new HeSoMauTraCuuBomMau
                {
                    Id = item.Id,
                    BuocId = item.BuocId,
                    HeSo = item.HeSo
                })
                .ToListAsync(cancellationToken);

        return new GoiDuLieuBuocTraCuuBomMau
        {
            BuocNhoms = buocNhoms,
            Buocs = buocs,
            DinhMucs = dinhMucs,
            HeSoDeTais = heSoDeTais,
            HeSoMaus = heSoMaus
        };
    }

    public async Task<IReadOnlyList<ChauInsertTraCuuBomMau>> LayChauInsertsAsync(
        long maHangId,
        CancellationToken cancellationToken)
        => await dbContext.BomMaHangChauInserts
            .AsNoTracking()
            .Where(item => item.MaHangId == maHangId && item.IsActive)
            .OrderBy(item => item.MaChauInsert)
            .Select(item => new ChauInsertTraCuuBomMau
            {
                ChauInsertId = item.ChauInsertId,
                MaChauInsert = item.MaChauInsert,
                TenChauInsert = item.ChauInsert.TenChauInsert,
                SoLuong = item.SoLuong,
                GhiChu = item.GhiChu,
                IsActive = item.ChauInsert.IsActive
            })
            .ToListAsync(cancellationToken);

    public Task<PhenTraCuuBomMau?> LayPhenAsync(
        long maHangId,
        CancellationToken cancellationToken)
        => dbContext.BomMaHangPhens
            .AsNoTracking()
            .Where(item => item.MaHangId == maHangId && item.IsActive)
            .Select(item => new PhenTraCuuBomMau
            {
                MaHangPhen = item.MaHangPhen,
                GhiChu = item.GhiChu
            })
            .SingleOrDefaultAsync(cancellationToken);
}
