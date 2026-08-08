using Eman.Application.Common.Persistence;
using Eman.Infrastructure.Modules.Engineering;
using Eman.Infrastructure.Modules.MasterData;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EmanConnection")
            ?? throw new InvalidOperationException(
                "Thiếu cấu hình ConnectionStrings:EmanConnection.");

        services.AddDbContext<EmanDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<EmanDbContext>());

        services.AddMasterDataModules();
        services.AddEngineeringModules();
        return services;
    }
}
