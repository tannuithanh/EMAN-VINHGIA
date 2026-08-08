using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Dtos;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Controllers.MasterData.Products;

[ApiController]
[Route(ApiRoutes.MasterData + "/thue-san-pham")]
[ApiExplorerSettings(GroupName = "ProductTax")]
[SwaggerTag("Quản lý danh mục thuế sản phẩm.")]
public sealed class ThueSanPhamController(IThueSanPhamService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách thuế sản phẩm")]
    public async Task<ActionResult<ApiResponse<PagedResult<ThueSanPhamDto>>>> LayDanhSach(
        [FromQuery] BoLocThueSanPhamRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<PagedResult<ThueSanPhamDto>>.Ok(
            await service.LayDanhSachAsync(request, cancellationToken)));

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết thuế sản phẩm")]
    public async Task<ActionResult<ApiResponse<ThueSanPhamDto>>> LayTheoId(
        Guid id, CancellationToken cancellationToken)
        => Ok(ApiResponse<ThueSanPhamDto>.Ok(await service.LayTheoIdAsync(id, cancellationToken)));

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo thuế sản phẩm")]
    public async Task<ActionResult<ApiResponse<ThueSanPhamDto>>> TaoMoi(
        [FromBody] TaoThueSanPhamRequest request, CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(nameof(LayTheoId), new { id = data.Id },
            ApiResponse<ThueSanPhamDto>.Ok(data, "Tạo thuế sản phẩm thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật thuế sản phẩm")]
    public async Task<ActionResult<ApiResponse<ThueSanPhamDto>>> CapNhat(
        Guid id, [FromBody] CapNhatThueSanPhamRequest request, CancellationToken cancellationToken)
        => Ok(ApiResponse<ThueSanPhamDto>.Ok(
            await service.CapNhatAsync(id, request, cancellationToken),
            "Cập nhật thuế sản phẩm thành công."));

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa thuế sản phẩm")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id, [FromQuery, Required] string rowVersion, CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { id }, "Xóa thuế sản phẩm thành công."));
    }
}
