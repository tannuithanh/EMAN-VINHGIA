using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KhoEntity = Eman.Domain.Modules.MasterData.Inventory.Entities.Kho;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Inventory.Kho;

public sealed class KhoConfiguration : IEntityTypeConfiguration<KhoEntity>
{
    public void Configure(EntityTypeBuilder<KhoEntity> builder)
    {
        builder.ToTable("md_kho", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaKho)
            .HasColumnName("ma_kho")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenKho)
            .HasColumnName("ten_kho")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.HangTon)
            .HasColumnName("hang_ton")
            .IsRequired();

        builder.Property(entity => entity.HangTru)
            .HasColumnName("hang_tru")
            .IsRequired();

        builder.Property(entity => entity.MoTa)
            .HasColumnName("mo_ta")
            .HasMaxLength(500);

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaKho)
            .IsUnique()
            .HasDatabaseName("UQ_md_kho_ma_kho");
    }
}
