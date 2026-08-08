using Eman.Api.Tests.Infrastructure;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Eman.Api.Tests.Shared;

/// <summary>
/// Bộ kiểm tra chung tự động áp dụng cho mọi endpoint hiện tại và endpoint mới.
/// Chỉ cần endpoint xuất hiện trong ApiExplorer thì bộ kiểm tra này sẽ tự nhận diện.
/// </summary>
public sealed class KiemTraEndpointTuDongTests(
    EmanApiFactory factory,
    ITestOutputHelper output) : IClassFixture<EmanApiFactory>
{
    [Fact(DisplayName = "Tất cả endpoint phải được phát hiện và không bị trùng route")]
    public void TatCaEndpoint_PhaiDuocPhatHienVaKhongTrungRoute()
    {
        var endpoints = LayTatCaEndpoint();

        Assert.NotEmpty(endpoints);

        var endpointBiTrung = endpoints
            .GroupBy(endpoint => new
            {
                Method = endpoint.HttpMethod?.ToUpperInvariant(),
                Path = EndpointTestHelper.ChuanHoaSwaggerPath(endpoint)
            })
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key.Method} {group.Key.Path}")
            .ToArray();

        Assert.True(
            endpointBiTrung.Length == 0,
            $"Phát hiện endpoint bị trùng:{Environment.NewLine}{string.Join(Environment.NewLine, endpointBiTrung)}");

        output.WriteLine($"Đã phát hiện {endpoints.Count} endpoint.");
    }

    [Fact(DisplayName = "Tất cả endpoint phải có nhóm Swagger")]
    public void TatCaEndpoint_PhaiCoNhomSwagger()
    {
        var endpointsKhongCoNhom = LayTatCaEndpoint()
            .Where(endpoint => string.IsNullOrWhiteSpace(endpoint.GroupName))
            .Select(endpoint => $"{endpoint.HttpMethod} /{endpoint.RelativePath}")
            .ToArray();

        Assert.True(
            endpointsKhongCoNhom.Length == 0,
            $"Các endpoint sau chưa có ApiExplorerSettings(GroupName):{Environment.NewLine}" +
            string.Join(Environment.NewLine, endpointsKhongCoNhom));
    }

    [Fact(DisplayName = "Mọi endpoint phải có mặt trong tài liệu Swagger")]
    public async Task TatCaEndpoint_PhaiCoTrongSwagger()
    {
        using var client = factory.CreateClient();
        var endpoints = LayTatCaEndpoint();
        var loi = new List<string>();

        foreach (var group in endpoints.GroupBy(endpoint => endpoint.GroupName!))
        {
            using var response = await client.GetAsync($"/swagger/{group.Key}/swagger.json");

            if (!response.IsSuccessStatusCode)
            {
                loi.Add($"Không tải được Swagger nhóm '{group.Key}': {(int)response.StatusCode} {response.StatusCode}");
                continue;
            }

            using var document = System.Text.Json.JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            if (!document.RootElement.TryGetProperty("paths", out var paths))
            {
                loi.Add($"Swagger nhóm '{group.Key}' không có phần paths.");
                continue;
            }

            foreach (var endpoint in group)
            {
                var path = EndpointTestHelper.ChuanHoaSwaggerPath(endpoint);
                var method = endpoint.HttpMethod!.ToLowerInvariant();

                if (!paths.TryGetProperty(path, out var pathItem)
                    || !pathItem.TryGetProperty(method, out _))
                {
                    loi.Add($"Thiếu trong Swagger: {endpoint.HttpMethod} {path} - nhóm {group.Key}");
                }
            }
        }

        Assert.True(
            loi.Count == 0,
            $"Phát hiện lỗi tài liệu Swagger:{Environment.NewLine}{string.Join(Environment.NewLine, loi)}");
    }


    [Fact(DisplayName = "Endpoint có body bắt buộc phải từ chối body rỗng")]
    public async Task EndpointCoBodyBatBuoc_BodyRongPhaiTra400()
    {
        using var client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });

        var endpoints = LayTatCaEndpoint()
            .Where(endpoint => endpoint.ParameterDescriptions.Any(parameter =>
                parameter.Source == BindingSource.Body
                && parameter.ModelMetadata?.ModelType
                    .GetProperties()
                    .Any(property => property.GetCustomAttributes(
                        typeof(RequiredAttribute), inherit: true).Length > 0) == true))
            .ToArray();

        var loi = new List<string>();

        foreach (var endpoint in endpoints)
        {
            using var request = EndpointTestHelper.TaoRequestKiemTra(endpoint);
            using var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                continue;
            }

            var content = await response.Content.ReadAsStringAsync();
            loi.Add(
                $"{request.Method} {request.RequestUri} phải trả 400 khi body rỗng " +
                $"nhưng nhận {(int)response.StatusCode} {response.StatusCode}. Phản hồi: {content}");
        }

        Assert.True(loi.Count == 0,
            $"Các endpoint sau không chặn body thiếu trường bắt buộc:{Environment.NewLine}" +
            string.Join(Environment.NewLine, loi));
    }

    [Fact(DisplayName = "Bộ request chung không được làm endpoint trả lỗi 500")]
    public async Task TatCaEndpoint_KhongDuocTraVeLoiMayChu()
    {
        using var client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });

        var endpoints = LayTatCaEndpoint();
        var loi = new List<string>();

        foreach (var endpoint in endpoints)
        {
            using var request = EndpointTestHelper.TaoRequestKiemTra(endpoint);
            using var response = await client.SendAsync(request);

            output.WriteLine(
                $"{request.Method} {request.RequestUri} -> {(int)response.StatusCode} {response.StatusCode}");

            if ((int)response.StatusCode < StatusCodes.Status500InternalServerError)
            {
                continue;
            }

            var content = await response.Content.ReadAsStringAsync();
            var noiDungRutGon = content.Length > 500
                ? content[..500]
                : content;

            loi.Add(
                $"{request.Method} {request.RequestUri} -> {(int)response.StatusCode} {response.StatusCode}. " +
                $"Phản hồi: {noiDungRutGon}");
        }

        Assert.True(
            loi.Count == 0,
            $"Các endpoint sau trả lỗi máy chủ:{Environment.NewLine}{string.Join(Environment.NewLine, loi)}");
    }

    private IReadOnlyList<ApiDescription> LayTatCaEndpoint()
    {
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider
            .GetRequiredService<IApiDescriptionGroupCollectionProvider>();

        return provider.ApiDescriptionGroups.Items
            .SelectMany(group => group.Items)
            .Where(endpoint => !string.IsNullOrWhiteSpace(endpoint.HttpMethod))
            .OrderBy(endpoint => endpoint.GroupName)
            .ThenBy(endpoint => endpoint.RelativePath)
            .ThenBy(endpoint => endpoint.HttpMethod)
            .ToArray();
    }
}
