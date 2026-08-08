using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Eman.Api.Tests.Infrastructure;

/// <summary>
/// Hàm dùng chung cho kiểm thử CRUD API B.O.M.
/// </summary>
public static class BomApiKiemThuHelper
{
    private static long _idHeSanPham = 10000;

    public static long TaoIdHeSanPham()
        => Interlocked.Increment(ref _idHeSanPham);

    public static string TaoMa(string tienTo)
    {
        var ma = $"{tienTo}-{Guid.NewGuid():N}".ToUpperInvariant();
        return ma[..Math.Min(tienTo.Length + 9, 30)];
    }

    public static async Task<JsonElement> TaoMoiAsync(
        HttpClient client,
        string route,
        object request,
        string thongDiepMongDoi)
    {
        using var response = await client.PostAsJsonAsync(route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.Created,
            thongDiepMongDoi);

        return await ApiKiemThuHelper.LayDataAsync(response);
    }

    public static async Task<JsonElement> LayTheoIdAsync(
        HttpClient client,
        string route,
        long id)
    {
        using var response = await client.GetAsync($"{route}/{id}");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.OK);

        return await ApiKiemThuHelper.LayDataAsync(response);
    }

    public static async Task<JsonElement> CapNhatAsync(
        HttpClient client,
        string route,
        long id,
        object request,
        string thongDiepMongDoi)
    {
        using var response = await client.PutAsJsonAsync($"{route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.OK,
            thongDiepMongDoi);

        return await ApiKiemThuHelper.LayDataAsync(response);
    }

    public static async Task XoaAsync(
        HttpClient client,
        string route,
        long id,
        string thongDiepMongDoi,
        string? rowVersion = null)
    {
        var url = rowVersion is null
            ? $"{route}/{id}"
            : $"{route}/{id}?rowVersion={Uri.EscapeDataString(rowVersion)}";

        using var response = await client.DeleteAsync(url);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.OK,
            thongDiepMongDoi);
    }

    public static async Task KiemTraDaXoaAsync(
        HttpClient client,
        string route,
        long id)
    {
        using var response = await client.GetAsync($"{route}/{id}");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.NotFound,
            "Không tìm thấy");
    }

    public static async Task KiemTraDanhSachAsync(
        HttpClient client,
        string route)
    {
        using var response = await client.GetAsync($"{route}?page=1&pageSize=20");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.OK);

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.True(data.TryGetProperty("items", out var items),
            "Phản hồi danh sách B.O.M không có thuộc tính items.");
        Assert.Equal(JsonValueKind.Array, items.ValueKind);
    }

    public static long LayId(JsonElement data)
        => data.GetProperty("id").GetInt64();

    public static string LayRowVersion(JsonElement data)
    {
        var rowVersion = data.GetProperty("rowVersion").GetString();
        Assert.False(string.IsNullOrWhiteSpace(rowVersion));
        return rowVersion!;
    }
}
