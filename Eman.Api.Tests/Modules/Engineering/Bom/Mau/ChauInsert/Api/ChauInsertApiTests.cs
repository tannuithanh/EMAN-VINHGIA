using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.ChauInsert.Api;

public sealed class ChauInsertApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/chau-insert";

    [Fact(DisplayName = "B.O.M màu - Chậu insert - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Chậu insert - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = ($"INSERT-{Guid.NewGuid():N}".ToUpperInvariant())[..20];
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { maChauInsert = ma, tenChauInsert = "Chậu insert kiểm thử", moTa = "Tạo từ API test" },
            "Tạo chậu insert thành công");

        Assert.Equal(System.Text.Json.JsonValueKind.String, created.GetProperty("id").ValueKind);
        var id = BomApiKiemThuHelper.LayGuidId(created);
        Assert.NotEqual(Guid.Empty, id);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new { maChauInsert = ma, tenChauInsert = "Chậu insert đã cập nhật", moTa = "Đã sửa", isActive = true, rowVersion },
            "Cập nhật chậu insert thành công");

        Assert.Equal("Chậu insert đã cập nhật", updated.GetProperty("tenChauInsert").GetString());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa chậu insert thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M màu - Chậu insert - Trùng mã phải trả 409")]
    public async Task TaoMoi_TrungMa_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = new { maChauInsert = ($"INSERT-{Guid.NewGuid():N}".ToUpperInvariant())[..20], tenChauInsert = "Chậu insert trùng" };
        await BomApiKiemThuHelper.TaoMoiAsync(client, Route, request, "Tạo chậu insert thành công");
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.Conflict, "đã tồn tại");
    }
}
