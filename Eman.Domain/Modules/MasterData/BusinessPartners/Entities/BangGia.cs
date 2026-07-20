using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.BusinessPartners.Entities;

public sealed class BangGia : BaseEntity
{
    public string MaBangGia { get; set; } = string.Empty;

    public string TenBangGia { get; set; } = string.Empty;

    public Guid DoiTacKinhDoanhId { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;

    public DoiTacKinhDoanh DoiTacKinhDoanh { get; set; } = null!;

    public ICollection<PhienBanBangGia> PhienBanBangGias { get; set; } = [];
}
