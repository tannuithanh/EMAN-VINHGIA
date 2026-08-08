using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MauSac;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.DungChung.MauSac;

public sealed class MauSacConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_mau_sac", "dbo");
        builder.CauHinhAudit();
        builder.Property(x => x.HeSanPhamId).HasColumnName("he_san_pham_id").IsRequired();
        builder.Property(x => x.DeTaiId).HasColumnName("de_tai_id").IsRequired();
        builder.Property(x => x.MaMau).HasColumnName("ma_mau").HasMaxLength(30).IsRequired();
        builder.Property(x => x.TenMau).HasColumnName("ten_mau").HasMaxLength(200).IsRequired();
        builder.Property(x => x.MaCotTho).HasColumnName("ma_cot_tho").HasMaxLength(30);
        builder.Property(x => x.MoTa).HasColumnName("mo_ta").HasMaxLength(500);

        builder.HasAlternateKey(x => new { x.HeSanPhamId, x.DeTaiId, x.Id })
            .HasName("UQ_md_mau_sac_he_de_tai_id");
        builder.HasAlternateKey(x => new { x.DeTaiId, x.Id })
            .HasName("UQ_md_mau_sac_de_tai_id");
        builder.HasIndex(x => new { x.HeSanPhamId, x.DeTaiId, x.MaMau })
            .IsUnique()
            .HasDatabaseName("UQ_md_mau_sac_he_de_tai_ma");

        builder.HasOne(x => x.HeSanPham)
            .WithMany(x => x.MauSacs)
            .HasForeignKey(x => x.HeSanPhamId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DeTai)
            .WithMany(x => x.MauSacs)
            .HasForeignKey(x => new { x.HeSanPhamId, x.DeTaiId })
            .HasPrincipalKey(x => new { x.HeSanPhamId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
