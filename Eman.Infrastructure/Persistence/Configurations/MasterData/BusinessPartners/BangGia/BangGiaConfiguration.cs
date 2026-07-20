using BangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.BangGia;
using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.BusinessPartners.BangGia;

public sealed class BangGiaConfiguration : IEntityTypeConfiguration<BangGiaEntity>
{
    public void Configure(EntityTypeBuilder<BangGiaEntity> builder)
    {
        builder.ToTable("md_bang_gia", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaBangGia)
            .HasColumnName("ma_bang_gia")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenBangGia)
            .HasColumnName("ten_bang_gia")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(entity => entity.DoiTacKinhDoanhId)
            .HasColumnName("doi_tac_kinh_doanh_id")
            .IsRequired();

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaBangGia)
            .IsUnique()
            .HasDatabaseName("UQ_md_bang_gia_ma");

        builder.HasIndex(entity => entity.DoiTacKinhDoanhId)
            .HasDatabaseName("IX_md_bang_gia_doi_tac_kinh_doanh_id");

        builder.HasOne(entity => entity.DoiTacKinhDoanh)
            .WithMany(entity => entity.BangGias)
            .HasForeignKey(entity => entity.DoiTacKinhDoanhId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_bang_gia_doi_tac_kinh_doanh");
    }
}
