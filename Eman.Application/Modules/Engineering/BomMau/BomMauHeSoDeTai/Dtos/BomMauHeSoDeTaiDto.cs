namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Dtos;

public sealed class BomMauHeSoDeTaiDto
{
    public long Id { get; init; }
    public long HeSanPhamId { get; init; }
    public long DeTaiId { get; init; }
    public long BuocId { get; init; }
    public decimal HeSo { get; init; }
    public string? GhiChu { get; init; }
    public string MaHe { get; init; } = string.Empty;
    public string MaDeTai { get; init; } = string.Empty;
    public string TenDeTai { get; init; } = string.Empty;
    public string MaBuoc { get; init; } = string.Empty;
    public string TenBuoc { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
