using Eman.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.Common;

internal static class EntityTypeBuilderExtensions
{
    public static void CauHinhBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2(0)")
            .IsRequired();

        builder.Property(entity => entity.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(entity => entity.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("datetime2(0)");

        builder.Property(entity => entity.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(entity => entity.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}
