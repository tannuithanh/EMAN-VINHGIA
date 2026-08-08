namespace Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Dtos;

public sealed class HeSanPhamDto
{
    public long Id { get; init; }
    public string MaHe { get; init; } = string.Empty;
    public string TenHe { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
