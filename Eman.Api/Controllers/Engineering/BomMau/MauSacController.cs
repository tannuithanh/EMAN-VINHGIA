using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.DungChung;

[ApiController]
[Route(ApiRoutes.EngineeringBomDungChung + "/mau-sac")]
[ApiExplorerSettings(GroupName = "BomCommon")]
[SwaggerTag("Quản lý màu sắc dùng chung cho B.O.M màu và B.O.M thô.")]
public sealed class MauSacController(IMauSacService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách màu sắc")]
    public async Task<ActionResult<ApiResponse<PagedResult<MauSacDto>>>> LayDanhSach([FromQuery] BoLocMauSacRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<MauSacDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<MauSacDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<MauSacDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MauSacDto>>> TaoMoi([FromBody] TaoMauSacRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<MauSacDto>.Ok(data, "Tạo màu sắc thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<MauSacDto>>> CapNhat(long id, [FromBody] CapNhatMauSacRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<MauSacDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật màu sắc thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa màu sắc thành công."));
    }
}
