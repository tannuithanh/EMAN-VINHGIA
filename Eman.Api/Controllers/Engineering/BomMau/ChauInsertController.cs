using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/chau-insert")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý chậu insert.")]
public sealed class ChauInsertController(IChauInsertService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách chậu insert")]
    public async Task<ActionResult<ApiResponse<PagedResult<ChauInsertDto>>>> LayDanhSach([FromQuery] BoLocChauInsertRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<ChauInsertDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết chậu insert")]
    public async Task<ActionResult<ApiResponse<ChauInsertDto>>> LayTheoId(Guid id, CancellationToken cancellationToken)
        => Ok(ApiResponse<ChauInsertDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo chậu insert")]
    public async Task<ActionResult<ApiResponse<ChauInsertDto>>> TaoMoi([FromBody] TaoChauInsertRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<ChauInsertDto>.Ok(data, "Tạo chậu insert thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật chậu insert")]
    public async Task<ActionResult<ApiResponse<ChauInsertDto>>> CapNhat(Guid id, [FromBody] CapNhatChauInsertRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<ChauInsertDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật chậu insert thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa chậu insert")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(Guid id, [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa chậu insert thành công."));
    }
}
