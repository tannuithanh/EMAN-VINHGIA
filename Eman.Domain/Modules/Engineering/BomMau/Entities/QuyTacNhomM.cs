using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

public sealed class QuyTacNhomM : BomAuditEntity
{
    public long HinhDangId { get; set; }
    public decimal DienTichTu { get; set; }
    public decimal? DienTichDen { get; set; }
    public bool BaoGomTu { get; set; }
    public bool BaoGomDen { get; set; }
    public long NhomMId { get; set; }
    public string? GhiChu { get; set; }
    public HinhDang HinhDang { get; set; } = null!;
    public NhomM NhomM { get; set; } = null!;
}
