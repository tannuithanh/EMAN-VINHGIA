using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.Inventory.Entities;

public sealed class Kho : BaseEntity
{
    public string MaKho { get; set; } = string.Empty;

    public string TenKho { get; set; } = string.Empty;

    public bool HangTon { get; set; }

    public bool HangTru { get; set; }

    public string? MoTa { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;
}
