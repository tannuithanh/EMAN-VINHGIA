using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauBuoc;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Mau.BomMauBuoc;

public sealed class BomMauBuocConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_bom_buoc", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(x => x.MaBuoc).HasColumnName("ma_buoc").HasMaxLength(30).IsRequired();
        builder.Property(x => x.TenBuoc).HasColumnName("ten_buoc").HasMaxLength(300).IsRequired();
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.RowVersion).HasColumnName("row_version").IsRowVersion().IsConcurrencyToken();
        builder.HasIndex(x => x.MaBuoc).IsUnique().HasDatabaseName("UQ_md_bom_buoc_ma_buoc");
    }
}
