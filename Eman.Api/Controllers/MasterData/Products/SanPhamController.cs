using Eman.Api.Common.Routing;
using Eman.Api.Contracts.MasterData.Products.Imports;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Products.SanPham.Dtos;
using Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;
using Eman.Application.Modules.MasterData.Products.SanPham.Imports.Interfaces;
using Eman.Application.Modules.MasterData.Products.SanPham.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Products;

[ApiController]
[Route(ApiRoutes.MasterData + "/san-pham")]
[ApiExplorerSettings(GroupName = "Products")]
[SwaggerTag("Quản lý danh mục sản phẩm EMAN, tải mẫu và import sản phẩm từ Excel.")]
public sealed class SanPhamController(
    ISanPhamService service,
    ISanPhamImportService importService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách sản phẩm")]
    public async Task<ActionResult<ApiResponse<PagedResult<SanPhamDto>>>> LayDanhSach(
        [FromQuery] BoLocSanPhamRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<SanPhamDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết sản phẩm")]
    public async Task<ActionResult<ApiResponse<SanPhamDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<SanPhamDto>.Ok(
            await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo sản phẩm")]
    public async Task<ActionResult<ApiResponse<SanPhamDto>>> TaoMoi(
        [FromBody] TaoSanPhamRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<SanPhamDto>.Ok(data, "Tạo sản phẩm thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật sản phẩm")]
    public async Task<ActionResult<ApiResponse<SanPhamDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatSanPhamRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<SanPhamDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật sản phẩm thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa sản phẩm")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa sản phẩm thành công."));
    }

    [HttpGet("import/template")]
    [SwaggerOperation(Summary = "Tải file mẫu import sản phẩm")]
    public async Task<IActionResult> TaiTemplateImport(
        CancellationToken cancellationToken)
    {
        var file = await importService.TaoTemplateAsync(cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost("import/preview")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Xem trước và kiểm tra lỗi file import sản phẩm")]
    public async Task<ActionResult<ApiResponse<SanPhamImportPreviewDto>>> XemTruocImport(
        [FromForm] SanPhamImportFileRequest request,
        CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();
        var data = await importService.XemTruocAsync(
            stream,
            request.File.FileName,
            request.File.Length,
            cancellationToken);

        var message = data.TongSoDong == 0
            ? "File import không có dòng dữ liệu sản phẩm."
            : data.SoDongLoi == 0
                ? $"File hợp lệ, có thể import {data.SoDongHopLe} sản phẩm."
                : data.SoDongHopLe > 0
                    ? $"Có {data.SoDongHopLe} dòng hợp lệ và {data.SoDongLoi} dòng lỗi. Khi import, hệ thống sẽ tự bỏ qua các dòng lỗi."
                    : $"Không có dòng hợp lệ để import. File có {data.SoDongLoi} dòng lỗi.";

        return Ok(ApiResponse<SanPhamImportPreviewDto>.Ok(data, message));
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Import chính thức sản phẩm vào md_san_pham")]
    public async Task<ActionResult<ApiResponse<SanPhamImportResultDto>>> Import(
        [FromForm] SanPhamImportFileRequest request,
        CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();
        var data = await importService.ImportAsync(
            stream,
            request.File.FileName,
            request.File.Length,
            request.CreatedByMsnv,
            cancellationToken);

        if (!data.ThanhCong)
        {
            return BadRequest(ApiResponse<SanPhamImportResultDto>.Fail(
                data.Message,
                data));
        }

        return Ok(ApiResponse<SanPhamImportResultDto>.Ok(data, data.Message));
    }
}
