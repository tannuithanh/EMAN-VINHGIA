using Eman.Api.Common.Routing;
using Eman.Application.Common;
using Eman.Application.Contracts.HeThong;
using Eman.Application.Dtos.HeThong;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eman.Api.Controllers.HeThong;

[ApiController]
[Route(ApiRoutes.System)]
[ApiExplorerSettings(GroupName = "System")]
[SwaggerTag("API mẫu kiểm tra trạng thái chạy của hệ thống EMAN.")]
public sealed class HeThongController(IThongTinHeThongService service) : ControllerBase
{
    /// <summary>
    /// Kiểm tra API EMAN đang hoạt động.
    /// </summary>
    [HttpGet("kiem-tra")]
    [SwaggerOperation(
        Summary = "Kiểm tra trạng thái hệ thống",
        Description = "Endpoint mẫu dùng để xác nhận API EMAN đã khởi động thành công.")]
    [ProducesResponseType(typeof(ApiResponse<ThongTinHeThongDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<ThongTinHeThongDto>> KiemTra()
    {
        var data = service.LayThongTin();
        return Ok(ApiResponse<ThongTinHeThongDto>.Ok(
            data,
            "API EMAN đang hoạt động."));
    }
}
