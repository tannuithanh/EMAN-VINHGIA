using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.SanPham;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Products.SanPham;

public sealed class SanPhamConfiguration : IEntityTypeConfiguration<SanPhamEntity>
{
    public void Configure(EntityTypeBuilder<SanPhamEntity> builder)
    {
        builder.ToTable("md_san_pham", "dbo");

        builder.HasKey(entity => entity.Id)
            .HasName("PK_md_san_pham");

        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        builder.Property(entity => entity.MaSanPham)
            .HasColumnName("ma_san_pham")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.MoTaTiengViet)
            .HasColumnName("mo_ta_tieng_viet")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(entity => entity.MoTaTiengAnh)
            .HasColumnName("mo_ta_tieng_anh")
            .HasMaxLength(500);

        builder.Property(entity => entity.DonViTinhId)
            .HasColumnName("don_vi_tinh_id")
            .IsRequired();

        builder.Property(entity => entity.NhomNangLucId)
            .HasColumnName("nhom_nang_luc_id");

        builder.Property(entity => entity.ChieuDaiCm)
            .HasColumnName("chieu_dai_cm")
            .HasPrecision(18, 3);

        builder.Property(entity => entity.ChieuRongCm)
            .HasColumnName("chieu_rong_cm")
            .HasPrecision(18, 3);

        builder.Property(entity => entity.ChieuCaoCm)
            .HasColumnName("chieu_cao_cm")
            .HasPrecision(18, 3);

        builder.Property(entity => entity.TrongLuong)
            .HasColumnName("trong_luong")
            .HasPrecision(18, 3);

        builder.Property(entity => entity.DienTich)
            .HasColumnName("dien_tich")
            .HasPrecision(18, 4);

        builder.Property(entity => entity.DoKho)
            .HasColumnName("do_kho")
            .HasPrecision(18, 4);

        builder.Property(entity => entity.HeSoTiTrong)
            .HasColumnName("he_so_ti_trong")
            .HasPrecision(18, 6);

        builder.Property(entity => entity.CbmMacDinh)
            .HasColumnName("cbm_mac_dinh")
            .HasPrecision(18, 6);

        builder.Property(entity => entity.KhoMacDinhId)
            .HasColumnName("kho_mac_dinh_id");

        builder.Property(entity => entity.KhoTonId)
            .HasColumnName("kho_ton_id");

        builder.Property(entity => entity.XuongMacDinhId)
            .HasColumnName("xuong_mac_dinh_id");

        builder.Property(entity => entity.ThueId)
            .HasColumnName("thue_id");

        builder.Property(entity => entity.LaBanThanhPham)
            .HasColumnName("la_ban_thanh_pham")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(entity => entity.NoiGiaoHang)
            .HasColumnName("noi_giao_hang")
            .HasMaxLength(500);

        builder.Property(entity => entity.GhiChu)
            .HasColumnName("ghi_chu")
            .HasMaxLength(1000);

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("SYSDATETIME()")
            .IsRequired();

        builder.Property(entity => entity.CreatedByMsnv)
            .HasColumnName("created_by_msnv")
            .HasMaxLength(50);

        builder.Property(entity => entity.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(entity => entity.UpdatedByMsnv)
            .HasColumnName("updated_by_msnv")
            .HasMaxLength(50);

        builder.Property(entity => entity.RowVersion)
            .HasColumnName("row_version")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(entity => entity.MaSanPham)
            .IsUnique()
            .HasDatabaseName("UQ_md_san_pham_ma_san_pham");

        builder.HasIndex(entity => entity.DonViTinhId)
            .HasDatabaseName("IX_md_san_pham_don_vi_tinh_id");

        builder.HasIndex(entity => entity.NhomNangLucId)
            .HasDatabaseName("IX_md_san_pham_nhom_nang_luc_id");

        builder.HasIndex(entity => entity.KhoMacDinhId)
            .HasDatabaseName("IX_md_san_pham_kho_mac_dinh_id");

        builder.HasIndex(entity => entity.KhoTonId)
            .HasDatabaseName("IX_md_san_pham_kho_ton_id");

        builder.HasIndex(entity => entity.XuongMacDinhId)
            .HasDatabaseName("IX_md_san_pham_xuong_mac_dinh_id");

        builder.HasIndex(entity => entity.ThueId)
            .HasDatabaseName("IX_md_san_pham_thue_id");


        builder.HasOne(entity => entity.DonViTinh)
            .WithMany()
            .HasForeignKey(entity => entity.DonViTinhId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_san_pham_don_vi_tinh");

        builder.HasOne(entity => entity.NhomNangLuc)
            .WithMany()
            .HasForeignKey(entity => entity.NhomNangLucId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_san_pham_nhom_nang_luc");

        builder.HasOne(entity => entity.KhoMacDinh)
            .WithMany()
            .HasForeignKey(entity => entity.KhoMacDinhId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_san_pham_kho_mac_dinh");

        builder.HasOne(entity => entity.KhoTon)
            .WithMany()
            .HasForeignKey(entity => entity.KhoTonId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_san_pham_kho_ton");

        builder.HasOne(entity => entity.XuongMacDinh)
            .WithMany()
            .HasForeignKey(entity => entity.XuongMacDinhId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_san_pham_xuong_mac_dinh");

        builder.HasOne(entity => entity.Thue)
            .WithMany()
            .HasForeignKey(entity => entity.ThueId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_san_pham_thue_san_pham");

    }
}
