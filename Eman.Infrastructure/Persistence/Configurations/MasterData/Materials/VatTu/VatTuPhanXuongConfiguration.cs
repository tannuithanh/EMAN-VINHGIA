using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VatTuPhanXuongEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTuPhanXuong;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.Materials.VatTu;

public sealed class VatTuPhanXuongConfiguration : IEntityTypeConfiguration<VatTuPhanXuongEntity>
{
    public void Configure(EntityTypeBuilder<VatTuPhanXuongEntity> builder)
    {
        builder.ToTable("md_vat_tu_phan_xuong", "dbo");
        builder.HasKey(entity => entity.Id).HasName("PK_md_vat_tu_phan_xuong");
        builder.Property(entity => entity.Id).HasColumnName("id")
            .HasDefaultValueSql("NEWSEQUENTIALID()").ValueGeneratedOnAdd();
        builder.Property(entity => entity.VatTuId).HasColumnName("vat_tu_id").IsRequired();
        builder.Property(entity => entity.PhanXuongId).HasColumnName("phan_xuong_id").IsRequired();
        builder.Property(entity => entity.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSDATETIME()").IsRequired();
        builder.Property(entity => entity.CreatedByMsnv).HasColumnName("created_by_msnv").HasMaxLength(50);
        builder.HasIndex(entity => new { entity.VatTuId, entity.PhanXuongId }).IsUnique()
            .HasDatabaseName("UQ_md_vat_tu_phan_xuong");
        builder.HasIndex(entity => entity.PhanXuongId).HasDatabaseName("IX_md_vat_tu_phan_xuong_phan_xuong_id");
        builder.HasOne(entity => entity.VatTu).WithMany(entity => entity.PhanXuongs)
            .HasForeignKey(entity => entity.VatTuId).OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_md_vat_tu_phan_xuong_vat_tu");
        builder.HasOne(entity => entity.PhanXuong).WithMany()
            .HasForeignKey(entity => entity.PhanXuongId).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_md_vat_tu_phan_xuong_phan_xuong");
    }
}
