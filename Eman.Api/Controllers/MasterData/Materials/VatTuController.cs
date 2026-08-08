using Eman.Api.Common.Routing;
using Eman.Api.Contracts.MasterData.Materials.Imports;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;
using Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Dtos;
using Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;
using Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Materials;

[ApiController]
[Route(ApiRoutes.MasterData + "/vat-tu")]
[ApiExplorerSettings(GroupName = "Materials")]
[SwaggerTag("Quản lý danh mục vật tư, xuất Excel, tải mẫu, xem trước và import vật tư từ Excel.")]
public sealed class VatTuController(
    IVatTuService service,
    IVatTuImportService importService,
    IVatTuExportService exportService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách vật tư")]
    public async Task<ActionResult<ApiResponse<PagedResult<VatTuDto>>>> LayDanhSach(
        [FromQuery] BoLocVatTuRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<VatTuDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết vật tư")]
    public async Task<ActionResult<ApiResponse<VatTuDto>>> LayTheoId(Guid id, CancellationToken cancellationToken)
        => Ok(ApiResponse<VatTuDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo vật tư")]
    public async Task<ActionResult<ApiResponse<VatTuDto>>> TaoMoi(
        [FromBody] TaoVatTuRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id },
            ApiResponse<VatTuDto>.Ok(data, "Tạo vật tư thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật vật tư")]
    public async Task<ActionResult<ApiResponse<VatTuDto>>> CapNhat(
        Guid id, [FromBody] CapNhatVatTuRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<VatTuDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật vật tư thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa vật tư")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa vật tư thành công."));
    }

    [HttpGet("export")]
    [SwaggerOperation(Summary = "Xuất danh sách vật tư ra Excel theo bộ lọc")]
    public async Task<IActionResult> XuatExcel(
        [FromQuery] BoLocXuatVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var file = await exportService.XuatExcelAsync(request, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("import/template")]
    [SwaggerOperation(Summary = "Tải file mẫu import vật tư")]
    public async Task<IActionResult> TaiTemplateImport(CancellationToken cancellationToken)
    {
        var file = await importService.TaoTemplateAsync(cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost("import/preview")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Xem trước và kiểm tra lỗi file import vật tư")]
    public async Task<ActionResult<ApiResponse<VatTuImportPreviewDto>>> XemTruocImport(
        [FromForm] VatTuImportFileRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();
        var data = await importService.XemTruocAsync(stream, request.File.FileName,
            request.File.Length, cancellationToken);
        var message = data.TongSoDong == 0
            ? "File import không có dòng dữ liệu vật tư."
            : data.SoDongLoi == 0
                ? $"File hợp lệ, có thể import {data.SoDongHopLe} vật tư."
                : data.SoDongHopLe > 0
                    ? $"Có {data.SoDongHopLe} dòng hợp lệ và {data.SoDongLoi} dòng lỗi. Khi import, hệ thống sẽ bỏ qua các dòng lỗi."
                    : $"Không có dòng hợp lệ để import. File có {data.SoDongLoi} dòng lỗi.";
        return Ok(ApiResponse<VatTuImportPreviewDto>.Ok(data, message));
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Import chính thức vật tư vào md_vat_tu")]
    public async Task<ActionResult<ApiResponse<VatTuImportResultDto>>> Import(
        [FromForm] VatTuImportFileRequest request, CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();
        var data = await importService.ImportAsync(stream, request.File.FileName,
            request.File.Length, request.CreatedByMsnv, cancellationToken);
        if (!data.ThanhCong)
        {
            return BadRequest(ApiResponse<VatTuImportResultDto>.Fail(data.Message, data));
        }
        return Ok(ApiResponse<VatTuImportResultDto>.Ok(data, data.Message));
    }
}
