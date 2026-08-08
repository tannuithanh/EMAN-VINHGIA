using Eman.Domain.Modules.MasterData.Production.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

/// <summary>
/// Các DbSet thuộc Master Data - Sản xuất.
/// </summary>
public sealed partial class EmanDbContext
{
    public DbSet<NhomNangLuc> NhomNangLucs => Set<NhomNangLuc>();

    public DbSet<PhanXuong> PhanXuongs => Set<PhanXuong>();
}
