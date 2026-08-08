using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Products.SanPham.Dtos;

public sealed class TaoSanPhamRequest
{
    /// <summary>
    /// ID sản phẩm từ Trading khi đồng bộ. Bỏ trống khi tạo sản phẩm trực tiếp tại EMAN.
    /// </summary>
    public Guid? Id { get; init; }

    [Required(ErrorMessage = "Mã sản phẩm là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Mã sản phẩm không được vượt quá 100 ký tự.")]
    public string MaSanPham { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mô tả tiếng Việt là bắt buộc.")]
    [MaxLength(500, ErrorMessage = "Mô tả tiếng Việt không được vượt quá 500 ký tự.")]
    public string MoTaTiengViet { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Mô tả tiếng Anh không được vượt quá 500 ký tự.")]
    public string? MoTaTiengAnh { get; init; }

    public Guid DonViTinhId { get; init; }

    public Guid? NhomNangLucId { get; init; }

    [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Chiều dài phải lớn hơn hoặc bằng 0.")]
    public decimal? ChieuDaiCm { get; init; }

    [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Chiều rộng phải lớn hơn hoặc bằng 0.")]
    public decimal? ChieuRongCm { get; init; }

    [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Chiều cao phải lớn hơn hoặc bằng 0.")]
    public decimal? ChieuCaoCm { get; init; }

    [Range(typeof(decimal), "0", "999999999999999.999", ErrorMessage = "Trọng lượng phải lớn hơn hoặc bằng 0.")]
    public decimal? TrongLuong { get; init; }

    [Range(typeof(decimal), "0", "99999999999999.9999", ErrorMessage = "Diện tích phải lớn hơn hoặc bằng 0.")]
    public decimal? DienTich { get; init; }

    [Range(typeof(decimal), "0", "99999999999999.9999", ErrorMessage = "Độ khó phải lớn hơn hoặc bằng 0.")]
    public decimal? DoKho { get; init; }

    [Range(typeof(decimal), "0", "999999999999.999999", ErrorMessage = "Hệ số tỉ trọng phải lớn hơn hoặc bằng 0.")]
    public decimal? HeSoTiTrong { get; init; }

    [Range(typeof(decimal), "0", "999999999999.999999", ErrorMessage = "CBM mặc định phải lớn hơn hoặc bằng 0.")]
    public decimal? CbmMacDinh { get; init; }

    public Guid? KhoMacDinhId { get; init; }

    public Guid? KhoTonId { get; init; }

    public Guid? XuongMacDinhId { get; init; }

    public Guid? ThueId { get; init; }

    public bool LaBanThanhPham { get; init; }

    [MaxLength(500, ErrorMessage = "Nơi giao hàng không được vượt quá 500 ký tự.")]
    public string? NoiGiaoHang { get; init; }

    [MaxLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1.000 ký tự.")]
    public string? GhiChu { get; init; }

    [MaxLength(50, ErrorMessage = "Mã nhân viên người tạo không được vượt quá 50 ký tự.")]
    public string? CreatedByMsnv { get; init; }
}
