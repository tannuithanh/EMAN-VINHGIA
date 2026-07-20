using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.BusinessPartners.Entities;

public sealed class LoaiDoiTac : BaseEntity
{
    public string MaLoaiDoiTac { get; set; } = string.Empty;

    public string TenLoaiDoiTac { get; set; } = string.Empty;

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;

    public ICollection<DoiTacKinhDoanh> DoiTacKinhDoanhs { get; set; } = [];
}
