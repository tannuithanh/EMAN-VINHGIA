using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Dtos;

public sealed class TaoBomMauBuocRequest
{
    [Required(ErrorMessage = "Mã bước là bắt buộc.")]
    [MaxLength(30, ErrorMessage = "Mã bước không được vượt quá 30 ký tự.")]
    public string MaBuoc { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên bước là bắt buộc.")]
    [MaxLength(300, ErrorMessage = "Tên bước không được vượt quá 300 ký tự.")]
    public string TenBuoc { get; init; } = string.Empty;
}
