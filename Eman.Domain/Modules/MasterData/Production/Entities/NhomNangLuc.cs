using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.Production.Entities;

public sealed class NhomNangLuc : BaseEntity
{
    public string MaNhomNangLuc { get; set; } = string.Empty;

    public string TenNhomNangLuc { get; set; } = string.Empty;

    public int? ThoiGianLamHang { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;
}
