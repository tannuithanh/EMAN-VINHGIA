namespace Eman.Domain.Modules.Engineering.Bom.Common;

/// <summary>
/// Thông tin dùng chung cho các bảng B.O.M có theo dõi thời gian và đồng thời dữ liệu.
/// </summary>
public abstract class BomAuditEntity
{
    public long Id { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
