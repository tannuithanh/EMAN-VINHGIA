namespace Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Dtos;

public sealed class NhomMDto
{
    public long Id { get; init; }
    public string PhamViBom { get; init; } = string.Empty;
    public string MaNhomM { get; init; } = string.Empty;
    public string TenNhomM { get; init; } = string.Empty;
    public int ThuTu { get; init; }
    public string? MoTa { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
