using Eman.Application.Common.Exceptions;
using Eman.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// =======================================
// 1) Controller + phản hồi validation tiếng Việt
// =======================================
builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(item => item.Value?.Errors.Count > 0)
                .ToDictionary(
                    item => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(item.Key),
                    item => item.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Giá trị không hợp lệ."
                            : error.ErrorMessage)
                        .Distinct(StringComparer.Ordinal)
                        .ToArray());

            var chiTietLoi = errors.Values
                .SelectMany(items => items)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            var message = chiTietLoi.Length == 0
                ? "Dữ liệu không hợp lệ."
                : $"Dữ liệu không hợp lệ: {string.Join(" ", chiTietLoi)}";

            return new BadRequestObjectResult(new
            {
                success = false,
                message,
                errors,
                traceId = context.HttpContext.TraceIdentifier
            });
        };
    });

// =======================================
// 2) Infrastructure: DbContext + Service
// =======================================
builder.Services.AddInfrastructure(builder.Configuration);

// =======================================
// 3) Swagger - tách riêng từng phần nghiệp vụ
// =======================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.SwaggerDoc("BusinessPartners", new OpenApiInfo
    {
        Title = "EMAN - MD - Đối tác kinh doanh",
        Version = "v1",
        Description = "API Loại đối tác, Điều kiện thanh toán, Điều kiện giao hàng, Đối tác kinh doanh, Bảng giá và Phiên bản bảng giá."
    });

    options.SwaggerDoc("Products", new OpenApiInfo
    {
        Title = "EMAN - MD - Sản phẩm",
        Version = "v1",
        Description = "API quản lý danh mục sản phẩm, tải mẫu, xem trước và import sản phẩm từ Excel."
    });

    options.SwaggerDoc("Materials", new OpenApiInfo
    {
        Title = "EMAN - MD - Vật tư",
        Version = "v1",
        Description = "API quản lý nhóm vật tư, cơ sở mua vật tư, danh mục vật tư, xuất Excel, tải mẫu, xem trước và import vật tư từ Excel."
    });

    options.SwaggerDoc("ProductTax", new OpenApiInfo
    {
        Title = "EMAN - MD - Thuế sản phẩm",
        Version = "v1",
        Description = "API quản lý danh mục thuế áp dụng cho sản phẩm."
    });

    options.SwaggerDoc("CapacityGroups", new OpenApiInfo
    {
        Title = "EMAN - MD - Nhóm năng lực",
        Version = "v1",
        Description = "API quản lý danh mục nhóm năng lực sản xuất."
    });

    options.SwaggerDoc("Warehouses", new OpenApiInfo
    {
        Title = "EMAN - MD - Kho",
        Version = "v1",
        Description = "API quản lý danh mục kho."
    });

    options.SwaggerDoc("Workshops", new OpenApiInfo
    {
        Title = "EMAN - MD - Phân xưởng",
        Version = "v1",
        Description = "API quản lý danh mục phân xưởng."
    });

    options.SwaggerDoc("UnitsOfMeasure", new OpenApiInfo
    {
        Title = "EMAN - MD - Đơn vị tính",
        Version = "v1",
        Description = "API quản lý danh mục đơn vị tính."
    });

    options.SwaggerDoc("BomCommon", new OpenApiInfo
    {
        Title = "EMAN - Engineering - B.O.M dùng chung",
        Version = "v1",
        Description = "API quản lý dữ liệu nền dùng chung cho B.O.M màu và B.O.M thô."
    });

    options.SwaggerDoc("BomColor", new OpenApiInfo
    {
        Title = "EMAN - Engineering - B.O.M màu",
        Version = "v1",
        Description = "API quản lý cấu hình và công thức riêng của B.O.M màu."
    });

    options.SwaggerDoc("BomCalculations", new OpenApiInfo
    {
        Title = "EMAN - Engineering - Tính toán B.O.M",
        Version = "v1",
        Description = "API tính thử, chẩn đoán và triển khai các bộ máy tính B.O.M màu, B.O.M thô trong tương lai."
    });

    options.SwaggerDoc("BomMaterial", new OpenApiInfo
    {
        Title = "EMAN - Engineering - B.O.M vật tư",
        Version = "v1",
        Description = "API quản lý phiên bản và thành phần B.O.M vật tư nhiều cấp."
    });

    options.DocInclusionPredicate((documentName, apiDescription) =>
        string.Equals(
            apiDescription.GroupName,
            documentName,
            StringComparison.OrdinalIgnoreCase));

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// =======================================
// 4) CORS
// =======================================
const string frontendCorsPolicy = "FrontendOnly";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// =======================================
// 5) Xử lý lỗi toàn cục
// =======================================
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = feature?.Error;

        var statusCode = exception switch
        {
            KhongTimThayException => StatusCodes.Status404NotFound,
            XungDotDuLieuException => StatusCodes.Status409Conflict,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            QuyTacNghiepVuException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            DbUpdateException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var message = exception switch
        {
            KhongTimThayException => exception.Message,
            XungDotDuLieuException => exception.Message,
            DbUpdateConcurrencyException =>
                "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại dữ liệu trước khi thao tác.",
            QuyTacNghiepVuException => exception.Message,
            InvalidOperationException => exception.Message,
            DbUpdateException => "Không thể lưu dữ liệu. Vui lòng kiểm tra ràng buộc dữ liệu.",
            _ => "Có lỗi hệ thống."
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message,
            errors = new Dictionary<string, string[]>
            {
                ["request"] = [message]
            },
            traceId = context.TraceIdentifier
        });
    });
});

// =======================================
// 6) Middleware
// =======================================
var swaggerEnabled = app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/BusinessPartners/swagger.json",
            "MD - Đối tác kinh doanh");
        options.SwaggerEndpoint(
            "/swagger/Products/swagger.json",
            "MD - Sản phẩm");
        options.SwaggerEndpoint(
            "/swagger/Materials/swagger.json",
            "MD - Vật tư");
        options.SwaggerEndpoint(
            "/swagger/ProductTax/swagger.json",
            "MD - Thuế sản phẩm");
        options.SwaggerEndpoint(
            "/swagger/CapacityGroups/swagger.json",
            "MD - Nhóm năng lực");
        options.SwaggerEndpoint(
            "/swagger/Warehouses/swagger.json",
            "MD - Kho");
        options.SwaggerEndpoint(
            "/swagger/Workshops/swagger.json",
            "MD - Phân xưởng");
        options.SwaggerEndpoint(
            "/swagger/UnitsOfMeasure/swagger.json",
            "MD - Đơn vị tính");
        options.SwaggerEndpoint(
            "/swagger/BomCommon/swagger.json",
            "Engineering - B.O.M dùng chung");
        options.SwaggerEndpoint(
            "/swagger/BomColor/swagger.json",
            "Engineering - B.O.M màu");
        options.SwaggerEndpoint(
            "/swagger/BomCalculations/swagger.json",
            "Engineering - Tính toán B.O.M");
        options.SwaggerEndpoint(
            "/swagger/BomMaterial/swagger.json",
            "Engineering - B.O.M vật tư");
        options.DocumentTitle = "EMAN API";
        options.RoutePrefix = "swagger";
        options.EnableFilter();
        options.DisplayRequestDuration();
    });
}

app.UseCors(frontendCorsPolicy);
app.MapControllers();

app.Run();

// Cho phép project kiểm thử khởi động API bằng WebApplicationFactory.
public partial class Program
{
}
