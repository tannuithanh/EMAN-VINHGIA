using Eman.Domain.Modules.MasterData.Inventory.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

/// <summary>
/// Các DbSet thuộc Master Data - Kho vận.
/// </summary>
public sealed partial class EmanDbContext
{
    public DbSet<Kho> Khos => Set<Kho>();
}
