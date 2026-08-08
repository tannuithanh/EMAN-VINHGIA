using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NhomNangLucEntity = Eman.Domain.Modules.MasterData.Production.Entities.NhomNangLuc;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Production.NhomNangLuc;

public sealed class NhomNangLucConfiguration : IEntityTypeConfiguration<NhomNangLucEntity>
{
    public void Configure(EntityTypeBuilder<NhomNangLucEntity> builder)
    {
        builder.ToTable("md_nhom_nang_luc", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaNhomNangLuc)
            .HasColumnName("ma_nhom_nang_luc")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenNhomNangLuc)
            .HasColumnName("ten_nhom_nang_luc")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.ThoiGianLamHang)
            .HasColumnName("thoi_gian_lam_hang");

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaNhomNangLuc)
            .IsUnique()
            .HasDatabaseName("UQ_md_nhom_nang_luc_ma");
    }
}
