using PhienBanBangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.PhienBanBangGia;
using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.BusinessPartners.PhienBanBangGia;

public sealed class PhienBanBangGiaConfiguration : IEntityTypeConfiguration<PhienBanBangGiaEntity>
{
    public void Configure(EntityTypeBuilder<PhienBanBangGiaEntity> builder)
    {
        builder.ToTable("md_phien_ban_bang_gia", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.BangGiaId)
            .HasColumnName("bang_gia_id")
            .IsRequired();

        builder.Property(entity => entity.SoPhienBan)
            .HasColumnName("so_phien_ban")
            .IsRequired();

        builder.Property(entity => entity.TuNgay)
            .HasColumnName("tu_ngay")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(entity => entity.DenNgay)
            .HasColumnName("den_ngay")
            .HasColumnType("date");

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => new { entity.BangGiaId, entity.SoPhienBan })
            .IsUnique()
            .HasDatabaseName("UQ_md_phien_ban_bang_gia_so_phien_ban");

        builder.HasIndex(entity => entity.BangGiaId)
            .HasDatabaseName("IX_md_phien_ban_bang_gia_bang_gia_id");

        builder.HasIndex(entity => entity.BangGiaId)
            .IsUnique()
            .HasFilter("[trang_thai] = 1")
            .HasDatabaseName("UX_md_phien_ban_bang_gia_dang_hieu_luc");

        builder.HasOne(entity => entity.BangGia)
            .WithMany(entity => entity.PhienBanBangGias)
            .HasForeignKey(entity => entity.BangGiaId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_phien_ban_bang_gia_bang_gia");
    }
}
