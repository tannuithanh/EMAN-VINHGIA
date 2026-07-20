using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Dtos;

public sealed class TaoLoaiDoiTacRequest
{
    [Required(ErrorMessage = "Mã loại đối tác là bắt buộc.")]
    [MaxLength(50, ErrorMessage = "Mã loại đối tác không được vượt quá 50 ký tự.")]
    public string MaLoaiDoiTac { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên loại đối tác là bắt buộc.")]
    [MaxLength(200, ErrorMessage = "Tên loại đối tác không được vượt quá 200 ký tự.")]
    public string TenLoaiDoiTac { get; init; } = string.Empty;
}
