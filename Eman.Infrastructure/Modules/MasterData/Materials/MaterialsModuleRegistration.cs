using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Interfaces;
using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Services;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Interfaces;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Services;
using Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Services;
using Eman.Infrastructure.Repositories.MasterData.Materials.CoSoMuaVatTu;
using Eman.Infrastructure.Repositories.MasterData.Materials.NhomVatTu;
using Eman.Infrastructure.Repositories.MasterData.Materials.VatTu;
using Eman.Infrastructure.Services.MasterData.Materials.VatTu.Exports;
using Eman.Infrastructure.Services.MasterData.Materials.VatTu.Imports;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData.Materials;

internal static class MaterialsModuleRegistration
{
    public static IServiceCollection AddMaterialsModule(this IServiceCollection services)
    {
        services.AddScoped<INhomVatTuRepository, NhomVatTuRepository>();
        services.AddScoped<INhomVatTuService, NhomVatTuService>();
        services.AddScoped<ICoSoMuaVatTuRepository, CoSoMuaVatTuRepository>();
        services.AddScoped<ICoSoMuaVatTuService, CoSoMuaVatTuService>();
        services.AddScoped<IVatTuRepository, VatTuRepository>();
        services.AddScoped<IVatTuService, VatTuService>();
        services.AddScoped<VatTuImportTemplateBuilder>();
        services.AddScoped<IVatTuImportService, VatTuImportService>();
        services.AddScoped<IVatTuExportService, VatTuExportService>();
        return services;
    }
}
