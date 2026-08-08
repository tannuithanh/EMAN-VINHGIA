using Microsoft.Data.SqlClient;

namespace Eman.Api.Tests.Infrastructure;

/// <summary>
/// Chặn bộ kiểm thử kết nối nhầm vào cơ sở dữ liệu nghiệp vụ thật.
/// </summary>
public static class BaoVeCoSoDuLieuKiemThu
{
    public const string TenCoSoDuLieuChoPhep = "EmanMasterDataDb_Test";

    public static string KiemTraVaChuanHoa(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Thiếu ConnectionStrings:EmanConnection trong appsettings.Testing.json của project Eman.Api.Tests.");
        }

        SqlConnectionStringBuilder builder;
        try
        {
            builder = new SqlConnectionStringBuilder(connectionString);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(
                "Connection string SQL Server kiểm thử không hợp lệ.",
                exception);
        }

        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            throw new InvalidOperationException(
                "Connection string kiểm thử chưa khai báo Server/Data Source.");
        }

        if (!string.Equals(
                builder.InitialCatalog,
                TenCoSoDuLieuChoPhep,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Đã chặn chạy test vì database phải là '{TenCoSoDuLieuChoPhep}', " +
                $"nhưng connection string đang trỏ tới '{builder.InitialCatalog}'.");
        }

        return builder.ConnectionString;
    }
}
