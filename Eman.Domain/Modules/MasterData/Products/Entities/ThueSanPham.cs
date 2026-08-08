using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.Products.Entities;

public sealed class ThueSanPham : BaseEntity
{
    public string MaThue { get; set; } = string.Empty;

    public string TenThue { get; set; } = string.Empty;

    public decimal ThueSuat { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;
}
