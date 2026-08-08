namespace Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Dtos;

public sealed class MauSacDto
{
    public long Id { get; init; }
    public long DeTaiId { get; init; }
    public string MaMau { get; init; } = string.Empty;
    public string TenMau { get; init; } = string.Empty;
    public string? MaCotTho { get; init; }
    public string? MoTa { get; init; }
    public string MaDeTai { get; init; } = string.Empty;
    public string TenDeTai { get; init; } = string.Empty;
    public long HeSanPhamId { get; init; }
    public string MaHe { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
