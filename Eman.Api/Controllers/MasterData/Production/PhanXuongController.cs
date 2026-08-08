using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Dtos;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Production;

[ApiController]
[Route(ApiRoutes.MasterData + "/phan-xuong")]
[ApiExplorerSettings(GroupName = "Workshops")]
[SwaggerTag("Quản lý danh mục phân xưởng.")]
public sealed class PhanXuongController(IPhanXuongService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách phân xưởng")]
    public async Task<ActionResult<ApiResponse<PagedResult<PhanXuongDto>>>> LayDanhSach(
        [FromQuery] BoLocPhanXuongRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<PhanXuongDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết phân xưởng")]
    public async Task<ActionResult<ApiResponse<PhanXuongDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PhanXuongDto>.Ok(
            await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo phân xưởng")]
    public async Task<ActionResult<ApiResponse<PhanXuongDto>>> TaoMoi(
        [FromBody] TaoPhanXuongRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<PhanXuongDto>.Ok(data, "Tạo phân xưởng thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật phân xưởng")]
    public async Task<ActionResult<ApiResponse<PhanXuongDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatPhanXuongRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PhanXuongDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật phân xưởng thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa phân xưởng")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa phân xưởng thành công."));
    }
}
