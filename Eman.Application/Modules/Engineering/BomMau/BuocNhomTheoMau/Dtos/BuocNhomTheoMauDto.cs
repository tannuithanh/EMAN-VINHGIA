namespace Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;

public sealed class BuocNhomTheoMauDto
{
    public long Id { get; init; }

    public long HeSanPhamId { get; init; }
    public string MaHe { get; init; } = string.Empty;
    public string TenHe { get; init; } = string.Empty;

    public long DeTaiId { get; init; }
    public string MaDeTai { get; init; } = string.Empty;
    public string TenDeTai { get; init; } = string.Empty;

    public long MauSacId { get; init; }
    public string MaMau { get; init; } = string.Empty;
    public string TenMau { get; init; } = string.Empty;

    public string MaBuoc { get; init; } = string.Empty;
    public string TenBuoc { get; init; } = string.Empty;

    public long MaHonHopId { get; init; }
    public string MaHonHop { get; init; } = string.Empty;
    public string? GhiChu { get; init; }

    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
