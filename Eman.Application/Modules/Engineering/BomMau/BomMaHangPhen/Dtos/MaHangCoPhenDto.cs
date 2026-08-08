namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;

public sealed class MaHangCoPhenDto
{
    public Guid CauHinhPhenId { get; init; }
    public long MaHangId { get; init; }
    public string MaHang { get; init; } = string.Empty;
    public string MaHangPhen { get; init; } = string.Empty;
    public string? GhiChu { get; init; }
    public bool IsActive { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
