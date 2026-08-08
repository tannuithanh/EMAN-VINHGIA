using Eman.Domain.Modules.Engineering.BomMau.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.BomMau.Common;

internal static class BomMauConfigurationExtensions
{
    public static void CauHinhAudit<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BomMauAuditEntity
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(entity => entity.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(0)").IsRequired();
        builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(0)");
        builder.Property(entity => entity.RowVersion).HasColumnName("row_version").IsRowVersion().IsConcurrencyToken();
    }
}
