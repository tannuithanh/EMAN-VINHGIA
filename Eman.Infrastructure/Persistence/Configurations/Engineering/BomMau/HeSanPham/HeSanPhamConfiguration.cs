using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HeSanPham;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.DungChung.HeSanPham;

public sealed class HeSanPhamConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_he_san_pham", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.MaHe).HasColumnName("ma_he").HasMaxLength(20).IsRequired();
        builder.Property(x => x.TenHe).HasColumnName("ten_he").HasMaxLength(200).IsRequired();
        builder.Property(x => x.MoTa).HasColumnName("mo_ta").HasMaxLength(500);
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(0)").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(0)");
        builder.HasIndex(x => x.MaHe).IsUnique().HasDatabaseName("UQ_md_he_san_pham_ma_he");
    }
}
