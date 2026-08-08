using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;

public sealed class TaoVatTuRequest
{

    [Required(ErrorMessage = "Mã vật tư là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Mã vật tư không được vượt quá 100 ký tự.")]
    public string MaVatTu { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên vật tư là bắt buộc.")]
    [MaxLength(300, ErrorMessage = "Tên vật tư không được vượt quá 300 ký tự.")]
    public string TenVatTu { get; init; } = string.Empty;

    [MaxLength(300, ErrorMessage = "Tên tiếng Anh không được vượt quá 300 ký tự.")]
    public string? TenTiengAnh { get; init; }

    public Guid DonViTinhId { get; init; }

    [MaxLength(500, ErrorMessage = "Quy cách đóng gói không được vượt quá 500 ký tự.")]
    public string? QuyCachDongGoi { get; init; }

    [Range(1, 2, ErrorMessage = "Phạm vi sử dụng chỉ nhận 1 hoặc 2.")]
    public byte? PhamViSuDung { get; init; }

    public IReadOnlyCollection<Guid>? PhanXuongIds { get; init; } = Array.Empty<Guid>();

    public Guid NhomVatTuId { get; init; }

    [MaxLength(1000, ErrorMessage = "Mục đích sử dụng không được vượt quá 1.000 ký tự.")]
    public string? MucDichSuDung { get; init; }

    [Range(1, 3, ErrorMessage = "Phương thức cung ứng chỉ nhận từ 1 đến 3.")]
    public byte PhuongThucCungUng { get; init; }

    public Guid? CoSoMuaVatTuId { get; init; }
    public Guid? NhaCungCapMacDinhId { get; init; }
    [Range(0, int.MaxValue, ErrorMessage = "Thời gian mua hàng phải lớn hơn hoặc bằng 0 ngày.")]
    public int? NgayMuaHang { get; init; }

    [Required(ErrorMessage = "Hạn sử dụng là bắt buộc.")]
    [Range(0, int.MaxValue, ErrorMessage = "Hạn sử dụng phải lớn hơn hoặc bằng 0 ngày.")]
    public int? HanSuDungNgay { get; init; }

    [Range(typeof(decimal), "0.001", "999999999999999.999", ErrorMessage = "MOQ phải lớn hơn 0.")]
    public decimal? Moq { get; init; }

    public Guid? ThueVatId { get; init; }

    [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Tồn tối thiểu phải lớn hơn hoặc bằng 0.")]
    public decimal? TonToiThieu { get; init; }

    public Guid? KhoLuuTruId { get; init; }

    [MaxLength(50, ErrorMessage = "Mã nhân viên người tạo không được vượt quá 50 ký tự.")]
    public string? CreatedByMsnv { get; init; }
}
