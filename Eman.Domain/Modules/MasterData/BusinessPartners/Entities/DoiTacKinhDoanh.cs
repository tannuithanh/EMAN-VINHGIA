using Eman.Domain.Common;
using Eman.Domain.Common.Enums;

namespace Eman.Domain.Modules.MasterData.BusinessPartners.Entities;

public sealed class DoiTacKinhDoanh : BaseEntity
{
    public string MaDoiTac { get; set; } = string.Empty;

    public string TenDoiTac { get; set; } = string.Empty;

    public Guid LoaiDoiTacId { get; set; }

    public bool LaNhaCungCap { get; set; }

    public string? MaSoThue { get; set; }

    public string? DiaChi { get; set; }

    public string? NguoiLienHe { get; set; }

    public string? DienThoai { get; set; }

    public string? Email { get; set; }

    public string? SoTaiKhoan { get; set; }

    public string? TenNganHang { get; set; }

    public Guid? DieuKienThanhToanId { get; set; }

    public Guid? DieuKienGiaoHangId { get; set; }

    public TrangThaiHoatDong TrangThai { get; set; } = TrangThaiHoatDong.HoatDong;

    public LoaiDoiTac LoaiDoiTac { get; set; } = null!;

    public DieuKienThanhToan? DieuKienThanhToan { get; set; }

    public DieuKienGiaoHang? DieuKienGiaoHang { get; set; }

    public ICollection<BangGia> BangGias { get; set; } = [];
}
