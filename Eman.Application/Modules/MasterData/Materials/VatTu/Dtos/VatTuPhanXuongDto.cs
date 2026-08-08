namespace Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;

public sealed record VatTuPhanXuongDto
{
    public Guid PhanXuongId { get; init; }
    public string MaPhanXuong { get; init; } = string.Empty;
    public string TenPhanXuong { get; init; } = string.Empty;
}
