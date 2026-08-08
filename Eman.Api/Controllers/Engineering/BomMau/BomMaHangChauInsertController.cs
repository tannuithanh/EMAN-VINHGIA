using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.Mau;

[ApiController]
[Route(ApiRoutes.EngineeringBomMau + "/ma-hang-chau-insert")]
[ApiExplorerSettings(GroupName = "BomColor")]
[SwaggerTag("Quản lý chậu insert theo mã hàng.")]
public sealed class BomMaHangChauInsertController(IBomMaHangChauInsertService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách cấu hình chậu insert theo mã hàng")]
    public async Task<ActionResult<ApiResponse<PagedResult<BomMaHangChauInsertDto>>>> LayDanhSach(
        [FromQuery] BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<BomMaHangChauInsertDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("ma-hang-co-chau-insert")]
    [SwaggerOperation(
        Summary = "Lấy danh sách mã hàng có chậu insert và số lượng",
        Description = "Phân trang theo mã hàng. Mỗi mã hàng trả về số loại insert, tổng số lượng và chi tiết từng loại insert.")]
    public async Task<ActionResult<ApiResponse<PagedResult<MaHangCoChauInsertDto>>>> LayDanhSachMaHangCoChauInsert(
        [FromQuery] BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<MaHangCoChauInsertDto>>.Ok(
            await service.LayDanhSachMaHangCoChauInsertAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BomMaHangChauInsertDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMaHangChauInsertDto>.Ok(
            await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BomMaHangChauInsertDto>>> TaoMoi(
        [FromBody] TaoBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<BomMaHangChauInsertDto>.Ok(
                data,
                "Tạo chậu insert theo mã hàng thành công."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BomMaHangChauInsertDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
        => Ok(ApiResponse<BomMaHangChauInsertDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật chậu insert theo mã hàng thành công."));

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(
            new { id },
            "Xóa chậu insert theo mã hàng thành công."));
    }
}
