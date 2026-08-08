namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;

public sealed class BomMauDinhMucNhomMDto
{
    public long Id { get; init; }
    public long BuocNhomMauId { get; init; }
    public long NhomMId { get; init; }
    public decimal DinhMuc { get; init; }
    public string? GhiChu { get; init; }
    public string MaNhomM { get; init; } = string.Empty;
    public string TenNhomM { get; init; } = string.Empty;
    public string TenBuoc { get; init; } = string.Empty;
    public string MaHonHop { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
