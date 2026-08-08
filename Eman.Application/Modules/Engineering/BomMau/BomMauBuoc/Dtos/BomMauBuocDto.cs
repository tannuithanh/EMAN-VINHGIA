namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Dtos;

public sealed class BomMauBuocDto
{
    public long Id { get; init; }
    public string MaBuoc { get; init; } = string.Empty;
    public string TenBuoc { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
