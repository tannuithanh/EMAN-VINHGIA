using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.HeSanPham;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.DeTai;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.MauSac;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.HinhDang;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.MaHang;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.NhomM;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Services;
using Eman.Infrastructure.Repositories.Engineering.Bom.DungChung.QuyTacNhomM;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure.Modules.Engineering.Bom.DungChung;

internal static class BomDungChungModuleRegistration
{
    public static IServiceCollection AddBomDungChungModule(this IServiceCollection services)
    {
        services.AddScoped<IHeSanPhamRepository, HeSanPhamRepository>();
        services.AddScoped<IHeSanPhamService, HeSanPhamService>();
        services.AddScoped<IDeTaiRepository, DeTaiRepository>();
        services.AddScoped<IDeTaiService, DeTaiService>();
        services.AddScoped<IMauSacRepository, MauSacRepository>();
        services.AddScoped<IMauSacService, MauSacService>();
        services.AddScoped<IHinhDangRepository, HinhDangRepository>();
        services.AddScoped<IHinhDangService, HinhDangService>();
        services.AddScoped<IMaHangRepository, MaHangRepository>();
        services.AddScoped<IMaHangService, MaHangService>();
        services.AddScoped<INhomMRepository, NhomMRepository>();
        services.AddScoped<INhomMService, NhomMService>();
        services.AddScoped<IQuyTacNhomMRepository, QuyTacNhomMRepository>();
        services.AddScoped<IQuyTacNhomMService, QuyTacNhomMService>();
        return services;
    }
}
