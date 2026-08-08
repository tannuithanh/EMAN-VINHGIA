using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;
using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.Mau.Entities;

public sealed class BomMauHeSoDeTai : BomAuditEntity
{
    public long HeSanPhamId { get; set; }
    public string MaHe { get; set; } = string.Empty;
    public long DeTaiId { get; set; }
    public string MaDeTai { get; set; } = string.Empty;
    public string TenDeTai { get; set; } = string.Empty;
    public long BuocId { get; set; }
    public string TenBuoc { get; set; } = string.Empty;
    public decimal HeSo { get; set; }
    public string? GhiChu { get; set; }
    public HeSanPham HeSanPham { get; set; } = null!;
    public DeTai DeTai { get; set; } = null!;
    public BomMauBuoc Buoc { get; set; } = null!;
}
