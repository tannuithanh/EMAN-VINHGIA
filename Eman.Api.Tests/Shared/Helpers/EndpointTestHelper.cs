using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Eman.Api.Tests.Shared;

internal static partial class EndpointTestHelper
{
    private static readonly Guid GuidKhongTonTai =
        Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

    public static HttpRequestMessage TaoRequestKiemTra(ApiDescription api)
    {
        var method = new HttpMethod(api.HttpMethod
            ?? throw new InvalidOperationException("Endpoint không khai báo HTTP method."));

        var path = TaoDuongDan(api);
        var request = new HttpRequestMessage(method, path);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (CoDuLieuForm(api))
        {
            request.Content = new MultipartFormDataContent();
        }
        else if (CoRequestBody(api))
        {
            request.Content = new StringContent(
                "{}",
                Encoding.UTF8,
                "application/json");
        }

        return request;
    }

    public static string ChuanHoaSwaggerPath(ApiDescription api)
    {
        var relativePath = api.RelativePath
            ?? throw new InvalidOperationException("Endpoint không có đường dẫn tương đối.");

        var path = relativePath.Split('?', 2)[0];
        path = RouteConstraintRegex().Replace(path, "{$1}");
        return path.StartsWith('/') ? path : $"/{path}";
    }

    private static string TaoDuongDan(ApiDescription api)
    {
        var relativePath = api.RelativePath
            ?? throw new InvalidOperationException("Endpoint không có đường dẫn tương đối.");

        var path = relativePath.Split('?', 2)[0];

        path = RouteParameterRegex().Replace(path, match =>
        {
            var parameterName = match.Groups[1].Value;
            var parameter = api.ParameterDescriptions
                .FirstOrDefault(item =>
                    string.Equals(item.Name, parameterName, StringComparison.OrdinalIgnoreCase));

            return TaoGiaTriRoute(parameter?.ModelMetadata?.ModelType);
        });

        return path.StartsWith('/') ? path : $"/{path}";
    }

    private static string TaoGiaTriRoute(Type? modelType)
    {
        var type = Nullable.GetUnderlyingType(modelType ?? typeof(string))
            ?? modelType
            ?? typeof(string);

        if (type == typeof(Guid))
        {
            return GuidKhongTonTai.ToString();
        }

        if (type == typeof(int) || type == typeof(short) || type == typeof(byte))
        {
            return "2147483647";
        }

        if (type == typeof(long))
        {
            return "9223372036854775807";
        }

        if (type == typeof(DateTime) || type == typeof(DateOnly))
        {
            return "2099-12-31";
        }

        return "du-lieu-kiem-thu";
    }

    private static bool CoRequestBody(ApiDescription api)
        => api.ParameterDescriptions.Any(parameter =>
            parameter.Source == BindingSource.Body);

    private static bool CoDuLieuForm(ApiDescription api)
        => api.ParameterDescriptions.Any(parameter =>
            parameter.Source == BindingSource.Form);

    [GeneratedRegex(@"\{([^}:?]+)(?:[:?][^}]*)?\}")]
    private static partial Regex RouteParameterRegex();

    [GeneratedRegex(@"\{([^}:?]+)(?:[:?][^}]*)?\}")]
    private static partial Regex RouteConstraintRegex();
}
