using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;
using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.Mau.Entities;

public sealed class BomMauDinhMucNhomM : BomAuditEntity
{
    public long BuocNhomMauId { get; set; }
    public long NhomMId { get; set; }
    public string MaNhomM { get; set; } = string.Empty;
    public decimal DinhMuc { get; set; }
    public string? GhiChu { get; set; }
    public BuocNhomTheoMau BuocNhomMau { get; set; } = null!;
    public NhomM NhomM { get; set; } = null!;
}
