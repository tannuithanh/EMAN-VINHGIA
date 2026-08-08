using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.BusinessPartners.Entities;
using Eman.Domain.Modules.MasterData.Common.Entities;
using Eman.Domain.Modules.MasterData.Inventory.Entities;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using Eman.Domain.Modules.MasterData.Products.Entities;

namespace Eman.Domain.Modules.MasterData.Materials.Entities;

/// <summary>
/// Danh mục vật tư dùng chung cho toàn bộ cấp vật tư. Cấp vật tư không được lưu tại đây.
/// </summary>
public sealed class VatTu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MaVatTu { get; set; } = string.Empty;
    public string TenVatTu { get; set; } = string.Empty;
    public string? TenTiengAnh { get; set; }
    public Guid DonViTinhId { get; set; }
    public string? QuyCachDongGoi { get; set; }
    public PhamViSuDungVatTu? PhamViSuDung { get; set; }
    public Guid NhomVatTuId { get; set; }
    public string? MucDichSuDung { get; set; }
    public PhuongThucCungUngVatTu PhuongThucCungUng { get; set; }
    public Guid? CoSoMuaVatTuId { get; set; }
    public Guid? NhaCungCapMacDinhId { get; set; }
    public int? NgayMuaHang { get; set; }
    public int HanSuDungNgay { get; set; }
    public decimal? Moq { get; set; }
    public Guid? ThueVatId { get; set; }
    public decimal? TonToiThieu { get; set; }
    public Guid? KhoLuuTruId { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByMsnv { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByMsnv { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DonViTinh DonViTinh { get; set; } = null!;
    public NhomVatTu NhomVatTu { get; set; } = null!;
    public CoSoMuaVatTu? CoSoMuaVatTu { get; set; }
    public DoiTacKinhDoanh? NhaCungCapMacDinh { get; set; }
    public ThueSanPham? ThueVat { get; set; }
    public Kho? KhoLuuTru { get; set; }
    public ICollection<VatTuPhanXuong> PhanXuongs { get; set; } = [];
}
