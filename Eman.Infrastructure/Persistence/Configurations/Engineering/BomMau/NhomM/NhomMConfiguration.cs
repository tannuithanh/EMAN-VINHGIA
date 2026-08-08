using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.NhomM;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.DungChung.NhomM;

public sealed class NhomMConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_nhom_m", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_md_nhom_m_pham_vi_bom",
                "[pham_vi_bom] IN (N'BOM_THO', N'BOM_MAU')");
            table.HasCheckConstraint("CK_md_nhom_m_thu_tu", "[thu_tu] > 0");
        });

        builder.CauHinhAudit();
        builder.Property(x => x.PhamViBom).HasColumnName("pham_vi_bom").HasMaxLength(20).IsRequired();
        builder.Property(x => x.MaNhomM).HasColumnName("ma_nhom_m").HasMaxLength(20).IsRequired();
        builder.Property(x => x.TenNhomM).HasColumnName("ten_nhom_m").HasMaxLength(200).IsRequired();
        builder.Property(x => x.ThuTu).HasColumnName("thu_tu").IsRequired();
        builder.Property(x => x.MoTa).HasColumnName("mo_ta").HasMaxLength(500);

        builder.HasIndex(x => new { x.PhamViBom, x.MaNhomM })
            .IsUnique()
            .HasDatabaseName("UQ_md_nhom_m_pham_vi_ma");
    }
}
