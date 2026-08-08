namespace Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Dtos;

public sealed class DeTaiDto
{
    public long Id { get; init; }
    public long HeSanPhamId { get; init; }
    public string MaDeTai { get; init; } = string.Empty;
    public string TenDeTai { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public string MaHe { get; init; } = string.Empty;
    public string TenHe { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
