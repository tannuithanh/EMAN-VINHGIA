using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;

public sealed class CapNhatBomMauDinhMucNhomMRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Bước nhóm theo màu không hợp lệ.")]
    public long BuocNhomMauId { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Nhóm M không hợp lệ.")]
    public long NhomMId { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Định mức không được nhỏ hơn 0.")]
    public decimal DinhMuc { get; init; }

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }

    public bool IsActive { get; init; }

    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
