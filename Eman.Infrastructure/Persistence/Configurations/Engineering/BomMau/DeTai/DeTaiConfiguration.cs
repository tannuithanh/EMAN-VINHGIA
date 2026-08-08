using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.DeTai;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.DungChung.DeTai;

public sealed class DeTaiConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_de_tai", "dbo");
        builder.CauHinhAudit();
        builder.Property(x => x.HeSanPhamId).HasColumnName("he_san_pham_id").IsRequired();
        builder.Property(x => x.MaDeTai).HasColumnName("ma_de_tai").HasMaxLength(30).IsRequired();
        builder.Property(x => x.TenDeTai).HasColumnName("ten_de_tai").HasMaxLength(200).IsRequired();
        builder.Property(x => x.MoTa).HasColumnName("mo_ta").HasMaxLength(500);
        builder.HasAlternateKey(x => new { x.HeSanPhamId, x.Id }).HasName("UQ_md_de_tai_he_id");
        builder.HasIndex(x => new { x.HeSanPhamId, x.MaDeTai }).IsUnique().HasDatabaseName("UQ_md_de_tai_he_ma");
        builder.HasOne(x => x.HeSanPham).WithMany(x => x.DeTais).HasForeignKey(x => x.HeSanPhamId).OnDelete(DeleteBehavior.Restrict);
    }
}
