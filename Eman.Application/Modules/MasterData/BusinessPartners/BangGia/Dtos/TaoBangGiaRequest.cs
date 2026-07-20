using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Dtos;

public sealed class TaoBangGiaRequest
{
    [Required(ErrorMessage = "Mã bảng giá là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã bảng giá không được vượt quá 50 ký tự.")]
    public string MaBangGia { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên bảng giá là bắt buộc.")]
    [MaxLength(250, ErrorMessage = "Tên bảng giá không được vượt quá 250 ký tự.")]
    public string TenBangGia { get; init; } = string.Empty;

    public Guid DoiTacKinhDoanhId { get; init; }
}
