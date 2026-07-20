using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Interfaces;
using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Services;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Services;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Interfaces;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Services;
using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Interfaces;
using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Services;
using Eman.Infrastructure.Repositories.MasterData.BusinessPartners.BangGia;
using Eman.Infrastructure.Repositories.MasterData.BusinessPartners.DoiTacKinhDoanh;
using Eman.Infrastructure.Repositories.MasterData.BusinessPartners.LoaiDoiTac;
using Eman.Infrastructure.Repositories.MasterData.BusinessPartners.PhienBanBangGia;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.MasterData.BusinessPartners;

/// <summary>
/// Đăng ký phụ thuộc riêng cho module Đối tác kinh doanh thuộc Master Data.
/// </summary>
internal static class BusinessPartnersModuleRegistration
{
    public static IServiceCollection AddBusinessPartnersModule(this IServiceCollection services)
    {
        services.AddScoped<ILoaiDoiTacRepository, LoaiDoiTacRepository>();
        services.AddScoped<ILoaiDoiTacService, LoaiDoiTacService>();

        services.AddScoped<IDoiTacKinhDoanhRepository, DoiTacKinhDoanhRepository>();
        services.AddScoped<IDoiTacKinhDoanhService, DoiTacKinhDoanhService>();

        services.AddScoped<IBangGiaRepository, BangGiaRepository>();
        services.AddScoped<IBangGiaService, BangGiaService>();

        services.AddScoped<IPhienBanBangGiaRepository, PhienBanBangGiaRepository>();
        services.AddScoped<IPhienBanBangGiaService, PhienBanBangGiaService>();

        return services;
    }
}
