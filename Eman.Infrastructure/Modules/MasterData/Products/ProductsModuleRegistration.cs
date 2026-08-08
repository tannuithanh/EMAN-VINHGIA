using Eman.Application.Modules.MasterData.Products.SanPham.Imports.Interfaces;
using Eman.Application.Modules.MasterData.Products.SanPham.Interfaces;
using Eman.Application.Modules.MasterData.Products.SanPham.Services;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Services;
using Eman.Infrastructure.Repositories.MasterData.Products.SanPham;
using Eman.Infrastructure.Repositories.MasterData.Products.ThueSanPham;
using Eman.Infrastructure.Services.MasterData.Products.SanPham.Imports;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData.Products;

internal static class ProductsModuleRegistration
{
    public static IServiceCollection AddProductsModule(this IServiceCollection services)
    {
        services.AddScoped<IThueSanPhamRepository, ThueSanPhamRepository>();
        services.AddScoped<IThueSanPhamService, ThueSanPhamService>();
        services.AddScoped<ISanPhamRepository, SanPhamRepository>();
        services.AddScoped<ISanPhamService, SanPhamService>();
        services.AddScoped<SanPhamImportTemplateBuilder>();
        services.AddScoped<ISanPhamImportService, SanPhamImportService>();
        return services;
    }
}
