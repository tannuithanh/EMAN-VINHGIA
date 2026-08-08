using Eman.Domain.Modules.MasterData.Production.Entities;

namespace Eman.Domain.Modules.MasterData.Materials.Entities;

public sealed class VatTuPhanXuong
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VatTuId { get; set; }
    public Guid PhanXuongId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByMsnv { get; set; }

    public VatTu VatTu { get; set; } = null!;
    public PhanXuong PhanXuong { get; set; } = null!;
}
