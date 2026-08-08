using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.QuyTacNhomM;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.DungChung.QuyTacNhomM;

public sealed class QuyTacNhomMConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_quy_tac_nhom_m", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_md_quy_tac_nhom_m_dien_tich",
                "[dien_tich_tu] >= 0 AND ([dien_tich_den] IS NULL OR [dien_tich_den] > [dien_tich_tu])");
        });

        builder.CauHinhAudit();
        builder.Property(x => x.HinhDangId).HasColumnName("hinh_dang_id").IsRequired();
        builder.Property(x => x.DienTichTu).HasColumnName("dien_tich_tu").HasColumnType("decimal(18,6)").IsRequired();
        builder.Property(x => x.DienTichDen).HasColumnName("dien_tich_den").HasColumnType("decimal(18,6)");
        builder.Property(x => x.BaoGomTu).HasColumnName("bao_gom_tu").IsRequired();
        builder.Property(x => x.BaoGomDen).HasColumnName("bao_gom_den").IsRequired();
        builder.Property(x => x.NhomMId).HasColumnName("nhom_m_id").IsRequired();
        builder.Property(x => x.GhiChu).HasColumnName("ghi_chu").HasMaxLength(500);

        builder.HasIndex(x => new { x.HinhDangId, x.NhomMId })
            .IsUnique()
            .HasDatabaseName("UQ_md_quy_tac_nhom_m_hinh_dang_nhom");

        builder.HasOne(x => x.HinhDang)
            .WithMany()
            .HasForeignKey(x => x.HinhDangId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_quy_tac_nhom_m_hinh_dang");

        builder.HasOne(x => x.NhomM)
            .WithMany()
            .HasForeignKey(x => x.NhomMId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_quy_tac_nhom_m_nhom_m");
    }
}
