using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

public sealed class DeTai : BomAuditEntity
{
    public long HeSanPhamId { get; set; }
    public string MaDeTai { get; set; } = string.Empty;
    public string TenDeTai { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public HeSanPham HeSanPham { get; set; } = null!;
    public ICollection<MauSac> MauSacs { get; set; } = [];
}
