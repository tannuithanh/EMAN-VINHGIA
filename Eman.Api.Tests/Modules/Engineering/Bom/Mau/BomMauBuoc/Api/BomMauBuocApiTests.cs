using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.BomMauBuoc.Api;

public sealed class BomMauBuocApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/buoc";

    [Fact(DisplayName = "B.O.M màu - Bước màu - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Bước màu - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("BUOC");
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { maBuoc = ma, tenBuoc = "Bước màu kiểm thử" },
            "Tạo bước B.O.M màu thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new { maBuoc = ma, tenBuoc = "Bước màu đã cập nhật", isActive = true, rowVersion },
            "Cập nhật bước B.O.M màu thành công");

        Assert.Equal("Bước màu đã cập nhật", updated.GetProperty("tenBuoc").GetString());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa bước B.O.M màu thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M màu - Bước màu - Trùng mã phải trả 409")]
    public async Task TaoMoi_TrungMa_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = new { maBuoc = BomApiKiemThuHelper.TaoMa("BUOC-TRUNG"), tenBuoc = "Bước trùng" };
        await BomApiKiemThuHelper.TaoMoiAsync(client, Route, request, "Tạo bước B.O.M màu thành công");
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.Conflict, "đã tồn tại");
    }

    [Fact(DisplayName = "B.O.M màu - Bước màu - RowVersion cũ phải trả 409")]
    public async Task CapNhat_RowVersionCu_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("BUOC-RV");
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { maBuoc = ma, tenBuoc = "Bước kiểm tra RowVersion" },
            "Tạo bước B.O.M màu thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        using var response = await client.PutAsJsonAsync(
            $"{Route}/{id}",
            new { maBuoc = ma, tenBuoc = "Không được cập nhật", isActive = true, rowVersion = DuLieuKiemThu.RowVersionHopLe });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.Conflict, "tải lại dữ liệu");
    }
}
