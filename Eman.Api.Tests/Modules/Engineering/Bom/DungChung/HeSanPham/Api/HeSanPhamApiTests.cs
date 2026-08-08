using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.DungChung.HeSanPham.Api;

public sealed class HeSanPhamApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/dung-chung/he-san-pham";

    [Fact(DisplayName = "B.O.M dùng chung - Hệ sản phẩm - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Hệ sản phẩm - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var id = BomApiKiemThuHelper.TaoIdHeSanPham();
        var ma = BomApiKiemThuHelper.TaoMa("HE");

        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { id, maHe = ma, tenHe = "Hệ kiểm thử", moTa = "Tạo từ API test" },
            "Tạo hệ sản phẩm thành công");

        Assert.Equal(id, BomApiKiemThuHelper.LayId(created));
        Assert.Equal(ma, created.GetProperty("maHe").GetString());

        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal("Hệ kiểm thử", detail.GetProperty("tenHe").GetString());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new { maHe = ma, tenHe = "Hệ kiểm thử đã cập nhật", moTa = "Đã sửa", isActive = true },
            "Cập nhật hệ sản phẩm thành công");

        Assert.Equal("Hệ kiểm thử đã cập nhật", updated.GetProperty("tenHe").GetString());

        await BomApiKiemThuHelper.XoaAsync(
            client, Route, id, "Xóa hệ sản phẩm thành công");
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Hệ sản phẩm - Trùng mã phải trả 409")]
    public async Task TaoMoi_TrungMa_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("HE-TRUNG");

        await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { id = BomApiKiemThuHelper.TaoIdHeSanPham(), maHe = ma, tenHe = "Hệ thứ nhất" },
            "Tạo hệ sản phẩm thành công");

        using var response = await client.PostAsJsonAsync(
            Route,
            new { id = BomApiKiemThuHelper.TaoIdHeSanPham(), maHe = ma, tenHe = "Hệ thứ hai" });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "đã tồn tại");
    }
}
