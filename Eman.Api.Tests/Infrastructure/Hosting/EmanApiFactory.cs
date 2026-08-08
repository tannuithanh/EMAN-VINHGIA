using Eman.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Eman.Api.Tests.Infrastructure;

/// <summary>
/// Khởi động toàn bộ API EMAN với SQL Server kiểm thử cố định.
/// Chỉ cho phép kết nối đến database EmanMasterDataDb_Test.
/// </summary>
public sealed class EmanApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(
                    "appsettings.Testing.json",
                    optional: false,
                    reloadOnChange: false);
        });

        builder.ConfigureServices((context, services) =>
        {
            var connectionString = BaoVeCoSoDuLieuKiemThu.KiemTraVaChuanHoa(
                context.Configuration.GetConnectionString("EmanConnection"));

            services.RemoveAll<DbContextOptions<EmanDbContext>>();
            services.RemoveAll<EmanDbContext>();

            services.AddDbContext<EmanDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.CommandTimeout(120);
                        sqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    });
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmanDbContext>();
        DuLieuKiemThu.KhoiTaoAsync(dbContext).GetAwaiter().GetResult();

        return host;
    }
}
