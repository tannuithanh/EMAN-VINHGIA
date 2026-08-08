namespace Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Dtos;

public sealed class HinhDangDto
{
    public long Id { get; init; }
    public string MaHinhDang { get; init; } = string.Empty;
    public string TenHinhDang { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
