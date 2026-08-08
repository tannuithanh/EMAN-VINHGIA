using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Dtos;
using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Production;

[ApiController]
[Route(ApiRoutes.MasterData + "/nhom-nang-luc")]
[ApiExplorerSettings(GroupName = "CapacityGroups")]
[SwaggerTag("Quản lý danh mục nhóm năng lực.")]
public sealed class NhomNangLucController(INhomNangLucService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách nhóm năng lực")]
    public async Task<ActionResult<ApiResponse<PagedResult<NhomNangLucDto>>>> LayDanhSach(
        [FromQuery] BoLocNhomNangLucRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<NhomNangLucDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết nhóm năng lực")]
    public async Task<ActionResult<ApiResponse<NhomNangLucDto>>> LayTheoId(
        Guid id, CancellationToken cancellationToken)
        => Ok(ApiResponse<NhomNangLucDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo nhóm năng lực")]
    public async Task<ActionResult<ApiResponse<NhomNangLucDto>>> TaoMoi(
        [FromBody] TaoNhomNangLucRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id },
            ApiResponse<NhomNangLucDto>.Ok(data, "Tạo nhóm năng lực thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật nhóm năng lực")]
    public async Task<ActionResult<ApiResponse<NhomNangLucDto>>> CapNhat(
        Guid id, [FromBody] CapNhatNhomNangLucRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<NhomNangLucDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật nhóm năng lực thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa nhóm năng lực")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa nhóm năng lực thành công."));
    }
}
