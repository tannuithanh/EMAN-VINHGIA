
using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.BusinessPartners;

[ApiController]
[Route(ApiRoutes.MasterData + "/dieu-kien-thanh-toan")]
[ApiExplorerSettings(GroupName = "BusinessPartners")]
[SwaggerTag("Quản lý danh mục điều kiện thanh toán.")]
public sealed class DieuKienThanhToanController(IDieuKienThanhToanService service)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách điều kiện thanh toán")]
    public async Task<ActionResult<ApiResponse<PagedResult<DieuKienThanhToanDto>>>> LayDanhSach(
        [FromQuery] BoLocDieuKienThanhToanRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.LayDanhSachAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<DieuKienThanhToanDto>>.Ok(data));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết điều kiện thanh toán")]
    public async Task<ActionResult<ApiResponse<DieuKienThanhToanDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var data = await service.LayTheoIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DieuKienThanhToanDto>.Ok(data));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo điều kiện thanh toán")]
    public async Task<ActionResult<ApiResponse<DieuKienThanhToanDto>>> TaoMoi(
        [FromBody] TaoDieuKienThanhToanRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<DieuKienThanhToanDto>.Ok(
                data,
                "Tạo điều kiện thanh toán thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật điều kiện thanh toán")]
    public async Task<ActionResult<ApiResponse<DieuKienThanhToanDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatDieuKienThanhToanRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.CapNhatAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DieuKienThanhToanDto>.Ok(
            data,
            "Cập nhật điều kiện thanh toán thành công."));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa điều kiện thanh toán")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(
            new { id },
            "Xóa điều kiện thanh toán thành công."));
    }
}
