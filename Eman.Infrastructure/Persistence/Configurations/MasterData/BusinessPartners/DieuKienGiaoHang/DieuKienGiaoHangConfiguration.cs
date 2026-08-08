
using DieuKienGiaoHangEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienGiaoHang;
using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.BusinessPartners.DieuKienGiaoHang;

public sealed class DieuKienGiaoHangConfiguration
    : IEntityTypeConfiguration<DieuKienGiaoHangEntity>
{
    public void Configure(EntityTypeBuilder<DieuKienGiaoHangEntity> builder)
    {
        builder.ToTable("md_dieu_kien_giao_hang", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaDieuKienGiaoHang)
            .HasColumnName("ma_dieu_kien_giao_hang")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenDieuKienGiaoHang)
            .HasColumnName("ten_dieu_kien_giao_hang")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaDieuKienGiaoHang)
            .IsUnique()
            .HasDatabaseName("UQ_md_dieu_kien_giao_hang_ma");
    }
}
