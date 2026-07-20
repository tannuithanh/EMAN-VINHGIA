using Eman.Api.Common.Routing;
using System.ComponentModel.DataAnnotations;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eman.Api.Controllers.MasterData.BusinessPartners;

[ApiController]
[Route(ApiRoutes.MasterData + "/bang-gia")]
[ApiExplorerSettings(GroupName = "MasterData")]
[SwaggerTag("Quản lý bảng giá của nhà cung cấp.")]
public sealed class BangGiaController(IBangGiaService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách bảng giá")]
    public async Task<ActionResult<ApiResponse<PagedResult<BangGiaDto>>>> LayDanhSach(
        [FromQuery] BoLocBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.LayDanhSachAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<BangGiaDto>>.Ok(data));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết bảng giá")]
    public async Task<ActionResult<ApiResponse<BangGiaDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var data = await service.LayTheoIdAsync(id, cancellationToken);
        return Ok(ApiResponse<BangGiaDto>.Ok(data));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo bảng giá")]
    public async Task<ActionResult<ApiResponse<BangGiaDto>>> TaoMoi(
        [FromBody] TaoBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<BangGiaDto>.Ok(data, "Tạo bảng giá thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật bảng giá")]
    public async Task<ActionResult<ApiResponse<BangGiaDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.CapNhatAsync(id, request, cancellationToken);
        return Ok(ApiResponse<BangGiaDto>.Ok(data, "Cập nhật bảng giá thành công."));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa bảng giá")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa bảng giá thành công."));
    }
}
