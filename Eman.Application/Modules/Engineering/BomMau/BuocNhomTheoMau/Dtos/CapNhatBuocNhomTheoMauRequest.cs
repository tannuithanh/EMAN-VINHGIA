using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;

public sealed class CapNhatBuocNhomTheoMauRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Hệ sản phẩm không hợp lệ.")]
    public long? HeSanPhamId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Đề tài không hợp lệ.")]
    public long? DeTaiId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Màu sắc không hợp lệ.")]
    public long MauSacId { get; init; }

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

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
