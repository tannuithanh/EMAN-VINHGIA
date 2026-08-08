using Eman.Infrastructure.Modules.Engineering.Bom.DungChung;
using Eman.Infrastructure.Modules.Engineering.Bom.Mau;
using Eman.Infrastructure.Modules.Engineering.Bom.TinhToan;
using Eman.Infrastructure.Modules.Engineering.Bom.VatTu;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.Engineering;

internal static class EngineeringModuleRegistration
{
    public static IServiceCollection AddEngineeringModules(this IServiceCollection services)
    {
        services.AddBomDungChungModule();
        services.AddBomMauModule();
        services.AddBomTinhToanModule();
        services.AddBomVatTuModule();
        return services;
    }
}
