using System.Net;
using System.Text.Json;

namespace Eman.Api.Tests.Infrastructure;

public static class ApiKiemThuHelper
{
    public static async Task<string> DocNoiDungAsync(HttpResponseMessage response)
        => await response.Content.ReadAsStringAsync();

    public static async Task KiemTraTrangThaiVaThongDiepAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatusCode,
        string? expectedMessage = null)
    {
        var content = await DocNoiDungAsync(response);

        Assert.True(
            response.StatusCode == expectedStatusCode,
            $"Mong đợi {(int)expectedStatusCode} {expectedStatusCode} nhưng nhận " +
            $"{(int)response.StatusCode} {response.StatusCode}. Phản hồi: {content}");

        if (!string.IsNullOrWhiteSpace(expectedMessage))
        {
            Assert.Contains(expectedMessage, content, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static async Task<JsonElement> LayDataAsync(HttpResponseMessage response)
    {
        var content = await DocNoiDungAsync(response);
        using var document = JsonDocument.Parse(content);
        Assert.True(document.RootElement.TryGetProperty("data", out var data),
            $"Phản hồi không có thuộc tính data: {content}");
        return data.Clone();
    }
}
