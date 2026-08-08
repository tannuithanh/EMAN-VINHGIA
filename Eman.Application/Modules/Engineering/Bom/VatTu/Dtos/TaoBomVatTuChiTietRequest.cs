using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;

public sealed class TaoBomVatTuChiTietRequest
{
    public Guid VatTuThanhPhanId { get; init; }

    [Range(typeof(decimal), "0.000001", "999999999999.999999", ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public decimal SoLuong { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int ThuTu { get; init; } = 1;

    [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    public string? GhiChu { get; init; }

    [MaxLength(50, ErrorMessage = "Mã nhân viên tạo không được vượt quá 50 ký tự.")]
    public string? CreatedByMsnv { get; init; }
}
