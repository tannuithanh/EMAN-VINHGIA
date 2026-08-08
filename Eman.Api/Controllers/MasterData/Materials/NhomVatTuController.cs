using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Dtos;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Materials;

[ApiController]
[Route(ApiRoutes.MasterData + "/nhom-vat-tu")]
[ApiExplorerSettings(GroupName = "Materials")]
[SwaggerTag("Quản lý danh mục nhóm vật tư.")]
public sealed class NhomVatTuController(INhomVatTuService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách nhóm vật tư")]
    public async Task<ActionResult<ApiResponse<PagedResult<NhomVatTuDto>>>> LayDanhSach(
        [FromQuery] BoLocNhomVatTuRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<NhomVatTuDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết nhóm vật tư")]
    public async Task<ActionResult<ApiResponse<NhomVatTuDto>>> LayTheoId(Guid id, CancellationToken cancellationToken)
        => Ok(ApiResponse<NhomVatTuDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo nhóm vật tư")]
    public async Task<ActionResult<ApiResponse<NhomVatTuDto>>> TaoMoi(
        [FromBody] TaoNhomVatTuRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id },
            ApiResponse<NhomVatTuDto>.Ok(data, "Tạo nhóm vật tư thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật nhóm vật tư")]
    public async Task<ActionResult<ApiResponse<NhomVatTuDto>>> CapNhat(
        Guid id, [FromBody] CapNhatNhomVatTuRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<NhomVatTuDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật nhóm vật tư thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa nhóm vật tư")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa nhóm vật tư thành công."));
    }
}
