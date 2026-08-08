using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.DungChung;

[ApiController]
[Route(ApiRoutes.EngineeringBomDungChung + "/de-tai")]
[ApiExplorerSettings(GroupName = "BomCommon")]
[SwaggerTag("Quản lý đề tài dùng chung cho B.O.M màu và B.O.M thô.")]
public sealed class DeTaiController(IDeTaiService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách đề tài")]
    public async Task<ActionResult<ApiResponse<PagedResult<DeTaiDto>>>> LayDanhSach([FromQuery] BoLocDeTaiRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<DeTaiDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<DeTaiDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<DeTaiDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DeTaiDto>>> TaoMoi([FromBody] TaoDeTaiRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<DeTaiDto>.Ok(data, "Tạo đề tài thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<DeTaiDto>>> CapNhat(long id, [FromBody] CapNhatDeTaiRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<DeTaiDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật đề tài thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa đề tài thành công."));
    }
}
