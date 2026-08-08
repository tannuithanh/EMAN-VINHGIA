using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;

public sealed class TaoBuocNhomTheoMauRequest
{
    /// <summary>
    /// Có thể bỏ trống để backend lấy từ màu sắc đã chọn.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = "Hệ sản phẩm không hợp lệ.")]
    public long? HeSanPhamId { get; init; }

    /// <summary>
    /// Giữ tương thích với giao diện cũ và dùng để kiểm tra quan hệ màu - đề tài.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = "Đề tài không hợp lệ.")]
    public long? DeTaiId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Màu sắc không hợp lệ.")]
    public long MauSacId { get; init; }

    /// <summary>
    /// Có thể bỏ trống trong thời gian chuyển tiếp; backend sẽ dùng tên bước làm mã bước.
    /// </summary>
    [MaxLength(300, ErrorMessage = "Mã bước không được vượt quá 300 ký tự.")]
    public string? MaBuoc { get; init; }

    [Required(ErrorMessage = "Tên bước là bắt buộc.")]
    [MaxLength(300, ErrorMessage = "Tên bước không được vượt quá 300 ký tự.")]
    public string TenBuoc { get; init; } = string.Empty;

    [Range(1, long.MaxValue, ErrorMessage = "Mã hỗn hợp không hợp lệ.")]
    public long MaHonHopId { get; init; }

    [Required(ErrorMessage = "Mã hỗn hợp là bắt buộc.")]
    [MaxLength(100, ErrorMessage = "Mã hỗn hợp không được vượt quá 100 ký tự.")]
    public string MaHonHop { get; init; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }
}
