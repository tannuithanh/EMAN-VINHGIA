namespace Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

public sealed class HeSanPham
{
    public long Id { get; set; }
    public string MaHe { get; set; } = string.Empty;
    public string TenHe { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<DeTai> DeTais { get; set; } = [];
    public ICollection<MauSac> MauSacs { get; set; } = [];
}
