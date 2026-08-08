using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangPhen;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Mau.BomMaHangPhen;

public sealed class BomMaHangPhenConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_bom_ma_hang_phen", "dbo");
        builder.CauHinhAuditGuid();
        builder.Property(x => x.MaHangId).HasColumnName("ma_hang_id").IsRequired();
        builder.Property(x => x.MaHang).HasColumnName("ma_hang").HasMaxLength(100).IsRequired();
        builder.Property(x => x.MaHangPhen).HasColumnName("ma_hang_phen").HasMaxLength(100).IsRequired();
        builder.Property(x => x.GhiChu).HasColumnName("ghi_chu").HasMaxLength(500);
        builder.HasIndex(x => x.MaHangId).IsUnique().HasDatabaseName("UQ_md_bom_ma_hang_phen_ma_hang");
        builder.HasOne(x => x.MaHangNavigation).WithMany().HasForeignKey(x => x.MaHangId).OnDelete(DeleteBehavior.Restrict);
    }
}
