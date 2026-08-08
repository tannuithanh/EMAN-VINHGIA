using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BuocNhomTheoMau;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Mau.BuocNhomTheoMau;

public sealed class BuocNhomTheoMauConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_buoc_nhom_theo_mau", "dbo");
        builder.CauHinhAudit();

        // Bảng thực tế đang dùng datetime2(7), khác các bảng B.O.M còn lại.
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("datetime2(7)")
            .IsRequired();
        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("datetime2(7)");

        builder.Property(x => x.HeSanPhamId)
            .HasColumnName("he_san_pham_id")
            .IsRequired();
        builder.Property(x => x.MauSacId)
            .HasColumnName("mau_sac")
            .IsRequired();
        builder.Property(x => x.MaBuoc)
            .HasColumnName("ma_buoc")
            .HasMaxLength(300)
            .IsRequired();
        builder.Property(x => x.TenBuoc)
            .HasColumnName("ten_buoc")
            .HasMaxLength(300)
            .IsRequired();
        builder.Property(x => x.MaHonHopId)
            .HasColumnName("ma_hon_hop_id")
            .IsRequired();
        builder.Property(x => x.MaHonHop)
            .HasColumnName("ma_hon_hop")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.GhiChu)
            .HasColumnName("ghi_chu")
            .HasMaxLength(500);

        builder.HasIndex(x => new
            {
                x.HeSanPhamId,
                x.MauSacId,
                x.MaBuoc,
                x.MaHonHopId
            })
            .IsUnique()
            .HasDatabaseName("UQ_md_buoc_nhom_theo_mau");

        builder.HasOne(x => x.HeSanPham)
            .WithMany()
            .HasForeignKey(x => x.HeSanPhamId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_buoc_nhom_theo_mau_he_san_pham");

        // Database chưa khai báo FK cho cột mau_sac, nhưng cột này đang lưu id của md_mau_sac.
        // Khai báo quan hệ trong EF để truy vấn dữ liệu liên quan đúng kiểu BIGINT.
        builder.HasOne(x => x.MauSac)
            .WithMany()
            .HasForeignKey(x => x.MauSacId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
