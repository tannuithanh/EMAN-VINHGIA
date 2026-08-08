using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

public sealed class NhomM : BomAuditEntity
{
    public string PhamViBom { get; set; } = string.Empty;
    public string MaNhomM { get; set; } = string.Empty;
    public string TenNhomM { get; set; } = string.Empty;
    public int ThuTu { get; set; }
    public string? MoTa { get; set; }
}
