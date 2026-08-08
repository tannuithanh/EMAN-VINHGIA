using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhanXuongEntity = Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Production.PhanXuong;

public sealed class PhanXuongConfiguration : IEntityTypeConfiguration<PhanXuongEntity>
{
    public void Configure(EntityTypeBuilder<PhanXuongEntity> builder)
    {
        builder.ToTable("md_phan_xuong", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaPhanXuong)
            .HasColumnName("ma_phan_xuong")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenPhanXuong)
            .HasColumnName("ten_phan_xuong")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.MoTa)
            .HasColumnName("mo_ta")
            .HasMaxLength(500);

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaPhanXuong)
            .IsUnique()
            .HasDatabaseName("UQ_md_phan_xuong_ma_phan_xuong");
    }
}
