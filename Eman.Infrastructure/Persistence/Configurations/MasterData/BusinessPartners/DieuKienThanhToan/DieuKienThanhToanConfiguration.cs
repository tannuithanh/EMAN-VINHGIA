
using DieuKienThanhToanEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienThanhToan;
using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.BusinessPartners.DieuKienThanhToan;

public sealed class DieuKienThanhToanConfiguration
    : IEntityTypeConfiguration<DieuKienThanhToanEntity>
{
    public void Configure(EntityTypeBuilder<DieuKienThanhToanEntity> builder)
    {
        builder.ToTable("md_dieu_kien_thanh_toan", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaDieuKienThanhToan)
            .HasColumnName("ma_dieu_kien_thanh_toan")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenDieuKienThanhToan)
            .HasColumnName("ten_dieu_kien_thanh_toan")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaDieuKienThanhToan)
            .IsUnique()
            .HasDatabaseName("UQ_md_dieu_kien_thanh_toan_ma");
    }
}
