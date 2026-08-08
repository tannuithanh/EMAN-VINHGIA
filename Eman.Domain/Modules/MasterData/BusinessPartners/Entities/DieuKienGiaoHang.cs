
using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.BusinessPartners.Entities;

public sealed class DieuKienGiaoHang : BaseEntity
{
    public string MaDieuKienGiaoHang { get; set; } = string.Empty;

    public string TenDieuKienGiaoHang { get; set; } = string.Empty;

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;

    public ICollection<DoiTacKinhDoanh> DoiTacKinhDoanhs { get; set; } = [];
}
