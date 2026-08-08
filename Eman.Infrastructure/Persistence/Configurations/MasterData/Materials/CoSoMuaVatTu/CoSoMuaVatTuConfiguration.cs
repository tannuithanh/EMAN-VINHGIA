using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoSoMuaVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.CoSoMuaVatTu;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Materials.CoSoMuaVatTu;

public sealed class CoSoMuaVatTuConfiguration : IEntityTypeConfiguration<CoSoMuaVatTuEntity>
{
    public void Configure(EntityTypeBuilder<CoSoMuaVatTuEntity> builder)
    {
        builder.ToTable("md_co_so_mua_vat_tu", "dbo", table =>
            table.HasCheckConstraint("CK_md_co_so_mua_vat_tu_trang_thai", "[trang_thai] IN (0, 1)"));
        builder.HasKey(entity => entity.Id).HasName("PK_md_co_so_mua_vat_tu");
        builder.Property(entity => entity.Id).HasColumnName("id")
            .HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        builder.Property(entity => entity.MaCoSoMuaVatTu).HasColumnName("ma_co_so_mua_vat_tu")
            .HasMaxLength(50).IsRequired();
        builder.Property(entity => entity.TenCoSoMuaVatTu).HasColumnName("ten_co_so_mua_vat_tu")
            .HasMaxLength(300).IsRequired();
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
        builder.HasIndex(entity => entity.MaCoSoMuaVatTu).IsUnique()
            .HasDatabaseName("UQ_md_co_so_mua_vat_tu_ma");
    }
}
