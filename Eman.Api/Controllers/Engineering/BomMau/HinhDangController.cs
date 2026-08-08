using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.DungChung;

[ApiController]
[Route(ApiRoutes.EngineeringBomDungChung + "/hinh-dang")]
[ApiExplorerSettings(GroupName = "BomCommon")]
[SwaggerTag("Quản lý hình dáng dùng chung cho B.O.M màu và B.O.M thô.")]
public sealed class HinhDangController(IHinhDangService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách hình dáng")]
    public async Task<ActionResult<ApiResponse<PagedResult<HinhDangDto>>>> LayDanhSach([FromQuery] BoLocHinhDangRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<HinhDangDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Lấy chi tiết hình dáng")]
    public async Task<ActionResult<ApiResponse<HinhDangDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<HinhDangDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo hình dáng")]
    public async Task<ActionResult<ApiResponse<HinhDangDto>>> TaoMoi([FromBody] TaoHinhDangRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<HinhDangDto>.Ok(data, "Tạo hình dáng thành công."));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Cập nhật hình dáng")]
    public async Task<ActionResult<ApiResponse<HinhDangDto>>> CapNhat(long id, [FromBody] CapNhatHinhDangRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<HinhDangDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật hình dáng thành công."));

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Xóa hình dáng")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa hình dáng thành công."));
    }
}
