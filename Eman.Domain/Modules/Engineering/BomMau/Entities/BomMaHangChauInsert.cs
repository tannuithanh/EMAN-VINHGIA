using Eman.Domain.Modules.Engineering.Bom.Common;
using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

namespace Eman.Domain.Modules.Engineering.Bom.Mau.Entities;

public sealed class BomMaHangChauInsert : BomGuidAuditEntity
{
    public long MaHangId { get; set; }
    public string MaHang { get; set; } = string.Empty;
    public Guid ChauInsertId { get; set; }
    public string MaChauInsert { get; set; } = string.Empty;
    public int SoLuong { get; set; } = 1;
    public string? GhiChu { get; set; }
    public MaHang MaHangNavigation { get; set; } = null!;
    public ChauInsert ChauInsert { get; set; } = null!;
}
