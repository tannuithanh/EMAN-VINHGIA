namespace Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Models;

public sealed class HeVaDeTaiTraCuuBomMau
{
    public long HeSanPhamId { get; init; }
    public string MaHe { get; init; } = string.Empty;
    public string TenHe { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyList<DeTaiTraCuuBomMau> DeTais { get; init; } = [];
}

public sealed class DeTaiTraCuuBomMau
{
    public long Id { get; init; }
    public string MaDeTai { get; init; } = string.Empty;
    public string TenDeTai { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public sealed class MauSacTraCuuBomMau
{
    public long Id { get; init; }
    public string MaMau { get; init; } = string.Empty;
    public string TenMau { get; init; } = string.Empty;
    public string? MaCotTho { get; init; }
    public bool IsActive { get; init; }
}

public sealed class MaHangTraCuuBomMau
{
    public long Id { get; init; }
    public string MaHang { get; init; } = string.Empty;
    public string LoaiMaHang { get; init; } = string.Empty;
    public decimal? DienTich { get; init; }
    public long? HinhDangBomMauId { get; init; }
    public string? MaHinhDangBomMau { get; init; }
    public string? TenHinhDangBomMau { get; init; }
    public bool IsActive { get; init; }
}

public sealed class QuyTacNhomMTraCuuBomMau
{
    public long Id { get; init; }
    public long NhomMId { get; init; }
    public string MaNhomM { get; init; } = string.Empty;
    public string TenNhomM { get; init; } = string.Empty;
    public decimal DienTichTu { get; init; }
    public decimal? DienTichDen { get; init; }
    public bool BaoGomTu { get; init; }
    public bool BaoGomDen { get; init; }
}

public sealed class GoiDuLieuBuocTraCuuBomMau
{
    public IReadOnlyList<BuocNhomTraCuuBomMau> BuocNhoms { get; init; } = [];
    public IReadOnlyList<BuocDanhMucTraCuuBomMau> Buocs { get; init; } = [];
    public IReadOnlyList<DinhMucTraCuuBomMau> DinhMucs { get; init; } = [];
    public IReadOnlyList<HeSoDeTaiTraCuuBomMau> HeSoDeTais { get; init; } = [];
    public IReadOnlyList<HeSoMauTraCuuBomMau> HeSoMaus { get; init; } = [];
}

public sealed class BuocNhomTraCuuBomMau
{
    public long Id { get; init; }
    public string MaBuoc { get; init; } = string.Empty;
    public string TenBuoc { get; init; } = string.Empty;
    public long MaHonHopId { get; init; }
    public string MaHonHop { get; init; } = string.Empty;
}

public sealed class BuocDanhMucTraCuuBomMau
{
    public long Id { get; init; }
    public string MaBuoc { get; init; } = string.Empty;
    public string TenBuoc { get; init; } = string.Empty;
}

public sealed class DinhMucTraCuuBomMau
{
    public long Id { get; init; }
    public long BuocNhomMauId { get; init; }
    public decimal DinhMuc { get; init; }
}

public sealed class HeSoDeTaiTraCuuBomMau
{
    public long Id { get; init; }
    public long BuocId { get; init; }
    public decimal HeSo { get; init; }
}

public sealed class HeSoMauTraCuuBomMau
{
    public long Id { get; init; }
    public long BuocId { get; init; }
    public decimal HeSo { get; init; }
}

public sealed class ChauInsertTraCuuBomMau
{
    public Guid ChauInsertId { get; init; }
    public string MaChauInsert { get; init; } = string.Empty;
    public string? TenChauInsert { get; init; }
    public int SoLuong { get; init; }
    public string? GhiChu { get; init; }
    public bool IsActive { get; init; }
}

public sealed class PhenTraCuuBomMau
{
    public string MaHangPhen { get; init; } = string.Empty;
    public string? GhiChu { get; init; }
}
