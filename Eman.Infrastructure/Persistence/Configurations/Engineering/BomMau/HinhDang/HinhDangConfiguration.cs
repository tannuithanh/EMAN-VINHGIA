using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HinhDang;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.DungChung.HinhDang;

public sealed class HinhDangConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_hinh_dang", "dbo");
        builder.CauHinhAudit();
        builder.Property(x => x.MaHinhDang).HasColumnName("ma_hinh_dang").HasMaxLength(30).IsRequired();
        builder.Property(x => x.TenHinhDang).HasColumnName("ten_hinh_dang").HasMaxLength(200).IsRequired();
        builder.Property(x => x.MoTa).HasColumnName("mo_ta").HasMaxLength(500);
        builder.HasIndex(x => x.MaHinhDang).IsUnique().HasDatabaseName("UQ_md_hinh_dang_ma");
    }
}
