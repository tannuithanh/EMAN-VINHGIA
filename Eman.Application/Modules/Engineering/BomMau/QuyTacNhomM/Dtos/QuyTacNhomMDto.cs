namespace Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Dtos;

public sealed class QuyTacNhomMDto
{
    public long Id { get; init; }
    public long HinhDangId { get; init; }
    public decimal DienTichTu { get; init; }
    public decimal? DienTichDen { get; init; }
    public bool BaoGomTu { get; init; }
    public bool BaoGomDen { get; init; }
    public long NhomMId { get; init; }
    public string? GhiChu { get; init; }
    public string MaHinhDang { get; init; } = string.Empty;
    public string TenHinhDang { get; init; } = string.Empty;
    public string PhamViBom { get; init; } = string.Empty;
    public string MaNhomM { get; init; } = string.Empty;
    public string TenNhomM { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
