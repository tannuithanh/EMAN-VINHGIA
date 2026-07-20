using DoiTacKinhDoanhEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DoiTacKinhDoanh;
using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.BusinessPartners.DoiTacKinhDoanh;

public sealed class DoiTacKinhDoanhConfiguration : IEntityTypeConfiguration<DoiTacKinhDoanhEntity>
{
    public void Configure(EntityTypeBuilder<DoiTacKinhDoanhEntity> builder)
    {
        builder.ToTable("md_doi_tac_kinh_doanh", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaDoiTac)
            .HasColumnName("ma_doi_tac")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenDoiTac)
            .HasColumnName("ten_doi_tac")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(entity => entity.LoaiDoiTacId)
            .HasColumnName("loai_doi_tac_id")
            .IsRequired();

        builder.Property(entity => entity.LaNhaCungCap)
            .HasColumnName("la_nha_cung_cap")
            .IsRequired();

        builder.Property(entity => entity.MaSoThue)
            .HasColumnName("ma_so_thue")
            .HasMaxLength(50);

        builder.Property(entity => entity.DiaChi)
            .HasColumnName("dia_chi")
            .HasMaxLength(500);

        builder.Property(entity => entity.NguoiLienHe)
            .HasColumnName("nguoi_lien_he")
            .HasMaxLength(200);

        builder.Property(entity => entity.DienThoai)
            .HasColumnName("dien_thoai")
            .HasMaxLength(50);

        builder.Property(entity => entity.Email)
            .HasColumnName("email")
            .HasMaxLength(200);

        builder.Property(entity => entity.SoTaiKhoan)
            .HasColumnName("so_tai_khoan")
            .HasMaxLength(100);

        builder.Property(entity => entity.TenNganHang)
            .HasColumnName("ten_ngan_hang")
            .HasMaxLength(250);

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaDoiTac)
            .IsUnique()
            .HasDatabaseName("UQ_md_doi_tac_kinh_doanh_ma");

        builder.HasIndex(entity => entity.LoaiDoiTacId)
            .HasDatabaseName("IX_md_doi_tac_kinh_doanh_loai_doi_tac_id");

        builder.HasOne(entity => entity.LoaiDoiTac)
            .WithMany(entity => entity.DoiTacKinhDoanhs)
            .HasForeignKey(entity => entity.LoaiDoiTacId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_doi_tac_kinh_doanh_loai_doi_tac");
    }
}
