using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Materials.VatTu;

public sealed class VatTuConfiguration : IEntityTypeConfiguration<VatTuEntity>
{
    public void Configure(EntityTypeBuilder<VatTuEntity> builder)
    {
        builder.ToTable("md_vat_tu", "dbo", table =>
        {
            table.HasCheckConstraint("CK_md_vat_tu_trang_thai", "[trang_thai] IN (0, 1)");
            table.HasCheckConstraint("CK_md_vat_tu_pham_vi_su_dung", "[pham_vi_su_dung] IS NULL OR [pham_vi_su_dung] IN (1, 2)");
            table.HasCheckConstraint("CK_md_vat_tu_phuong_thuc_cung_ung", "[phuong_thuc_cung_ung] IN (1, 2, 3)");
            table.HasCheckConstraint("CK_md_vat_tu_ngay_mua_hang", "[ngay_mua_hang] IS NULL OR [ngay_mua_hang] >= 0");
            table.HasCheckConstraint("CK_md_vat_tu_han_su_dung", "[han_su_dung_ngay] >= 0");
            table.HasCheckConstraint("CK_md_vat_tu_moq", "[moq] IS NULL OR [moq] > 0");
            table.HasCheckConstraint("CK_md_vat_tu_ton_toi_thieu", "[ton_toi_thieu] IS NULL OR [ton_toi_thieu] >= 0");
            table.HasCheckConstraint(
                "CK_md_vat_tu_thong_tin_mua",
                "(([phuong_thuc_cung_ung] = 3 AND [co_so_mua_vat_tu_id] IS NULL AND [nha_cung_cap_mac_dinh_id] IS NULL AND [ngay_mua_hang] IS NULL AND [moq] IS NULL AND [thue_vat_id] IS NULL) OR ([phuong_thuc_cung_ung] IN (1, 2) AND [co_so_mua_vat_tu_id] IS NOT NULL AND [ngay_mua_hang] IS NOT NULL AND [thue_vat_id] IS NOT NULL))");
        });

        builder.HasKey(entity => entity.Id).HasName("PK_md_vat_tu");
        builder.Property(entity => entity.Id).HasColumnName("id")
            .HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        builder.Property(entity => entity.MaVatTu).HasColumnName("ma_vat_tu").HasMaxLength(100).IsRequired();
        builder.Property(entity => entity.TenVatTu).HasColumnName("ten_vat_tu").HasMaxLength(300).IsRequired();
        builder.Property(entity => entity.TenTiengAnh).HasColumnName("ten_tieng_anh").HasMaxLength(300);
        builder.Property(entity => entity.DonViTinhId).HasColumnName("don_vi_tinh_id").IsRequired();
        builder.Property(entity => entity.QuyCachDongGoi).HasColumnName("quy_cach_dong_goi").HasMaxLength(500);
        builder.Property(entity => entity.PhamViSuDung).HasColumnName("pham_vi_su_dung");
        builder.Property(entity => entity.NhomVatTuId).HasColumnName("nhom_vat_tu_id").IsRequired();
        builder.Property(entity => entity.MucDichSuDung).HasColumnName("muc_dich_su_dung").HasMaxLength(1000);
        builder.Property(entity => entity.PhuongThucCungUng).HasColumnName("phuong_thuc_cung_ung").HasConversion<byte>().IsRequired();
        builder.Property(entity => entity.CoSoMuaVatTuId).HasColumnName("co_so_mua_vat_tu_id");
        builder.Property(entity => entity.NhaCungCapMacDinhId).HasColumnName("nha_cung_cap_mac_dinh_id");
        builder.Property(entity => entity.NgayMuaHang).HasColumnName("ngay_mua_hang");
        builder.Property(entity => entity.HanSuDungNgay).HasColumnName("han_su_dung_ngay").IsRequired();
        builder.Property(entity => entity.Moq).HasColumnName("moq").HasPrecision(18, 3);
        builder.Property(entity => entity.ThueVatId).HasColumnName("thue_vat_id");
        builder.Property(entity => entity.TonToiThieu).HasColumnName("ton_toi_thieu").HasPrecision(18, 3);
        builder.Property(entity => entity.KhoLuuTruId).HasColumnName("kho_luu_tru_id");
        builder.Property(entity => entity.TrangThai).HasColumnName("trang_thai").HasConversion<byte>()
            .IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSDATETIME()").IsRequired();
        builder.Property(entity => entity.CreatedByMsnv).HasColumnName("created_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(0)");
        builder.Property(entity => entity.UpdatedByMsnv).HasColumnName("updated_by_msnv").HasMaxLength(50);
        builder.Property(entity => entity.RowVersion).HasColumnName("row_version").IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(entity => entity.MaVatTu).IsUnique().HasDatabaseName("UQ_md_vat_tu_ma");
        builder.HasIndex(entity => entity.DonViTinhId).HasDatabaseName("IX_md_vat_tu_don_vi_tinh_id");
        builder.HasIndex(entity => entity.NhomVatTuId).HasDatabaseName("IX_md_vat_tu_nhom_vat_tu_id");
        builder.HasIndex(entity => entity.CoSoMuaVatTuId).HasDatabaseName("IX_md_vat_tu_co_so_mua_id");
        builder.HasIndex(entity => entity.NhaCungCapMacDinhId).HasDatabaseName("IX_md_vat_tu_ncc_mac_dinh_id");
        builder.HasIndex(entity => entity.ThueVatId).HasDatabaseName("IX_md_vat_tu_thue_vat_id");
        builder.HasIndex(entity => entity.KhoLuuTruId).HasDatabaseName("IX_md_vat_tu_kho_luu_tru_id");
        builder.HasIndex(entity => new { entity.PhuongThucCungUng, entity.TrangThai })
            .HasDatabaseName("IX_md_vat_tu_phuong_thuc_trang_thai");

        builder.HasOne(entity => entity.DonViTinh).WithMany()
            .HasForeignKey(entity => entity.DonViTinhId).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_vat_tu_don_vi_tinh");
        builder.HasOne(entity => entity.NhomVatTu).WithMany(entity => entity.VatTus)
            .HasForeignKey(entity => entity.NhomVatTuId).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_vat_tu_nhom_vat_tu");
        builder.HasOne(entity => entity.CoSoMuaVatTu).WithMany(entity => entity.VatTus)
            .HasForeignKey(entity => entity.CoSoMuaVatTuId).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_vat_tu_co_so_mua");
        builder.HasOne(entity => entity.NhaCungCapMacDinh).WithMany()
            .HasForeignKey(entity => entity.NhaCungCapMacDinhId).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_vat_tu_ncc_mac_dinh");
        builder.HasOne(entity => entity.ThueVat).WithMany()
            .HasForeignKey(entity => entity.ThueVatId).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_vat_tu_thue_vat");
        builder.HasOne(entity => entity.KhoLuuTru).WithMany()
            .HasForeignKey(entity => entity.KhoLuuTruId).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_vat_tu_kho_luu_tru");
    }
}
