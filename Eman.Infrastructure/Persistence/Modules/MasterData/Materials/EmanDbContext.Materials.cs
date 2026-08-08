using Eman.Domain.Modules.MasterData.Materials.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

/// <summary>
/// Các DbSet thuộc Master Data - Vật tư.
/// </summary>
public sealed partial class EmanDbContext
{
    public DbSet<NhomVatTu> NhomVatTus => Set<NhomVatTu>();
    public DbSet<CoSoMuaVatTu> CoSoMuaVatTus => Set<CoSoMuaVatTu>();
    public DbSet<VatTu> VatTus => Set<VatTu>();
    public DbSet<VatTuPhanXuong> VatTuPhanXuongs => Set<VatTuPhanXuong>();
}
