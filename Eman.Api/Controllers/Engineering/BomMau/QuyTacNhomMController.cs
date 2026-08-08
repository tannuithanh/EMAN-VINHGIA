using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.DungChung;

[ApiController]
[Route(ApiRoutes.EngineeringBomDungChung + "/quy-tac-nhom-m")]
[ApiExplorerSettings(GroupName = "BomCommon")]
[SwaggerTag("Quản lý quy tắc nhóm M dùng chung cho B.O.M màu và B.O.M thô.")]
public sealed class QuyTacNhomMController(IQuyTacNhomMService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách quy tắc nhóm M")]
    public async Task<ActionResult<ApiResponse<PagedResult<QuyTacNhomMDto>>>> LayDanhSach([FromQuery] BoLocQuyTacNhomMRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<QuyTacNhomMDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<QuyTacNhomMDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<QuyTacNhomMDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<QuyTacNhomMDto>>> TaoMoi([FromBody] TaoQuyTacNhomMRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<QuyTacNhomMDto>.Ok(data, "Tạo quy tắc nhóm M thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<QuyTacNhomMDto>>> CapNhat(long id, [FromBody] CapNhatQuyTacNhomMRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<QuyTacNhomMDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật quy tắc nhóm M thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa quy tắc nhóm M thành công."));
    }
}
