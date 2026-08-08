using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/he-so-de-tai")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý hệ số đề tài.")]
public sealed class BomMauHeSoDeTaiController(IBomMauHeSoDeTaiService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách hệ số đề tài")]
    public async Task<ActionResult<ApiResponse<PagedResult<BomMauHeSoDeTaiDto>>>> LayDanhSach([FromQuery] BoLocBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BomMauHeSoDeTaiDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")] public async Task<ActionResult<ApiResponse<BomMauHeSoDeTaiDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauHeSoDeTaiDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BomMauHeSoDeTaiDto>>> TaoMoi([FromBody] TaoBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<BomMauHeSoDeTaiDto>.Ok(data, "Tạo hệ số đề tài thành công."));
    }

    [HttpPut("{id:long}")] public async Task<ActionResult<ApiResponse<BomMauHeSoDeTaiDto>>> CapNhat(long id, [FromBody] CapNhatBomMauHeSoDeTaiRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMauHeSoDeTaiDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật hệ số đề tài thành công."));

    [HttpDelete("{id:long}")] public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa hệ số đề tài thành công."));
    }
}
