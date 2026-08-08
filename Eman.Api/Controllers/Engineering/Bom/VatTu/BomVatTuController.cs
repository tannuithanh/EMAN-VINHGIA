using Eman.Api.Common.Routing;
using Eman.Api.Contracts.Engineering.Bom.VatTu.Imports;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;
using Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;
using Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Interfaces;
using Eman.Application.Modules.Engineering.Bom.VatTu.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.VatTu;

[ApiController]
[Route(ApiRoutes.EngineeringBomVatTu)]
[ApiExplorerSettings(GroupName = "BomMaterial")]
[SwaggerTag("Quản lý phiên bản và thành phần B.O.M vật tư nhiều cấp.")]
public sealed class BomVatTuController(IBomVatTuService service, IBomVatTuImportService importService) : ControllerBase
{

    [HttpGet("import/template")]
    [SwaggerOperation(Summary = "Tải file mẫu import B.O.M vật tư")]
    public async Task<IActionResult> TaiTemplateImport(CancellationToken cancellationToken)
    {
        var file = await importService.TaoTemplateAsync(cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost("import/preview")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Xem trước và kiểm tra lỗi file import B.O.M vật tư")]
    public async Task<ActionResult<ApiResponse<BomVatTuImportPreviewDto>>> XemTruocImport(
        [FromForm] BomVatTuImportFileRequest request,
        CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();
        var data = await importService.XemTruocAsync(
            stream, request.File.FileName, request.File.Length, cancellationToken);
        var message = data.TongSoDong == 0
            ? "File import không có dòng dữ liệu B.O.M vật tư."
            : data.SoBomLoi == 0
                ? $"File hợp lệ, có thể import {data.SoBomCoTheImport} B.O.M vật tư."
                : data.SoBomCoTheImport > 0
                    ? $"Có {data.SoBomCoTheImport} B.O.M hợp lệ và {data.SoBomLoi} B.O.M lỗi. B.O.M có lỗi sẽ bị bỏ qua toàn bộ khi import."
                    : $"Không có B.O.M hợp lệ để import. File có {data.SoBomLoi} B.O.M lỗi.";
        return Ok(ApiResponse<BomVatTuImportPreviewDto>.Ok(data, message));
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(Summary = "Import B.O.M vật tư và tự tạo phiên bản Nháp mới")]
    public async Task<ActionResult<ApiResponse<BomVatTuImportResultDto>>> Import(
        [FromForm] BomVatTuImportFileRequest request,
        CancellationToken cancellationToken)
    {
        await using var stream = request.File.OpenReadStream();
        var data = await importService.ImportAsync(
            stream, request.File.FileName, request.File.Length, request.CreatedByMsnv, cancellationToken);
        if (!data.ThanhCong)
        {
            return BadRequest(ApiResponse<BomVatTuImportResultDto>.Fail(data.Message, data));
        }
        return Ok(ApiResponse<BomVatTuImportResultDto>.Ok(data, data.Message));
    }

    [HttpGet("phien-ban")]
    [SwaggerOperation(Summary = "Lấy danh sách phiên bản B.O.M vật tư")]
    public async Task<ActionResult<ApiResponse<PagedResult<BomVatTuPhienBanDto>>>> LayDanhSach(
        [FromQuery] BoLocBomVatTuPhienBanRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BomVatTuPhienBanDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("phien-ban/{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết một phiên bản B.O.M vật tư")]
    public async Task<ActionResult<ApiResponse<BomVatTuPhienBanDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomVatTuPhienBanDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost("phien-ban")]
    [SwaggerOperation(Summary = "Tạo phiên bản B.O.M vật tư ở trạng thái Nháp")]
    public async Task<ActionResult<ApiResponse<BomVatTuPhienBanDto>>> TaoPhienBan(
        [FromBody] TaoBomVatTuPhienBanRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoPhienBanAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id },
            ApiResponse<BomVatTuPhienBanDto>.Ok(data, "Tạo phiên bản B.O.M vật tư thành công."));
    }

    [HttpPut("phien-ban/{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật phiên bản B.O.M vật tư đang Nháp")]
    public async Task<ActionResult<ApiResponse<BomVatTuPhienBanDto>>> CapNhatPhienBan(
        Guid id,
        [FromBody] CapNhatBomVatTuPhienBanRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomVatTuPhienBanDto>.Ok(
            await service.CapNhatPhienBanAsync(id, request, cancellationToken),
            "Cập nhật phiên bản B.O.M vật tư thành công."));

    [HttpPost("phien-ban/{id:guid}/hieu-luc")]
    [SwaggerOperation(Summary = "Hiệu lực phiên bản B.O.M vật tư")]
    public async Task<ActionResult<ApiResponse<BomVatTuPhienBanDto>>> HieuLuc(
        Guid id,
        [FromQuery, Required] string rowVersion,
        [FromQuery, MaxLength(50)] string? updatedByMsnv,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomVatTuPhienBanDto>.Ok(
            await service.HieuLucAsync(id, rowVersion, updatedByMsnv, cancellationToken),
            "Hiệu lực phiên bản B.O.M vật tư thành công."));

    [HttpPost("phien-ban/{id:guid}/ngung-hieu-luc")]
    [SwaggerOperation(Summary = "Ngừng hiệu lực phiên bản B.O.M vật tư")]
    public async Task<ActionResult<ApiResponse<BomVatTuPhienBanDto>>> NgungHieuLuc(
        Guid id,
        [FromQuery, Required] string rowVersion,
        [FromQuery, MaxLength(50)] string? updatedByMsnv,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomVatTuPhienBanDto>.Ok(
            await service.NgungHieuLucAsync(id, rowVersion, updatedByMsnv, cancellationToken),
            "Ngừng hiệu lực phiên bản B.O.M vật tư thành công."));

    [HttpDelete("phien-ban/{id:guid}")]
    [SwaggerOperation(Summary = "Xóa phiên bản B.O.M vật tư đang Nháp")]
    public async Task<ActionResult<ApiResponse<object>>> XoaPhienBan(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaPhienBanAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa phiên bản B.O.M vật tư thành công."));
    }

    [HttpPost("phien-ban/{phienBanId:guid}/chi-tiet")]
    [SwaggerOperation(Summary = "Thêm vật tư thành phần vào phiên bản B.O.M")]
    public async Task<ActionResult<ApiResponse<BomVatTuChiTietDto>>> ThemChiTiet(
        Guid phienBanId,
        [FromBody] TaoBomVatTuChiTietRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomVatTuChiTietDto>.Ok(
            await service.ThemChiTietAsync(phienBanId, request, cancellationToken),
            "Thêm vật tư thành phần thành công."));

    [HttpPut("chi-tiet/{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật vật tư thành phần của phiên bản B.O.M")]
    public async Task<ActionResult<ApiResponse<BomVatTuChiTietDto>>> CapNhatChiTiet(
        Guid id,
        [FromBody] CapNhatBomVatTuChiTietRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomVatTuChiTietDto>.Ok(
            await service.CapNhatChiTietAsync(id, request, cancellationToken),
            "Cập nhật vật tư thành phần thành công."));

    [HttpDelete("chi-tiet/{id:guid}")]
    [SwaggerOperation(Summary = "Xóa vật tư thành phần khỏi phiên bản B.O.M")]
    public async Task<ActionResult<ApiResponse<object>>> XoaChiTiet(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaChiTietAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa vật tư thành phần thành công."));
    }
}
