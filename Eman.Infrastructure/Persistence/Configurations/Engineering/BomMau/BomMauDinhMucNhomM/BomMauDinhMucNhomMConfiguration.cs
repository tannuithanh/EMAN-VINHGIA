using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauDinhMucNhomM;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Mau.BomMauDinhMucNhomM;

public sealed class BomMauDinhMucNhomMConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_bom_mau_dinh_muc_nhom_m", "dbo", table => table.HasCheckConstraint("CK_md_bom_mau_dinh_muc_nhom_m", "[dinh_muc] >= 0"));
        builder.CauHinhAudit();
        builder.Property(x => x.BuocNhomMauId).HasColumnName("buoc_nhom_mau_id").IsRequired();
        builder.Property(x => x.NhomMId).HasColumnName("nhom_m_id").IsRequired();
        builder.Property(x => x.MaNhomM).HasColumnName("ma_nhom_m").HasMaxLength(20).IsRequired();
        builder.Property(x => x.DinhMuc).HasColumnName("dinh_muc").HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(x => x.GhiChu).HasColumnName("ghi_chu").HasMaxLength(500);
        builder.HasIndex(x => new { x.BuocNhomMauId, x.NhomMId }).IsUnique().HasDatabaseName("UQ_md_bom_mau_dinh_muc_nhom_m");
        builder.HasOne(x => x.BuocNhomMau).WithMany().HasForeignKey(x => x.BuocNhomMauId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.NhomM).WithMany().HasForeignKey(x => x.NhomMId).OnDelete(DeleteBehavior.Restrict);
    }
}
