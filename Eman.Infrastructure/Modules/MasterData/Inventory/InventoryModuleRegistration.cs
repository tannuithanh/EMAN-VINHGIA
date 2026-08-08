using Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;
using Eman.Application.Modules.MasterData.Inventory.Kho.Services;
using Eman.Infrastructure.Repositories.MasterData.Inventory.Kho;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData.Inventory;

internal static class InventoryModuleRegistration
{
    public static IServiceCollection AddInventoryModule(this IServiceCollection services)
    {
        services.AddScoped<IKhoRepository, KhoRepository>();
        services.AddScoped<IKhoService, KhoService>();
        return services;
    }
}
