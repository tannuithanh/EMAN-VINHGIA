using LoaiDoiTacEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.LoaiDoiTac;
using Eman.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eman.Infrastructure.Persistence.Configurations.MasterData.BusinessPartners.LoaiDoiTac;

public sealed class LoaiDoiTacConfiguration : IEntityTypeConfiguration<LoaiDoiTacEntity>
{
    public void Configure(EntityTypeBuilder<LoaiDoiTacEntity> builder)
    {
        builder.ToTable("md_loai_doi_tac", "dbo");
        builder.CauHinhBaseEntity();

        builder.Property(entity => entity.MaLoaiDoiTac)
            .HasColumnName("ma_loai_doi_tac")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.TenLoaiDoiTac)
            .HasColumnName("ten_loai_doi_tac")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.TrangThai)
            .HasColumnName("trang_thai")
            .HasConversion<byte>()
            .IsRequired();

        builder.HasIndex(entity => entity.MaLoaiDoiTac)
            .IsUnique()
            .HasDatabaseName("UQ_md_loai_doi_tac_ma");
    }
}
