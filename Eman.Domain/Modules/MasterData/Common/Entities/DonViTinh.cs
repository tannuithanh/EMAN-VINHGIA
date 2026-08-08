using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.Common.Entities;

public sealed class DonViTinh : BaseEntity
{
    public string MaDonViTinh { get; set; } = string.Empty;

    public string TenDonViTinh { get; set; } = string.Empty;

    public string? KyHieu { get; set; }

    public string? MoTa { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;
}
