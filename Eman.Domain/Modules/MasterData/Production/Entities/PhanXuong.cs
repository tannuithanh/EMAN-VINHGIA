using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.Production.Entities;

public sealed class PhanXuong : BaseEntity
{
    public string MaPhanXuong { get; set; } = string.Empty;

    public string TenPhanXuong { get; set; } = string.Empty;

    public string? MoTa { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;
}
