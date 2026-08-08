using Eman.Api.Common.Routing;
using System.ComponentModel.DataAnnotations;
using Eman.Application.Common;
using Eman.Application.Common.Requests;
using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eman.Api.Controllers.MasterData.BusinessPartners;

[ApiController]
[Route(ApiRoutes.MasterData + "/phien-ban-bang-gia")]
[ApiExplorerSettings(GroupName = "BusinessPartners")]
[SwaggerTag("Quản lý phiên bản bảng giá và vòng đời hiệu lực.")]
public sealed class PhienBanBangGiaController(IPhienBanBangGiaService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<PagedResult<PhienBanBangGiaDto>>>> LayDanhSach(
        [FromQuery] BoLocPhienBanBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.LayDanhSachAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResult<PhienBanBangGiaDto>>.Ok(data));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Lấy chi tiết phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<PhienBanBangGiaDto>>> LayTheoId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var data = await service.LayTheoIdAsync(id, cancellationToken);
        return Ok(ApiResponse<PhienBanBangGiaDto>.Ok(data));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Tạo phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<PhienBanBangGiaDto>>> TaoMoi(
        [FromBody] TaoPhienBanBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.TaoMoiAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LayTheoId),
            new { id = data.Id },
            ApiResponse<PhienBanBangGiaDto>.Ok(
                data,
                "Tạo phiên bản bảng giá thành công."));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Cập nhật phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<PhienBanBangGiaDto>>> CapNhat(
        Guid id,
        [FromBody] CapNhatPhienBanBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.CapNhatAsync(id, request, cancellationToken);
        return Ok(ApiResponse<PhienBanBangGiaDto>.Ok(
            data,
            "Cập nhật phiên bản bảng giá thành công."));
    }

    [HttpPost("{id:guid}/hieu-luc")]
    [SwaggerOperation(Summary = "Hiệu lực phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<PhienBanBangGiaDto>>> HieuLuc(
        Guid id,
        [FromBody] YeuCauRowVersion request,
        CancellationToken cancellationToken)
    {
        var data = await service.HieuLucAsync(id, request.RowVersion, cancellationToken);
        return Ok(ApiResponse<PhienBanBangGiaDto>.Ok(
            data,
            "Hiệu lực phiên bản bảng giá thành công."));
    }

    [HttpPost("{id:guid}/het-hieu-luc")]
    [SwaggerOperation(Summary = "Kết thúc hiệu lực phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<PhienBanBangGiaDto>>> HetHieuLuc(
        Guid id,
        [FromBody] YeuCauRowVersion request,
        CancellationToken cancellationToken)
    {
        var data = await service.HetHieuLucAsync(id, request.RowVersion, cancellationToken);
        return Ok(ApiResponse<PhienBanBangGiaDto>.Ok(
            data,
            "Kết thúc hiệu lực phiên bản bảng giá thành công."));
    }

    [HttpPost("{id:guid}/huy")]
    [SwaggerOperation(Summary = "Hủy phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<PhienBanBangGiaDto>>> Huy(
        Guid id,
        [FromBody] YeuCauRowVersion request,
        CancellationToken cancellationToken)
    {
        var data = await service.HuyAsync(id, request.RowVersion, cancellationToken);
        return Ok(ApiResponse<PhienBanBangGiaDto>.Ok(
            data,
            "Hủy phiên bản bảng giá thành công."));
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Xóa phiên bản bảng giá")]
    public async Task<ActionResult<ApiResponse<object>>> Xoa(
        Guid id,
        [FromQuery, Required] string rowVersion,
        CancellationToken cancellationToken)
    {
        await service.XoaAsync(id, rowVersion, cancellationToken);
        return Ok(ApiResponse<object>.Ok(
            new { id },
            "Xóa phiên bản bảng giá thành công."));
    }
}
