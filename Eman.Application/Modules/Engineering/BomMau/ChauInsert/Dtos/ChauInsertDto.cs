namespace Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Dtos;

public sealed class ChauInsertDto
{
    public Guid Id { get; init; }
    public string MaChauInsert { get; init; } = string.Empty;
    public string? TenChauInsert { get; init; }
    public string? MoTa { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
