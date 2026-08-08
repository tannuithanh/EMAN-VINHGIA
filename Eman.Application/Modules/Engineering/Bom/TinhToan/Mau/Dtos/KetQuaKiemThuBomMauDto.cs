namespace Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Dtos;

/// <summary>
/// Kết quả chẩn đoán và tính thử B.O.M màu từ một mã sản phẩm hoàn chỉnh.
/// </summary>
public sealed class KetQuaKiemThuBomMauDto
{
    public string MaSanPham { get; set; } = string.Empty;
    public bool DaTinhThanhCong { get; set; }
    public bool DayDuThongTinPhuTro { get; set; }
    public string TrangThai { get; set; } = "CHUA_TINH";
    public string CongThucTongQuat { get; set; }
        = "Lượng tiêu hao = Diện tích × Định mức nhóm M × Hệ số đề tài × Hệ số màu";
    public ThongTinPhanTichMaSanPhamBomMauDto PhanTichMaSanPham { get; set; } = new();
    public ThongTinMaHangTinhBomMauDto? MaHang { get; set; }
    public ThongTinMauSacTinhBomMauDto? MauSac { get; set; }
    public ThongTinNhomMTinhBomMauDto? NhomM { get; set; }
    public List<ChiTietBuocTinhBomMauDto> CacBuoc { get; set; } = [];
    public int TongSoBuoc => CacBuoc.Count;
    public int SoBuocTinhDuoc => CacBuoc.Count(item => item.DaTinhDuoc);
    public List<ChauInsertTinhBomMauDto> ChauInserts { get; set; } = [];
    public bool CoChauInsert => ChauInserts.Count > 0;
    public int TongSoLuongChauInsert => ChauInserts.Sum(item => item.SoLuong);
    public ThongTinPhenTinhBomMauDto Phen { get; set; } = new();
    public ThongTinCotThoTinhBomMauDto CotTho { get; set; } = new();
    public List<string> LoiCauHinh { get; set; } = [];
    public List<string> CanhBao { get; set; } = [];
    public List<string> CacBangThamGiaTinhToan { get; set; } =
    [
        "md_he_san_pham",
        "md_de_tai",
        "md_mau_sac",
        "md_ma_hang",
        "md_quy_tac_nhom_m",
        "md_nhom_m",
        "md_buoc_nhom_theo_mau",
        "md_bom_buoc",
        "md_bom_mau_dinh_muc_nhom_m",
        "md_bom_mau_he_so_de_tai",
        "md_bom_mau_he_so_mau",
        "md_bom_ma_hang_chau_insert",
        "md_chau_insert",
        "md_bom_ma_hang_phen"
    ];
}

public sealed class ThongTinPhanTichMaSanPhamBomMauDto
{
    public string MaHe { get; set; } = string.Empty;
    public long? HeSanPhamId { get; set; }
    public string? TenHe { get; set; }
    public string PhanDoanChuaDeTai { get; set; } = string.Empty;
    public string MaDeTai { get; set; } = string.Empty;
    public long? DeTaiId { get; set; }
    public string? TenDeTai { get; set; }
    public string MaMau { get; set; } = string.Empty;
    public string MaHangNen { get; set; } = string.Empty;
}

public sealed class ThongTinMaHangTinhBomMauDto
{
    public long Id { get; set; }
    public string MaHang { get; set; } = string.Empty;
    public string LoaiMaHang { get; set; } = string.Empty;
    public decimal? DienTich { get; set; }
    public long? HinhDangBomMauId { get; set; }
    public string? MaHinhDangBomMau { get; set; }
    public string? TenHinhDangBomMau { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ThongTinMauSacTinhBomMauDto
{
    public long Id { get; set; }
    public string MaMau { get; set; } = string.Empty;
    public string TenMau { get; set; } = string.Empty;
    public string? MaCotTho { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ThongTinNhomMTinhBomMauDto
{
    public long Id { get; set; }
    public string MaNhomM { get; set; } = string.Empty;
    public string TenNhomM { get; set; } = string.Empty;
    public long QuyTacNhomMId { get; set; }
    public decimal DienTichTu { get; set; }
    public decimal? DienTichDen { get; set; }
    public bool BaoGomTu { get; set; }
    public bool BaoGomDen { get; set; }
}

public sealed class ChiTietBuocTinhBomMauDto
{
    public int ThuTu { get; set; }
    public long BuocNhomMauId { get; set; }
    public string MaBuoc { get; set; } = string.Empty;
    public string TenBuoc { get; set; } = string.Empty;
    public long MaHonHopId { get; set; }
    public string MaHonHop { get; set; } = string.Empty;
    public long? BuocId { get; set; }
    public decimal DienTich { get; set; }
    public long? DinhMucNhomMId { get; set; }
    public decimal? DinhMucNhomM { get; set; }
    public long? HeSoDeTaiId { get; set; }
    public decimal? HeSoDeTai { get; set; }
    public long? HeSoMauId { get; set; }
    public decimal? HeSoMau { get; set; }
    public decimal? LuongTieuHaoChuaLamTron { get; set; }
    public decimal? LuongTieuHao { get; set; }
    public int SoChuSoLamTron { get; set; } = 6;
    public string? CongThucThaySo { get; set; }
    public bool DaTinhDuoc { get; set; }
    public List<string> LoiCauHinh { get; set; } = [];
}

public sealed class ChauInsertTinhBomMauDto
{
    public Guid ChauInsertId { get; set; }
    public string MaChauInsert { get; set; } = string.Empty;
    public string? TenChauInsert { get; set; }
    public int SoLuong { get; set; }
    public string? GhiChu { get; set; }
    public bool IsActive { get; set; }
}

public sealed class ThongTinPhenTinhBomMauDto
{
    public bool CoPhen { get; set; }
    public string? MaHangPhen { get; set; }
    public string? GhiChu { get; set; }
}

public sealed class ThongTinCotThoTinhBomMauDto
{
    public string? MaCotTho { get; set; }
    public string? MaHangCotThoDuKien { get; set; }
    public bool TonTaiTrongDanhMuc { get; set; }
    public long? MaHangCotThoId { get; set; }
    public bool? IsActive { get; set; }
}
