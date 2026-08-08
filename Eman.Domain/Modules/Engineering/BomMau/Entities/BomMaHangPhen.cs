using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;
using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.Mau.Entities;

public sealed class BomMaHangPhen : BomGuidAuditEntity
{
    public long MaHangId { get; set; }
    public string MaHang { get; set; } = string.Empty;
    public string MaHangPhen { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
    public MaHang MaHangNavigation { get; set; } = null!;
}
