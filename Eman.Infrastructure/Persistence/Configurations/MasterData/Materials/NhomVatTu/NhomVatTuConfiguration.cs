using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NhomVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.NhomVatTu;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Materials.NhomVatTu;

public sealed class NhomVatTuConfiguration : IEntityTypeConfiguration<NhomVatTuEntity>
{
    public void Configure(EntityTypeBuilder<NhomVatTuEntity> builder)
    {
        builder.ToTable("md_nhom_vat_tu", "dbo", table =>
            table.HasCheckConstraint("CK_md_nhom_vat_tu_trang_thai", "[trang_thai] IN (0, 1)"));
        builder.HasKey(entity => entity.Id).HasName("PK_md_nhom_vat_tu");
        builder.Property(entity => entity.Id).HasColumnName("id")
            .HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        builder.Property(entity => entity.MaNhomVatTu).HasColumnName("ma_nhom_vat_tu")
            .HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.TenNhomVatTu).HasColumnName("ten_nhom_vat_tu")
            .HasMaxLength(200).IsRequired();
        builder.Property(entity => entity.MoTa).HasColumnName("mo_ta").HasMaxLength(500);
        builder.Property(entity => entity.TrangThai).HasColumnName("trang_thai")
            .HasConversion<byte>().IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at")
            .HasColumnType("datetime2(0)").HasDefaultValueSql("SYSDATETIME()").IsRequired();
        builder.Property(entity => entity.CreatedByMsnv).HasColumnName("created_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(0)");
        builder.Property(entity => entity.UpdatedByMsnv).HasColumnName("updated_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.RowVersion).HasColumnName("row_version")
            .IsRowVersion().IsConcurrencyToken();
        builder.HasIndex(entity => entity.MaNhomVatTu).IsUnique()
            .HasDatabaseName("UQ_md_nhom_vat_tu_ma");
    }
}
