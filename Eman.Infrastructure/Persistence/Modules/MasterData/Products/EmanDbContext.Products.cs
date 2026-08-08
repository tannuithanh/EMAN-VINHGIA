using Eman.Domain.Modules.MasterData.Products.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

/// <summary>
/// Các DbSet thuộc Master Data - Sản phẩm.
/// </summary>
public sealed partial class EmanDbContext
{
    public DbSet<ThueSanPham> ThueSanPhams => Set<ThueSanPham>();

    public DbSet<SanPham> SanPhams => Set<SanPham>();
}
