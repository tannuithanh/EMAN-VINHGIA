using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;
namespace Eman.Domain.Modules.Engineering.Bom.Mau.Entities;

public sealed class BomMauBuoc
{
    public long Id { get; set; }
    public string MaBuoc { get; set; } = string.Empty;
    public string TenBuoc { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
