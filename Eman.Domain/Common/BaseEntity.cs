namespace Eman.Domain.Common;

/// <summary>
/// Thông tin dùng chung cho các thực thể có theo dõi tạo, cập nhật và đồng thời dữ liệu.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
