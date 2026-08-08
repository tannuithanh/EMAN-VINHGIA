using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

public sealed class MauSac : BomAuditEntity
{
    public long HeSanPhamId { get; set; }
    public long DeTaiId { get; set; }
    public string MaMau { get; set; } = string.Empty;
    public string TenMau { get; set; } = string.Empty;
    public string? MaCotTho { get; set; }
    public string? MoTa { get; set; }
    public HeSanPham HeSanPham { get; set; } = null!;
    public DeTai DeTai { get; set; } = null!;
}
