using Eman.Infrastructure.Modules.MasterData.BusinessPartners;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData;

/// <summary>
/// Điểm đăng ký tập trung cho toàn bộ module dữ liệu gốc (các bảng md_*).
/// Khi bổ sung Sản phẩm, Vật tư, Kho, Phân xưởng... chỉ đăng ký tại đây.
/// </summary>
internal static class MasterDataModuleRegistration
{
    public static IServiceCollection AddMasterDataModules(this IServiceCollection services)
    {
        services.AddBusinessPartnersModule();
        return services;
    }
}
