using Eman.Infrastructure.Modules.MasterData.BusinessPartners;
using Eman.Infrastructure.Modules.MasterData.Common;
using Eman.Infrastructure.Modules.MasterData.Inventory;
using Eman.Infrastructure.Modules.MasterData.Materials;
using Eman.Infrastructure.Modules.MasterData.Products;
using Eman.Infrastructure.Modules.MasterData.Production;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData;

/// <summary>
/// Điểm đăng ký tập trung cho toàn bộ module dữ liệu gốc (các bảng md_*).
/// </summary>
internal static class MasterDataModuleRegistration
{
    public static IServiceCollection AddMasterDataModules(this IServiceCollection services)
    {
        services.AddBusinessPartnersModule();
        services.AddCommonModule();
        services.AddInventoryModule();
        services.AddMaterialsModule();
        services.AddProductsModule();
        services.AddProductionModule();
        return services;
    }
}
