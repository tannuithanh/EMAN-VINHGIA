using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;

public sealed class TaoBomMaHangChauInsertRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Mã hàng không hợp lệ.")]
    public long MaHangId { get; init; }

    public Guid ChauInsertId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng chậu insert phải lớn hơn hoặc bằng 1.")]
    public int SoLuong { get; init; } = 1;

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }
}
