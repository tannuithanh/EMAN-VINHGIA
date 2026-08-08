using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Dtos;
using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Materials;

[ApiController]
[Route(ApiRoutes.MasterData + "/co-so-mua-vat-tu")]
[ApiExplorerSettings(GroupName = "Materials")]
[SwaggerTag("Quản lý danh mục cơ sở mua vật tư.")]
public sealed class CoSoMuaVatTuController(ICoSoMuaVatTuService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách cơ sở mua vật tư")]
    public async Task<ActionResult<ApiResponse<PagedResult<CoSoMuaVatTuDto>>>> LayDanhSach(
        [FromQuery] BoLocCoSoMuaVatTuRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<CoSoMuaVatTuDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết cơ sở mua vật tư")]
    public async Task<ActionResult<ApiResponse<CoSoMuaVatTuDto>>> LayTheoId(Guid id, CancellationToken cancellationToken)
        => Ok(ApiResponse<CoSoMuaVatTuDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo cơ sở mua vật tư")]
    public async Task<ActionResult<ApiResponse<CoSoMuaVatTuDto>>> TaoMoi(
        [FromBody] TaoCoSoMuaVatTuRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id },
            ApiResponse<CoSoMuaVatTuDto>.Ok(data, "Tạo cơ sở mua vật tư thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật cơ sở mua vật tư")]
    public async Task<ActionResult<ApiResponse<CoSoMuaVatTuDto>>> CapNhat(
        Guid id, [FromBody] CapNhatCoSoMuaVatTuRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<CoSoMuaVatTuDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật cơ sở mua vật tư thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa cơ sở mua vật tư")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa cơ sở mua vật tư thành công."));
    }
}
