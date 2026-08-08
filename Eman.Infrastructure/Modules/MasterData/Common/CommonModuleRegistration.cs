using Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Services;
using Eman.Infrastructure.Repositories.MasterData.Common.DonViTinh;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData.Common;

internal static class CommonModuleRegistration
{
    public static IServiceCollection AddCommonModule(this IServiceCollection services)
    {
        services.AddScoped<IDonViTinhRepository, DonViTinhRepository>();
        services.AddScoped<IDonViTinhService, DonViTinhService>();
        return services;
    }
}
