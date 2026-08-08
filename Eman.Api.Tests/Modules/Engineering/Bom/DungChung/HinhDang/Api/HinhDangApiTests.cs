using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.DungChung.HinhDang.Api;

public sealed class HinhDangApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/dung-chung/hinh-dang";

    [Fact(DisplayName = "B.O.M dùng chung - Hình dáng - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Hình dáng - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("HD");
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { maHinhDang = ma, tenHinhDang = "Hình dáng kiểm thử", moTa = "Tạo từ API test" },
            "Tạo hình dáng thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new { maHinhDang = ma, tenHinhDang = "Hình dáng đã cập nhật", moTa = "Đã sửa", isActive = true, rowVersion },
            "Cập nhật hình dáng thành công");

        Assert.Equal("Hình dáng đã cập nhật", updated.GetProperty("tenHinhDang").GetString());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa hình dáng thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Hình dáng - Trùng mã phải trả 409")]
    public async Task TaoMoi_TrungMa_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = new { maHinhDang = BomApiKiemThuHelper.TaoMa("HD-TRUNG"), tenHinhDang = "Hình dáng trùng" };
        await BomApiKiemThuHelper.TaoMoiAsync(client, Route, request, "Tạo hình dáng thành công");
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.Conflict, "đã tồn tại");
    }
}
