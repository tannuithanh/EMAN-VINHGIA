namespace Eman.Domain.Modules.Engineering.Bom.Common;

/// <summary>
/// Thông tin dùng chung cho các bảng B.O.M sử dụng khóa chính GUID.
/// </summary>
public abstract class BomGuidAuditEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
