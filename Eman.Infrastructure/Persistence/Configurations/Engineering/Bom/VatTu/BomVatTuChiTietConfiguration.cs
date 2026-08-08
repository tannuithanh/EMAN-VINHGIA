using Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.VatTu;

public sealed class BomVatTuChiTietConfiguration : IEntityTypeConfiguration<BomVatTuChiTiet>
{
    public void Configure(EntityTypeBuilder<BomVatTuChiTiet> builder)
    {
        builder.ToTable("md_bom_vat_tu_chi_tiet", "dbo", table =>
        {
            table.HasCheckConstraint("CK_md_bom_vat_tu_chi_tiet_so_luong", "[so_luong] > 0");
            table.HasCheckConstraint("CK_md_bom_vat_tu_chi_tiet_thu_tu", "[thu_tu] > 0");
        });

        builder.HasKey(entity => entity.Id).HasName("PK_md_bom_vat_tu_chi_tiet");
        builder.Property(entity => entity.Id).HasColumnName("id")
            .HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        builder.Property(entity => entity.BomVatTuPhienBanId).HasColumnName("bom_vat_tu_phien_ban_id").IsRequired();
        builder.Property(entity => entity.VatTuThanhPhanId).HasColumnName("vat_tu_thanh_phan_id").IsRequired();
        builder.Property(entity => entity.SoLuong).HasColumnName("so_luong").HasPrecision(18, 6).IsRequired();
        builder.Property(entity => entity.ThuTu).HasColumnName("thu_tu").HasDefaultValue(1).IsRequired();
        builder.Property(entity => entity.GhiChu).HasColumnName("ghi_chu").HasMaxLength(500);
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSDATETIME()").IsRequired();
        builder.Property(entity => entity.CreatedByMsnv).HasColumnName("created_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(0)");
        builder.Property(entity => entity.UpdatedByMsnv).HasColumnName("updated_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.RowVersion).HasColumnName("row_version").IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(entity => new { entity.BomVatTuPhienBanId, entity.VatTuThanhPhanId })
            .IsUnique().HasDatabaseName("UQ_md_bom_vat_tu_chi_tiet_phien_ban_vat_tu");
        builder.HasIndex(entity => entity.BomVatTuPhienBanId)
            .HasDatabaseName("IX_md_bom_vat_tu_chi_tiet_phien_ban_id");
        builder.HasIndex(entity => entity.VatTuThanhPhanId)
            .HasDatabaseName("IX_md_bom_vat_tu_chi_tiet_vat_tu_thanh_phan_id");

        builder.HasOne(entity => entity.BomVatTuPhienBan)
            .WithMany(entity => entity.ChiTiets)
            .HasForeignKey(entity => entity.BomVatTuPhienBanId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_bom_vat_tu_chi_tiet_phien_ban");

        builder.HasOne(entity => entity.VatTuThanhPhan)
            .WithMany()
            .HasForeignKey(entity => entity.VatTuThanhPhanId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_bom_vat_tu_chi_tiet_vat_tu_thanh_phan");
    }
}
