using Eman.Api.Common.Routing;
using System.ComponentModel.DataAnnotations;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eman.Api.Controllers.MasterData.BusinessPartners;

[ApiController]
[Route(ApiRoutes.MasterData + "/loai-doi-tac")]
[ApiExplorerSettings(GroupName = "BusinessPartners")]
[SwaggerTag("Quản lý danh mục loại đối tác.")]
public sealed class LoaiDoiTacController(ILoaiDoiTacService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách loại đối tác")]
    public async Task<ActionResult<ApiResponse<PagedResult<LoaiDoiTacDto>>>> LayDanhSach(
        [FromQuery] BoLocLoaiDoiTacRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.LayDanhSachAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<LoaiDoiTacDto>>.Ok(data));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết loại đối tác")]
    public async Task<ActionResult<ApiResponse<LoaiDoiTacDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var data = await service.LayTheoIdAsync(id, cancellationToken);
        return Ok(ApiResponse<LoaiDoiTacDto>.Ok(data));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo loại đối tác")]
    public async Task<ActionResult<ApiResponse<LoaiDoiTacDto>>> TaoMoi(
        [FromBody] TaoLoaiDoiTacRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<LoaiDoiTacDto>.Ok(data, "Tạo loại đối tác thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật loại đối tác")]
    public async Task<ActionResult<ApiResponse<LoaiDoiTacDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatLoaiDoiTacRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.CapNhatAsync(id, request, cancellationToken);
        return Ok(ApiResponse<LoaiDoiTacDto>.Ok(data, "Cập nhật loại đối tác thành công."));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa loại đối tác")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa loại đối tác thành công."));
    }
}
