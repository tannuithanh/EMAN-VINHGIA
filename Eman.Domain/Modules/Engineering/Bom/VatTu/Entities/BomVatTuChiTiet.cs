using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;

/// <summary>
/// Một vật tư thành phần trực tiếp trong một phiên bản B.O.M vật tư.
/// </summary>
public sealed class BomVatTuChiTiet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BomVatTuPhienBanId { get; set; }
    public Guid VatTuThanhPhanId { get; set; }
    public decimal SoLuong { get; set; }
    public int ThuTu { get; set; } = 1;
    public string? GhiChu { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByMsnv { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByMsnv { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public BomVatTuPhienBan BomVatTuPhienBan { get; set; } = null!;
    public VatTuEntity VatTuThanhPhan { get; set; } = null!;
}
