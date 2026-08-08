using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Dtos;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Common;

[ApiController]
[Route(ApiRoutes.MasterData + "/don-vi-tinh")]
[ApiExplorerSettings(GroupName = "UnitsOfMeasure")]
[SwaggerTag("Quản lý danh mục đơn vị tính.")]
public sealed class DonViTinhController(IDonViTinhService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách đơn vị tính")]
    public async Task<ActionResult<ApiResponse<PagedResult<DonViTinhDto>>>> LayDanhSach(
        [FromQuery] BoLocDonViTinhRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<DonViTinhDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết đơn vị tính")]
    public async Task<ActionResult<ApiResponse<DonViTinhDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<DonViTinhDto>.Ok(
            await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo đơn vị tính")]
    public async Task<ActionResult<ApiResponse<DonViTinhDto>>> TaoMoi(
        [FromBody] TaoDonViTinhRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<DonViTinhDto>.Ok(data, "Tạo đơn vị tính thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật đơn vị tính")]
    public async Task<ActionResult<ApiResponse<DonViTinhDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatDonViTinhRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<DonViTinhDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật đơn vị tính thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa đơn vị tính")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa đơn vị tính thành công."));
    }
}
