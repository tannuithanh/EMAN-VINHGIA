using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Dtos;

public sealed class TaoDoiTacKinhDoanhRequest
{
    [Required(ErrorMessage = "Mã đối tác là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã đối tác không được vượt quá 50 ký tự.")]
    public string MaDoiTac { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên đối tác là bắt buộc.")]
    [MaxLength(250, ErrorMessage = "Tên đối tác không được vượt quá 250 ký tự.")]
    public string TenDoiTac { get; init; } = string.Empty;

    public Guid LoaiDoiTacId { get; init; }

    public bool LaNhaCungCap { get; init; }

    [MaxLength(50, ErrorMessage = "Mã số thuế không được vượt quá 50 ký tự.")]
    public string? MaSoThue { get; init; }

    [MaxLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự.")]
    public string? DiaChi { get; init; }

    [MaxLength(200, ErrorMessage = "Người liên hệ không được vượt quá 200 ký tự.")]
    public string? NguoiLienHe { get; init; }

    [MaxLength(50, ErrorMessage = "Điện thoại không được vượt quá 50 ký tự.")]
    public string? DienThoai { get; init; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [MaxLength(200, ErrorMessage = "Email không được vượt quá 200 ký tự.")]
    public string? Email { get; init; }

    [MaxLength(100, ErrorMessage = "Số tài khoản không được vượt quá 100 ký tự.")]
    public string? SoTaiKhoan { get; init; }

    [MaxLength(250, ErrorMessage = "Tên ngân hàng không được vượt quá 250 ký tự.")]
    public string? TenNganHang { get; init; }

    public Guid DieuKienThanhToanId { get; init; }

    public Guid DieuKienGiaoHangId { get; init; }
}

