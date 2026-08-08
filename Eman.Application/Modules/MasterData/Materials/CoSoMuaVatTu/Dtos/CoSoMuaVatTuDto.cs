namespace Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Dtos;

public sealed record CoSoMuaVatTuDto
{
    public Guid Id { get; init; }
    public string MaCoSoMuaVatTu { get; init; } = string.Empty;
    public string TenCoSoMuaVatTu { get; init; } = string.Empty;
    public string? MoTa { get; init; }
    public byte TrangThai { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedByMsnv { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedByMsnv { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
