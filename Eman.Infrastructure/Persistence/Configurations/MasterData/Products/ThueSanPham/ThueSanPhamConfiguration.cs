using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ThueSanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.ThueSanPham;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Products.ThueSanPham;

public sealed class ThueSanPhamConfiguration : IEntityTypeConfiguration<ThueSanPhamEntity>
{
    public void Configure(EntityTypeBuilder<ThueSanPhamEntity> builder)
    {
        builder.ToTable("md_thue_san_pham", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaThue)
            .HasColumnName("ma_thue")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenThue)
            .HasColumnName("ten_thue")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.ThueSuat)
            .HasColumnName("thue_suat")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaThue)
            .IsUnique()
            .HasDatabaseName("UQ_md_thue_san_pham_ma");
    }
}
