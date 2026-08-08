using Eman.Api.Common.Routing;
using System.ComponentModel.DataAnnotations;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eman.Api.Controllers.MasterData.BusinessPartners;

[ApiController]
[Route(ApiRoutes.MasterData + "/doi-tac-kinh-doanh")]
[ApiExplorerSettings(GroupName = "BusinessPartners")]
[SwaggerTag("Quản lý đối tác kinh doanh và nhà cung cấp.")]
public sealed class DoiTacKinhDoanhController(IDoiTacKinhDoanhService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách đối tác kinh doanh")]
    public async Task<ActionResult<ApiResponse<PagedResult<DoiTacKinhDoanhDto>>>> LayDanhSach(
        [FromQuery] BoLocDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.LayDanhSachAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<DoiTacKinhDoanhDto>>.Ok(data));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết đối tác kinh doanh")]
    public async Task<ActionResult<ApiResponse<DoiTacKinhDoanhDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var data = await service.LayTheoIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DoiTacKinhDoanhDto>.Ok(data));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo đối tác kinh doanh")]
    public async Task<ActionResult<ApiResponse<DoiTacKinhDoanhDto>>> TaoMoi(
        [FromBody] TaoDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<DoiTacKinhDoanhDto>.Ok(data, "Tạo đối tác kinh doanh thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật đối tác kinh doanh")]
    public async Task<ActionResult<ApiResponse<DoiTacKinhDoanhDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.CapNhatAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DoiTacKinhDoanhDto>.Ok(
            data,
            "Cập nhật đối tác kinh doanh thành công."));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa đối tác kinh doanh")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa đối tác kinh doanh thành công."));
    }
}
