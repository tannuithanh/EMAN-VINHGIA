using Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.VatTu;

public sealed class BomVatTuPhienBanConfiguration : IEntityTypeConfiguration<BomVatTuPhienBan>
{
    public void Configure(EntityTypeBuilder<BomVatTuPhienBan> builder)
    {
        builder.ToTable("md_bom_vat_tu_phien_ban", "dbo", table =>
        {
            table.HasCheckConstraint("CK_md_bom_vat_tu_phien_ban_so_phien_ban", "[so_phien_ban] > 0");
            table.HasCheckConstraint("CK_md_bom_vat_tu_phien_ban_trang_thai", "[trang_thai] IN (0, 1, 2)");
        });

        builder.HasKey(entity => entity.Id).HasName("PK_md_bom_vat_tu_phien_ban");
        builder.Property(entity => entity.Id).HasColumnName("id")
            .HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        builder.Property(entity => entity.VatTuId).HasColumnName("vat_tu_id").IsRequired();
        builder.Property(entity => entity.SoPhienBan).HasColumnName("so_phien_ban").IsRequired();
        builder.Property(entity => entity.TrangThai).HasColumnName("trang_thai").HasConversion<byte>().IsRequired();
        builder.Property(entity => entity.GhiChu).HasColumnName("ghi_chu").HasMaxLength(500);
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSDATETIME()").IsRequired();
        builder.Property(entity => entity.CreatedByMsnv).HasColumnName("created_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(0)");
        builder.Property(entity => entity.UpdatedByMsnv).HasColumnName("updated_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.RowVersion).HasColumnName("row_version").IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(entity => new { entity.VatTuId, entity.SoPhienBan })
            .IsUnique().HasDatabaseName("UQ_md_bom_vat_tu_phien_ban_vat_tu_so_phien_ban");
        builder.HasIndex(entity => entity.VatTuId)
            .HasDatabaseName("IX_md_bom_vat_tu_phien_ban_vat_tu_id");
        builder.HasIndex(entity => entity.TrangThai)
            .HasDatabaseName("IX_md_bom_vat_tu_phien_ban_trang_thai");
        builder.HasIndex(entity => entity.VatTuId)
            .IsUnique().HasFilter("[trang_thai] = 1")
            .HasDatabaseName("UX_md_bom_vat_tu_phien_ban_hieu_luc");

        builder.HasOne(entity => entity.VatTu)
            .WithMany()
            .HasForeignKey(entity => entity.VatTuId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_bom_vat_tu_phien_ban_vat_tu");
    }
}
