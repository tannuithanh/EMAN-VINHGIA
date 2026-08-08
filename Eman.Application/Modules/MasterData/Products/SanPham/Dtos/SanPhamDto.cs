namespace Eman.Application.Modules.MasterData.Products.SanPham.Dtos;

public sealed record SanPhamDto
{
    public Guid Id { get; init; }
    public string MaSanPham { get; init; } = string.Empty;
    public string MoTaTiengViet { get; init; } = string.Empty;
    public string? MoTaTiengAnh { get; init; }

    public Guid DonViTinhId { get; init; }
    public string MaDonViTinh { get; init; } = string.Empty;
    public string TenDonViTinh { get; init; } = string.Empty;
    public string? KyHieuDonViTinh { get; init; }

    public Guid? NhomNangLucId { get; init; }
    public string? MaNhomNangLuc { get; init; }
    public string? TenNhomNangLuc { get; init; }

    public decimal? ChieuDaiCm { get; init; }
    public decimal? ChieuRongCm { get; init; }
    public decimal? ChieuCaoCm { get; init; }
    public decimal? TrongLuong { get; init; }
    public decimal? DienTich { get; init; }
    public decimal? DoKho { get; init; }
    public decimal? HeSoTiTrong { get; init; }
    public decimal? CbmMacDinh { get; init; }

    public Guid? KhoMacDinhId { get; init; }
    public string? MaKhoMacDinh { get; init; }
    public string? TenKhoMacDinh { get; init; }

    public Guid? KhoTonId { get; init; }
    public string? MaKhoTon { get; init; }
    public string? TenKhoTon { get; init; }

    public Guid? XuongMacDinhId { get; init; }
    public string? MaXuongMacDinh { get; init; }
    public string? TenXuongMacDinh { get; init; }

    public Guid? ThueId { get; init; }
    public string? MaThue { get; init; }
    public string? TenThue { get; init; }
    public decimal? ThueSuat { get; init; }

    public bool LaBanThanhPham { get; init; }

    public string? NoiGiaoHang { get; init; }

    public string? GhiChu { get; init; }
    public byte TrangThai { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedByMsnv { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedByMsnv { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
