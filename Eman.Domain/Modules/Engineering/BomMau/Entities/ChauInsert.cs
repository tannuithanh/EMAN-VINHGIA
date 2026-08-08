using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;
using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.Mau.Entities;

public sealed class ChauInsert : BomGuidAuditEntity
{
    public string MaChauInsert { get; set; } = string.Empty;
    public string? TenChauInsert { get; set; }
    public string? MoTa { get; set; }
}
