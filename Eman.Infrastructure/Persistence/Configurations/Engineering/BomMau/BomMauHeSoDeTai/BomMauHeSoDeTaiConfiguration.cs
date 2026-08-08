using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauHeSoDeTai;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Mau.BomMauHeSoDeTai;

public sealed class BomMauHeSoDeTaiConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_bom_mau_he_so_de_tai", "dbo", table => table.HasCheckConstraint("CK_md_bom_mau_he_so_de_tai_he_so", "[he_so] >= 0"));
        builder.CauHinhAudit();
        builder.Property(x => x.HeSanPhamId).HasColumnName("he_san_pham_id").IsRequired();
        builder.Property(x => x.MaHe).HasColumnName("ma_he").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DeTaiId).HasColumnName("de_tai_id").IsRequired();
        builder.Property(x => x.MaDeTai).HasColumnName("ma_de_tai").HasMaxLength(30).IsRequired();
        builder.Property(x => x.TenDeTai).HasColumnName("ten_de_tai").HasMaxLength(200).IsRequired();
        builder.Property(x => x.BuocId).HasColumnName("buoc_id").IsRequired();
        builder.Property(x => x.TenBuoc).HasColumnName("ten_buoc").HasMaxLength(300).IsRequired();
        builder.Property(x => x.HeSo).HasColumnName("he_so").HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(x => x.GhiChu).HasColumnName("ghi_chu").HasMaxLength(500);
        builder.HasIndex(x => new { x.HeSanPhamId, x.DeTaiId, x.BuocId }).IsUnique().HasDatabaseName("UQ_md_bom_mau_he_so_de_tai");
        builder.HasOne(x => x.HeSanPham).WithMany().HasForeignKey(x => x.HeSanPhamId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.DeTai).WithMany().HasForeignKey(x => new { x.HeSanPhamId, x.DeTaiId }).HasPrincipalKey(x => new { x.HeSanPhamId, x.Id }).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Buoc).WithMany().HasForeignKey(x => x.BuocId).OnDelete(DeleteBehavior.Restrict);
    }
}
