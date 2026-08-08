using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

public sealed class HinhDang : BomAuditEntity
{
    public string MaHinhDang { get; set; } = string.Empty;
    public string TenHinhDang { get; set; } = string.Empty;
    public string? MoTa { get; set; }
}
