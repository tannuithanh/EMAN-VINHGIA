namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;

public sealed class MaHangCoChauInsertDto
{
    public long MaHangId { get; init; }
    public string MaHang { get; init; } = string.Empty;
    public int SoLoaiChauInsert { get; init; }
    public int TongSoLuongChauInsert { get; init; }
    public IReadOnlyList<ChiTietChauInsertTheoMaHangDto> DanhSachChauInsert { get; init; } = [];
}

public sealed class ChiTietChauInsertTheoMaHangDto
{
    public Guid CauHinhChauInsertId { get; init; }
    public Guid ChauInsertId { get; init; }
    public string MaChauInsert { get; init; } = string.Empty;
    public string? TenChauInsert { get; init; }
    public int SoLuong { get; init; }
    public string? GhiChu { get; init; }
    public bool IsActive { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
