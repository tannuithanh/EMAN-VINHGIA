using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Dtos;
using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eman.Api.Controllers.Engineering.Bom.TinhToan;

[ApiController]
[Route(ApiRoutes.EngineeringBomTinhToan + "/mau")]
[ApiExplorerSettings(GroupName = "BomCalculations")]
[SwaggerTag("Tính thử và chẩn đoán công thức B.O.M màu.")]
public sealed class TinhBomMauController(
    ITinhBomMauService service) : ControllerBase
{
    [HttpGet("test")]
    [SwaggerOperation(
        Summary = "Tính thử B.O.M màu theo mã sản phẩm",
        Description =
            "Nhập mã sản phẩm hoàn chỉnh, ví dụ 66-52220-01-02. " +
            "API trả chi tiết hệ, đề tài, màu, mã hàng nền, nhóm M, " +
            "các bước/hỗn hợp, phép tính lượng tiêu hao, chậu insert, phên và cốt thô.")]
    public async Task<ActionResult<ApiResponse<KetQuaKiemThuBomMauDto>>> KiemThu(
        [FromQuery] KiemThuTinhBomMauRequest request,
        CancellationToken cancellationToken)
    {
        var data = await service.KiemThuAsync(
            request.MaSanPham,
            cancellationToken);
        var message = data.DaTinhThanhCong
            ? "Tính thử B.O.M màu thành công."
            : "Đã hoàn tất kiểm tra nhưng chưa thể tính đầy đủ B.O.M màu. Vui lòng xem lỗi cấu hình trong kết quả.";

        return Ok(ApiResponse<KetQuaKiemThuBomMauDto>.Ok(data, message));
    }
}
