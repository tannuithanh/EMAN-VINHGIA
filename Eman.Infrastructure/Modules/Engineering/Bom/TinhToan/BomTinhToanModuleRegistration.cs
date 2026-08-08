using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Interfaces;
using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.TinhToan.Mau;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.Engineering.Bom.TinhToan;

internal static class BomTinhToanModuleRegistration
{
    public static IServiceCollection AddBomTinhToanModule(
        this IServiceCollection services)
    {
        services.AddScoped<ITraCuuTinhBomMauRepository, TraCuuTinhBomMauRepository>();
        services.AddScoped<ITinhBomMauService, TinhBomMauService>();
        return services;
    }
}
