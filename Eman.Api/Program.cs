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
                    item => item.Key,
                    item => item.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Giá trị không hợp lệ."
                            : error.ErrorMessage)
                        .ToArray());

            return new BadRequestObjectResult(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ.",
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
// 3) Swagger
// =======================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();

    options.SwaggerDoc("MasterData", new OpenApiInfo
    {
        Title = "EMAN Master Data API",
        Version = "v1",
        Description = "API dữ liệu gốc của hệ thống EMAN, ánh xạ các bảng md_*."
    });

    options.SwaggerDoc("System", new OpenApiInfo
    {
        Title = "EMAN System API",
        Version = "v1",
        Description = "API kiểm tra và vận hành hệ thống EMAN."
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
            "/swagger/MasterData/swagger.json",
            "EMAN Master Data API v1");
        options.SwaggerEndpoint(
            "/swagger/System/swagger.json",
            "EMAN System API v1");
        options.DocumentTitle = "EMAN API";
        options.RoutePrefix = "swagger";
    });
}

app.UseCors(frontendCorsPolicy);
app.MapControllers();

app.Run();
