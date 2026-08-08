namespace Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Dtos;

public sealed class MaHangDto
{
    public long Id { get; init; }
    public string MaHang { get; init; } = string.Empty;
    public decimal? DienTich { get; init; }

    public long? HinhDangBomThoId { get; init; }
    public string? MaHinhDangBomTho { get; init; }
    public string? TenHinhDangBomTho { get; init; }

    public long? HinhDangBomMauId { get; init; }
    public string? MaHinhDangBomMau { get; init; }
    public string? TenHinhDangBomMau { get; init; }

    public string? MoTa { get; init; }
    public string LoaiMaHang { get; init; } = "SAN_PHAM";
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
