using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/dinh-muc-nhom-m")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý định mức nhóm M.")]
public sealed class BomMauDinhMucNhomMController(IBomMauDinhMucNhomMService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách định mức nhóm M")]
    public async Task<ActionResult<ApiResponse<PagedResult<BomMauDinhMucNhomMDto>>>> LayDanhSach([FromQuery] BoLocBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BomMauDinhMucNhomMDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<BomMauDinhMucNhomMDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauDinhMucNhomMDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BomMauDinhMucNhomMDto>>> TaoMoi([FromBody] TaoBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<BomMauDinhMucNhomMDto>.Ok(data, "Tạo định mức nhóm M thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<BomMauDinhMucNhomMDto>>> CapNhat(long id, [FromBody] CapNhatBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauDinhMucNhomMDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật định mức nhóm M thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa định mức nhóm M thành công."));
    }
}
