using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Dtos;

public sealed class TaoQuyTacNhomMRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Hình dáng không hợp lệ.")]
    public long HinhDangId { get; init; }

    [Required(ErrorMessage = "Diện tích bắt đầu là bắt buộc.")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Diện tích bắt đầu không được nhỏ hơn 0.")]
    public decimal? DienTichTu { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Diện tích kết thúc không được nhỏ hơn 0.")]
    public decimal? DienTichDen { get; init; }

    public bool BaoGomTu { get; init; }

    public bool BaoGomDen { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Nhóm M không hợp lệ.")]
    public long NhomMId { get; init; }

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }
}
