using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.ChauInsert;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Mau.ChauInsert;

public sealed class ChauInsertConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_chau_insert", "dbo");
        builder.CauHinhAuditGuid();
        builder.Property(x => x.MaChauInsert).HasColumnName("ma_chau_insert").HasMaxLength(100).IsRequired();
        builder.Property(x => x.TenChauInsert).HasColumnName("ten_chau_insert").HasMaxLength(300);
        builder.Property(x => x.MoTa).HasColumnName("mo_ta").HasMaxLength(500);
        builder.HasIndex(x => x.MaChauInsert).IsUnique().HasDatabaseName("UQ_md_chau_insert_ma");
    }
}
