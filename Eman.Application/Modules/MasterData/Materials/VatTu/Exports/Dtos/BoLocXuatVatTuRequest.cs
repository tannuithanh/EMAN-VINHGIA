using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Dtos;

/// <summary>
/// Bộ lọc xuất Excel vật tư. Không áp dụng phân trang; file xuất chứa toàn bộ dữ liệu khớp bộ lọc.
/// </summary>
public sealed class BoLocXuatVatTuRequest
{
    public string? Keyword { get; init; }
    public Guid? DonViTinhId { get; init; }
    public Guid? NhomVatTuId { get; init; }
    public Guid? CoSoMuaVatTuId { get; init; }
    public Guid? NhaCungCapMacDinhId { get; init; }
    public Guid? ThueVatId { get; init; }
    public Guid? KhoLuuTruId { get; init; }
    public Guid? PhanXuongId { get; init; }

    [Range(1, 2, ErrorMessage = "Phạm vi sử dụng chỉ nhận 1 hoặc 2.")]
    public byte? PhamViSuDung { get; init; }

    [Range(1, 3, ErrorMessage = "Phương thức cung ứng chỉ nhận từ 1 đến 3.")]
    public byte? PhuongThucCungUng { get; init; }

    [Range(0, 1, ErrorMessage = "Trạng thái chỉ nhận 0 hoặc 1.")]
    public byte? TrangThai { get; init; }
}
