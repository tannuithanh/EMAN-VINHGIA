using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MaHang;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.DungChung.MaHang;

public sealed class MaHangConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_ma_hang", "dbo", table =>
        {
            table.HasCheckConstraint(
                "CK_md_ma_hang_dien_tich",
                "[dien_tich] IS NULL OR [dien_tich] >= 0");
        });

        builder.CauHinhAudit();

        builder.Property(x => x.MaHangCode)
            .HasColumnName("ma_hang")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DienTich)
            .HasColumnName("dien_tich")
            .HasColumnType("decimal(18,6)");

        builder.Property(x => x.HinhDangBomThoId)
            .HasColumnName("hinh_dang_bom_tho_id");

        builder.Property(x => x.HinhDangBomMauId)
            .HasColumnName("hinh_dang_bom_mau_id");

        builder.Property(x => x.MoTa)
            .HasColumnName("mo_ta")
            .HasMaxLength(500);

        builder.Property(x => x.LoaiMaHang)
            .HasColumnName("loai_ma_hang")
            .HasMaxLength(20)
            .HasDefaultValue("SAN_PHAM")
            .IsRequired();

        builder.HasIndex(x => x.MaHangCode)
            .IsUnique()
            .HasDatabaseName("UQ_md_ma_hang_ma");

        builder.HasOne(x => x.HinhDangBomTho)
            .WithMany()
            .HasForeignKey(x => x.HinhDangBomThoId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_ma_hang_hinh_dang_bom_tho");

        builder.HasOne(x => x.HinhDangBomMau)
            .WithMany()
            .HasForeignKey(x => x.HinhDangBomMauId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_ma_hang_hinh_dang_bom_mau");
    }
}
