using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.Engineering.Bom.DungChung;

[ApiController]
[Route(ApiRoutes.EngineeringBomDungChung + "/he-san-pham")]
[ApiExplorerSettings(GroupName = "BomCommon")]
[SwaggerTag("Quản lý hệ sản phẩm dùng chung cho B.O.M màu và B.O.M thô.")]
public sealed class HeSanPhamController(IHeSanPhamService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách hệ sản phẩm")]
    public async Task<ActionResult<ApiResponse<PagedResult<HeSanPhamDto>>>> LayDanhSach([FromQuery] BoLocHeSanPhamRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<HeSanPhamDto>>.Ok(await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Lấy chi tiết hệ sản phẩm")]
    public async Task<ActionResult<ApiResponse<HeSanPhamDto>>> LayTheoId(long id, CancellationToken cancellationToken)
        => Ok(ApiResponse<HeSanPhamDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo hệ sản phẩm")]
    public async Task<ActionResult<ApiResponse<HeSanPhamDto>>> TaoMoi([FromBody] TaoHeSanPhamRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id }, ApiResponse<HeSanPhamDto>.Ok(data, "Tạo hệ sản phẩm thành công."));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Cập nhật hệ sản phẩm")]
    public async Task<ActionResult<ApiResponse<HeSanPhamDto>>> CapNhat(long id, [FromBody] CapNhatHeSanPhamRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<HeSanPhamDto>.Ok(await service.CapNhatAsync(id, request, cancellationToken), "Cập nhật hệ sản phẩm thành công."));

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Xóa hệ sản phẩm")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(long id, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa hệ sản phẩm thành công."));
    }
}
