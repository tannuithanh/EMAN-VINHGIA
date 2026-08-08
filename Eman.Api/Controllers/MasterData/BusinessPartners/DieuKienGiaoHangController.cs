
using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.BusinessPartners;

[ApiController]
[Route(ApiRoutes.MasterData + "/dieu-kien-giao-hang")]
[ApiExplorerSettings(GroupName = "BusinessPartners")]
[SwaggerTag("Quản lý danh mục điều kiện giao hàng.")]
public sealed class DieuKienGiaoHangController(IDieuKienGiaoHangService service)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách điều kiện giao hàng")]
    public async Task<ActionResult<ApiResponse<PagedResult<DieuKienGiaoHangDto>>>> LayDanhSach(
        [FromQuery] BoLocDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.LayDanhSachAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<DieuKienGiaoHangDto>>.Ok(data));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết điều kiện giao hàng")]
    public async Task<ActionResult<ApiResponse<DieuKienGiaoHangDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var data = await service.LayTheoIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DieuKienGiaoHangDto>.Ok(data));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo điều kiện giao hàng")]
    public async Task<ActionResult<ApiResponse<DieuKienGiaoHangDto>>> TaoMoi(
        [FromBody] TaoDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<DieuKienGiaoHangDto>.Ok(
                data,
                "Tạo điều kiện giao hàng thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật điều kiện giao hàng")]
    public async Task<ActionResult<ApiResponse<DieuKienGiaoHangDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.CapNhatAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DieuKienGiaoHangDto>.Ok(
            data,
            "Cập nhật điều kiện giao hàng thành công."));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa điều kiện giao hàng")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(
            new { id },
            "Xóa điều kiện giao hàng thành công."));
    }
}
