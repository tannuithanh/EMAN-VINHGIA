using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Eman.Api.Tests.Infrastructure;

/// <summary>
/// Mô phỏng rowversion của SQL Server khi dùng EF Core InMemory trong kiểm thử.
/// </summary>
public sealed class RowVersionInMemoryInterceptor : SaveChangesInterceptor
{
    private long _version = DateTime.UtcNow.Ticks;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        CapNhatRowVersion(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CapNhatRowVersion(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void CapNhatRowVersion(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(item => item.State is EntityState.Added or EntityState.Modified))
        {
            var property = entry.Properties.FirstOrDefault(item =>
                string.Equals(item.Metadata.Name, "RowVersion", StringComparison.Ordinal));

            if (property is null)
            {
                continue;
            }

            property.CurrentValue = BitConverter.GetBytes(Interlocked.Increment(ref _version));
            if (entry.State == EntityState.Modified)
            {
                property.IsModified = true;
            }
        }
    }
}
