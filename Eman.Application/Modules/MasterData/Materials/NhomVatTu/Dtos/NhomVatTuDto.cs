namespace Eman.Application.Modules.MasterData.Materials.NhomVatTu.Dtos;

public sealed record NhomVatTuDto
{
    public Guid Id { get; init; }
    public string MaNhomVatTu { get; init; } = string.Empty;
    public string TenNhomVatTu { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public byte TrangThai { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedByMsnv { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedByMsnv { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
