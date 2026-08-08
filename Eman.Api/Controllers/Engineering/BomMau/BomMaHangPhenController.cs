using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/ma-hang-phen")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý phên theo mã hàng.")]
public sealed class BomMaHangPhenController(IBomMaHangPhenService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách cấu hình phên theo mã hàng")]
    public async Task<ActionResult<ApiResponse<PagedResult<BomMaHangPhenDto>>>> LayDanhSach(
        [FromQuery] BoLocBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BomMaHangPhenDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("ma-hang-co-phen")]
    [SwaggerOperation(
        Summary = "Lấy danh sách mã hàng có phên",
        Description = "Trả về mã hàng, mã phên và thông tin cấu hình phên. Có hỗ trợ phân trang và lọc theo keyword, trạng thái, mã hàng.")]
    public async Task<ActionResult<ApiResponse<PagedResult<MaHangCoPhenDto>>>> LayDanhSachMaHangCoPhen(
        [FromQuery] BoLocBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<MaHangCoPhenDto>>.Ok(
            await service.LayDanhSachMaHangCoPhenAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BomMaHangPhenDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMaHangPhenDto>.Ok(
            await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BomMaHangPhenDto>>> TaoMoi(
        [FromBody] TaoBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<BomMaHangPhenDto>.Ok(data, "Tạo phên theo mã hàng thành công."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BomMaHangPhenDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMaHangPhenDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật phên theo mã hàng thành công."));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa phên theo mã hàng thành công."));
    }
}
