namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;

public sealed class BomMaHangChauInsertDto
{
    public Guid Id { get; init; }
    public long MaHangId { get; init; }
    public Guid ChauInsertId { get; init; }
    public int SoLuong { get; init; }
    public string? GhiChu { get; init; }
    public string MaHang { get; init; } = string.Empty;
    public string MaChauInsert { get; init; } = string.Empty;
    public string? TenChauInsert { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
