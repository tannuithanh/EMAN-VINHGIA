using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Inventory.Kho.Dtos;
using Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Inventory;

[ApiController]
[Route(ApiRoutes.MasterData + "/kho")]
[ApiExplorerSettings(GroupName = "Warehouses")]
[SwaggerTag("Quản lý danh mục kho.")]
public sealed class KhoController(IKhoService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách kho")]
    public async Task<ActionResult<ApiResponse<PagedResult<KhoDto>>>> LayDanhSach(
        [FromQuery] BoLocKhoRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<KhoDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết kho")]
    public async Task<ActionResult<ApiResponse<KhoDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<KhoDto>.Ok(
            await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo kho")]
    public async Task<ActionResult<ApiResponse<KhoDto>>> TaoMoi(
        [FromBody] TaoKhoRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<KhoDto>.Ok(data, "Tạo kho thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật kho")]
    public async Task<ActionResult<ApiResponse<KhoDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatKhoRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<KhoDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật kho thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa kho")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa kho thành công."));
    }
}
