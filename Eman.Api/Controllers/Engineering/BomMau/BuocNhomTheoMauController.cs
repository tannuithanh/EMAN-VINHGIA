using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/buoc-nhom-theo-mau")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý bước nhóm theo màu.")]
public sealed class BuocNhomTheoMauController(IBuocNhomTheoMauService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách bước nhóm theo màu")]
    public async Task<ActionResult<ApiResponse<PagedResult<BuocNhomTheoMauDto>>>> LayDanhSach([FromQuery] BoLocBuocNhomTheoMauRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BuocNhomTheoMauDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<BuocNhomTheoMauDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<BuocNhomTheoMauDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BuocNhomTheoMauDto>>> TaoMoi([FromBody] TaoBuocNhomTheoMauRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<BuocNhomTheoMauDto>.Ok(data, "Tạo bước nhóm theo màu thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<BuocNhomTheoMauDto>>> CapNhat(long id, [FromBody] CapNhatBuocNhomTheoMauRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<BuocNhomTheoMauDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật bước nhóm theo màu thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa bước nhóm theo màu thành công."));
    }
}
