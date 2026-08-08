using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.DungChung;

[ApiController]
[Route(ApiRoutes.EngineeringBomDungChung + "/ma-hang")]
[ApiExplorerSettings(GroupName = "BomCommon")]
[SwaggerTag("Quản lý mã hàng dùng chung cho B.O.M màu và B.O.M thô.")]
public sealed class MaHangController(IMaHangService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách mã hàng")]
    public async Task<ActionResult<ApiResponse<PagedResult<MaHangDto>>>> LayDanhSach([FromQuery] BoLocMaHangRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<MaHangDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<MaHangDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<MaHangDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MaHangDto>>> TaoMoi([FromBody] TaoMaHangRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<MaHangDto>.Ok(data, "Tạo mã hàng thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<MaHangDto>>> CapNhat(long id, [FromBody] CapNhatMaHangRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<MaHangDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật mã hàng thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa mã hàng thành công."));
    }
}
