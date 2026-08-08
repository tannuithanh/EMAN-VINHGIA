using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/buoc")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý bước B.O.M màu.")]
public sealed class BomMauBuocController(IBomMauBuocService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách bước B.O.M màu")]
    public async Task<ActionResult<ApiResponse<PagedResult<BomMauBuocDto>>>> LayDanhSach([FromQuery] BoLocBomMauBuocRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BomMauBuocDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Lấy chi tiết bước B.O.M màu")]
    public async Task<ActionResult<ApiResponse<BomMauBuocDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauBuocDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo bước B.O.M màu")]
    public async Task<ActionResult<ApiResponse<BomMauBuocDto>>> TaoMoi([FromBody] TaoBomMauBuocRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<BomMauBuocDto>.Ok(data, "Tạo bước B.O.M màu thành công."));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Cập nhật bước B.O.M màu")]
    public async Task<ActionResult<ApiResponse<BomMauBuocDto>>> CapNhat(long id, [FromBody] CapNhatBomMauBuocRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauBuocDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật bước B.O.M màu thành công."));

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Xóa bước B.O.M màu")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa bước B.O.M màu thành công."));
    }
}
