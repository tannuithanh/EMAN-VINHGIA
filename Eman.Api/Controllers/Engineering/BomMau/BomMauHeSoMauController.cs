using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/he-so-mau")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý hệ số màu.")]
public sealed class BomMauHeSoMauController(IBomMauHeSoMauService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách hệ số màu")]
    public async Task<ActionResult<ApiResponse<PagedResult<BomMauHeSoMauDto>>>> LayDanhSach([FromQuery] BoLocBomMauHeSoMauRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BomMauHeSoMauDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<BomMauHeSoMauDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauHeSoMauDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BomMauHeSoMauDto>>> TaoMoi([FromBody] TaoBomMauHeSoMauRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<BomMauHeSoMauDto>.Ok(data, "Tạo hệ số màu thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<BomMauHeSoMauDto>>> CapNhat(long id, [FromBody] CapNhatBomMauHeSoMauRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauHeSoMauDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật hệ số màu thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa hệ số màu thành công."));
    }
}
