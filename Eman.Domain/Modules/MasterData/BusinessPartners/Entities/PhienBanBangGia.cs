using Eman.Domain.Common;
using Eman.Domain.Modules.MasterData.BusinessPartners.Enums;

namespace Eman.Domain.Modules.MasterData.BusinessPartners.Entities;

public sealed class PhienBanBangGia : BaseEntity
{
    public Guid BangGiaId { get; set; }

    public int SoPhienBan { get; set; }

    public DateOnly TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }

    public TrangThaiPhienBanBangGia TrangThai { get; set; } = TrangThaiPhienBanBangGia.SoanThao;

    public BangGia BangGia { get; set; } = null!;
}
