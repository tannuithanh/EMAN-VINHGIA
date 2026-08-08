using Eman.Api.Tests.Infrastructure;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Api.Tests.Shared.Database;

/// <summary>
/// Xác nhận bộ test đang dùng SQL Server và đúng database kiểm thử.
/// </summary>
public sealed class KiemTraKetNoiSqlServerTests(EmanApiFactory factory)
    : IClassFixture<EmanApiFactory>
{
    [Fact]
    [Trait("Phân hệ", "Hệ thống kiểm thử")]
    [Trait("Module", "SQL Server")]
    public async Task PhaiKetNoiDungDatabaseSqlServerKiemThu()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmanDbContext>();

        Assert.True(
            dbContext.Database.IsSqlServer(),
            "DbContext không sử dụng SQL Server.");

        var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT DB_NAME()";

            var databaseName = Convert.ToString(await command.ExecuteScalarAsync());

            Assert.Equal(
                BaoVeCoSoDuLieuKiemThu.TenCoSoDuLieuChoPhep,
                databaseName);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    [Fact]
    [Trait("Phân hệ", "Hệ thống kiểm thử")]
    [Trait("Module", "SQL Server")]
    public async Task DuLieuNgungHoatDong_PhaiDuocLuuDungGiaTriKhong()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmanDbContext>();

        var nhomVatTu = await dbContext.NhomVatTus
            .AsNoTracking()
            .SingleAsync(item => item.Id == DuLieuKiemThu.NhomVatTuNgungId);
        var coSoMuaVatTu = await dbContext.CoSoMuaVatTus
            .AsNoTracking()
            .SingleAsync(item => item.Id == DuLieuKiemThu.CoSoMuaNgungId);

        Assert.Equal(TrangThaiHoatDong.NgungHoatDong, nhomVatTu.TrangThai);
        Assert.Equal(TrangThaiHoatDong.NgungHoatDong, coSoMuaVatTu.TrangThai);
    }
}
