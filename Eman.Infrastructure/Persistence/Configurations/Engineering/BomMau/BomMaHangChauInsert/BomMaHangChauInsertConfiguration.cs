using Eman.Infrastructure.Persistence.Configurations.Engineering.Bom.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangChauInsert;

namespace Eman.Infrastructure.Persistence.Configurations.Engineering.BomMau.BomMaHangChauInsert;

public sealed class BomMaHangChauInsertConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.ToTable("md_bom_ma_hang_chau_insert", "dbo");
        builder.CauHinhAuditGuid();
        builder.Property(x => x.MaHangId).HasColumnName("ma_hang_id").IsRequired();
        builder.Property(x => x.MaHang).HasColumnName("ma_hang").HasMaxLength(100).IsRequired();
        builder.Property(x => x.ChauInsertId).HasColumnName("chau_insert_id").HasColumnType("uniqueidentifier").IsRequired();
        builder.Property(x => x.MaChauInsert).HasColumnName("ma_chau_insert").HasMaxLength(100).IsRequired();
        builder.Property(x => x.SoLuong).HasColumnName("so_luong").HasDefaultValue(1).IsRequired();
        builder.Property(x => x.GhiChu).HasColumnName("ghi_chu").HasMaxLength(500);

        builder.HasIndex(x => new { x.MaHangId, x.ChauInsertId })
            .IsUnique()
            .HasDatabaseName("UQ_md_bom_ma_hang_chau_insert_ma_hang_chau_insert");

        builder.HasOne(x => x.MaHangNavigation)
            .WithMany()
            .HasForeignKey(x => x.MaHangId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ChauInsert)
            .WithMany()
            .HasForeignKey(x => x.ChauInsertId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
