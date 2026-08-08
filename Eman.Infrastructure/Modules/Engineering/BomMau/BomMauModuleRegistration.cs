using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauBuoc;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BuocNhomTheoMau;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauDinhMucNhomM;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauHeSoDeTai;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMauHeSoMau;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMaHangPhen;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.ChauInsert;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.Mau.BomMaHangChauInsert;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.Engineering.Bom.Mau;

internal static class BomMauModuleRegistration
{
    public static IServiceCollection AddBomMauModule(this IServiceCollection services)
    {
        services.AddScoped<IBomMauBuocRepository, BomMauBuocRepository>();
        services.AddScoped<IBomMauBuocService, BomMauBuocService>();
        services.AddScoped<IBuocNhomTheoMauRepository, BuocNhomTheoMauRepository>();
        services.AddScoped<IBuocNhomTheoMauService, BuocNhomTheoMauService>();
        services.AddScoped<IBomMauDinhMucNhomMRepository, BomMauDinhMucNhomMRepository>();
        services.AddScoped<IBomMauDinhMucNhomMService, BomMauDinhMucNhomMService>();
        services.AddScoped<IBomMauHeSoDeTaiRepository, BomMauHeSoDeTaiRepository>();
        services.AddScoped<IBomMauHeSoDeTaiService, BomMauHeSoDeTaiService>();
        services.AddScoped<IBomMauHeSoMauRepository, BomMauHeSoMauRepository>();
        services.AddScoped<IBomMauHeSoMauService, BomMauHeSoMauService>();
        services.AddScoped<IBomMaHangPhenRepository, BomMaHangPhenRepository>();
        services.AddScoped<IBomMaHangPhenService, BomMaHangPhenService>();
        services.AddScoped<IChauInsertRepository, ChauInsertRepository>();
        services.AddScoped<IChauInsertService, ChauInsertService>();
        services.AddScoped<IBomMaHangChauInsertRepository, BomMaHangChauInsertRepository>();
        services.AddScoped<IBomMaHangChauInsertService, BomMaHangChauInsertService>();
        return services;
    }
}
