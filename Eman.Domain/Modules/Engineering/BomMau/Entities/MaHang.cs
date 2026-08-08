using Eman.Domain.Modules.Engineering.Bom.Common;

namespace Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

public sealed class MaHang : BomAuditEntity
{
    public string MaHangCode { get; set; } = string.Empty;

    /// <summary>
    /// Database cho phép NULL để giữ được dữ liệu mã hàng chưa có diện tích.
    /// </summary>
    public decimal? DienTich { get; set; }

    public long? HinhDangBomThoId { get; set; }
    public long? HinhDangBomMauId { get; set; }
    public string? MoTa { get; set; }
    public string LoaiMaHang { get; set; } = "SAN_PHAM";

    public HinhDang? HinhDangBomTho { get; set; }
    public HinhDang? HinhDangBomMau { get; set; }
}
