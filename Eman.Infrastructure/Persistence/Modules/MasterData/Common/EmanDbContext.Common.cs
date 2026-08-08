using Eman.Domain.Modules.MasterData.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

/// <summary>
/// Các DbSet dùng chung thuộc Master Data.
/// </summary>
public sealed partial class EmanDbContext
{
    public DbSet<DonViTinh> DonViTinhs => Set<DonViTinh>();
}
