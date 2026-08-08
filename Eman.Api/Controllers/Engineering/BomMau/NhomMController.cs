using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.DungChung;

[ApiController]
[Route(ApiRoutes.EngineeringBomDungChung + "/nhom-m")]
[ApiExplorerSettings(GroupName = "BomCommon")]
[SwaggerTag("Quản lý nhóm M dùng chung cho B.O.M màu và B.O.M thô.")]
public sealed class NhomMController(INhomMService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách nhóm M")]
    public async Task<ActionResult<ApiResponse<PagedResult<NhomMDto>>>> LayDanhSach([FromQuery] BoLocNhomMRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<NhomMDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Lấy chi tiết nhóm M")]
    public async Task<ActionResult<ApiResponse<NhomMDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<NhomMDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo nhóm M")]
    public async Task<ActionResult<ApiResponse<NhomMDto>>> TaoMoi([FromBody] TaoNhomMRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<NhomMDto>.Ok(data, "Tạo nhóm M thành công."));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Cập nhật nhóm M")]
    public async Task<ActionResult<ApiResponse<NhomMDto>>> CapNhat(long id, [FromBody] CapNhatNhomMRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<NhomMDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật nhóm M thành công."));

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Xóa nhóm M")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa nhóm M thành công."));
    }
}
