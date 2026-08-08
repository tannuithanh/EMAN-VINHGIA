using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.Common.Entities;
using Eman.Domain.Modules.MasterData.Inventory.Entities;
using Eman.Domain.Modules.MasterData.Production.Entities;

namespace Eman.Domain.Modules.MasterData.Products.Entities;

/// <summary>
/// Sản phẩm được quản lý tại EMAN.
/// ID có thể được nhận trực tiếp từ Trading khi thực hiện đồng bộ sản phẩm.
/// </summary>
public sealed class SanPham
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string MaSanPham { get; set; } = string.Empty;

    public string MoTaTiengViet { get; set; } = string.Empty;

    public string? MoTaTiengAnh { get; set; }

    public Guid DonViTinhId { get; set; }

    public Guid? NhomNangLucId { get; set; }

    public decimal? ChieuDaiCm { get; set; }

    public decimal? ChieuRongCm { get; set; }

    public decimal? ChieuCaoCm { get; set; }

    public decimal? TrongLuong { get; set; }

    public decimal? DienTich { get; set; }

    public decimal? DoKho { get; set; }

    public decimal? HeSoTiTrong { get; set; }

    public decimal? CbmMacDinh { get; set; }

    public Guid? KhoMacDinhId { get; set; }

    public Guid? KhoTonId { get; set; }

    public Guid? XuongMacDinhId { get; set; }

    public Guid? ThueId { get; set; }

    public bool LaBanThanhPham { get; set; }

    public string? NoiGiaoHang { get; set; }

    public string? GhiChu { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? CreatedByMsnv { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedByMsnv { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DonViTinh DonViTinh { get; set; } = null!;

    public NhomNangLuc? NhomNangLuc { get; set; }

    public Kho? KhoMacDinh { get; set; }

    public Kho? KhoTon { get; set; }

    public PhanXuong? XuongMacDinh { get; set; }

    public ThueSanPham? Thue { get; set; }

}
