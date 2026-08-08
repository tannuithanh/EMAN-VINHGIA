using Eman.Api.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace Eman.Api.Tests.Modules.MasterData.Common.DonViTinh.Api;

/// <summary>
/// Ví dụ một bài kiểm thử nghiệp vụ riêng.
/// Khi có quy tắc mới, tạo thêm test tương tự trong thư mục BusinessRules.
/// </summary>
public sealed class DonViTinhBusinessTests(EmanApiFactory factory)
    : IClassFixture<EmanApiFactory>
{
    [Fact(DisplayName = "Tạo đơn vị tính thiếu mã và tên phải bị từ chối")]
    public async Task TaoDonViTinh_ThieuDuLieuBatBuoc_PhaiTraVe400()
    {
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/master-data/don-vi-tinh",
            new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
