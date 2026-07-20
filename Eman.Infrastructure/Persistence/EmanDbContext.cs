using Eman.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

/// <summary>
/// DbContext trung tâm của EMAN.
/// DbSet được tách theo từng module bằng partial class để file này không phình to.
/// </summary>
public sealed partial class EmanDbContext : DbContext, IUnitOfWork
{
    public EmanDbContext(DbContextOptions<EmanDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmanDbContext).Assembly);
    }
}
