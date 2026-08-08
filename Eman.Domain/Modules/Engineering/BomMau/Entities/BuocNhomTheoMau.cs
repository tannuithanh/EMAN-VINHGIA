using Eman.Domain.Modules.Engineering.Bom.Common;
using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;

namespace Eman.Domain.Modules.Engineering.Bom.Mau.Entities;

/// <summary>
/// Cấu hình bước và mã hỗn hợp theo hệ sản phẩm, màu sắc.
/// Tên thuộc tính bám theo nghiệp vụ, còn tên cột được ánh xạ tại Infrastructure.
/// </summary>
public sealed class BuocNhomTheoMau : BomAuditEntity
{
    public long HeSanPhamId { get; set; }

    /// <summary>
    /// Ánh xạ tới cột mau_sac trong database.
    /// </summary>
    public long MauSacId { get; set; }

    public string MaBuoc { get; set; } = string.Empty;
    public string TenBuoc { get; set; } = string.Empty;
    public long MaHonHopId { get; set; }
    public string MaHonHop { get; set; } = string.Empty;
    public string? GhiChu { get; set; }

    public HeSanPham HeSanPham { get; set; } = null!;
    public MauSac MauSac { get; set; } = null!;
}
