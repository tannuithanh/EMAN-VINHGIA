using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.Materials.Entities;

public sealed class CoSoMuaVatTu
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MaCoSoMuaVatTu { get; set; } = string.Empty;
    public string TenCoSoMuaVatTu { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByMsnv { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByMsnv { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public ICollection<VatTu> VatTus { get; set; } = [];
}
