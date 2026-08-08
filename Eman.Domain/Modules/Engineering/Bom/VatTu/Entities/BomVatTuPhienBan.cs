using Eman.Domain.Modules.Engineering.Bom.VatTu.Enums;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;

/// <summary>
/// Một phiên bản công thức B.O.M của vật tư đầu ra.
/// </summary>
public sealed class BomVatTuPhienBan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VatTuId { get; set; }
    public int SoPhienBan { get; set; }
    public TrangThaiBomVatTuPhienBan TrangThai { get; set; } = TrangThaiBomVatTuPhienBan.Nhap;
    public string? GhiChu { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByMsnv { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByMsnv { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public VatTuEntity VatTu { get; set; } = null!;
    public ICollection<BomVatTuChiTiet> ChiTiets { get; set; } = [];
}
