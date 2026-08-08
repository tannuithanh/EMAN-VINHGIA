
using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.BusinessPartners.Entities;

public sealed class DieuKienThanhToan : BaseEntity
{
    public string MaDieuKienThanhToan { get; set; } = string.Empty;

    public string TenDieuKienThanhToan { get; set; } = string.Empty;

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;

    public ICollection<DoiTacKinhDoanh> DoiTacKinhDoanhs { get; set; } = [];
}
