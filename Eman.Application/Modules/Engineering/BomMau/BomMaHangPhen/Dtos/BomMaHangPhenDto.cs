namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;

public sealed class BomMaHangPhenDto
{
    public Guid Id { get; init; }
    public long MaHangId { get; init; }
    public string MaHangPhen { get; init; } = string.Empty;
    public string? GhiChu { get; init; }
    public string MaHang { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
