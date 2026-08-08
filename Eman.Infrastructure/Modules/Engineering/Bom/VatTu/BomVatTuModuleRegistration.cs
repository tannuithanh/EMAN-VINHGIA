using Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Interfaces;
using Eman.Application.Modules.Engineering.Bom.VatTu.Interfaces;
using Eman.Application.Modules.Engineering.Bom.VatTu.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.VatTu;
using Eman.Infrastructure.Services.Engineering.Bom.VatTu.Imports;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.Engineering.Bom.VatTu;

internal static class BomVatTuModuleRegistration
{
    public static IServiceCollection AddBomVatTuModule(this IServiceCollection services)
    {
        services.AddScoped<IBomVatTuRepository, BomVatTuRepository>();
        services.AddScoped<IBomVatTuService, BomVatTuService>();
        services.AddScoped<BomVatTuImportTemplateBuilder>();
        services.AddScoped<IBomVatTuImportService, BomVatTuImportService>();
        return services;
    }
}
