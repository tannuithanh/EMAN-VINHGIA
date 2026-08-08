using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DonViTinhEntity = Eman.Domain.Modules.MasterData.Common.Entities.DonViTinh;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Common.DonViTinh;

public sealed class DonViTinhConfiguration : IEntityTypeConfiguration<DonViTinhEntity>
{
    public void Configure(EntityTypeBuilder<DonViTinhEntity> builder)
    {
        builder.ToTable("md_don_vi_tinh", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaDonViTinh)
            .HasColumnName("ma_don_vi_tinh")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenDonViTinh)
            .HasColumnName("ten_don_vi_tinh")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.KyHieu)
            .HasColumnName("ky_hieu")
            .HasMaxLength(50);

        builder.Property(entity => entity.MoTa)
            .HasColumnName("mo_ta")
            .HasMaxLength(500);

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaDonViTinh)
            .IsUnique()
            .HasDatabaseName("UQ_md_don_vi_tinh_ma_don_vi_tinh");
    }
}
