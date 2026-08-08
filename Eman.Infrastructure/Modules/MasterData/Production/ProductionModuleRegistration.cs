using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Interfaces;
using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Services;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Interfaces;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Services;
using Eman.Infrastructure.Repositories.MasterData.Production.NhomNangLuc;
using Eman.Infrastructure.Repositories.MasterData.Production.PhanXuong;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData.Production;

internal static class ProductionModuleRegistration
{
    public static IServiceCollection AddProductionModule(this IServiceCollection services)
    {
        services.AddScoped<INhomNangLucRepository, NhomNangLucRepository>();
        services.AddScoped<INhomNangLucService, NhomNangLucService>();
        services.AddScoped<IPhanXuongRepository, PhanXuongRepository>();
        services.AddScoped<IPhanXuongService, PhanXuongService>();
        return services;
    }
}
