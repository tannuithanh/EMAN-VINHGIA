using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.DungChung.DeTai.Api;

public sealed class DeTaiApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/dung-chung/de-tai";

    [Fact(DisplayName = "B.O.M dùng chung - Đề tài - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Đề tài - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("DT");

        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { heSanPhamId = DuLieuKiemThu.HeSanPham66Id, maDeTai = ma, tenDeTai = "Đề tài kiểm thử", moTa = "Tạo từ API test" },
            "Tạo đề tài thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);

        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal(DuLieuKiemThu.HeSanPham66Id, detail.GetProperty("heSanPhamId").GetInt64());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                maDeTai = ma,
                tenDeTai = "Đề tài đã cập nhật",
                moTa = "Đã sửa",
                isActive = true,
                rowVersion
            },
            "Cập nhật đề tài thành công");

        Assert.Equal("Đề tài đã cập nhật", updated.GetProperty("tenDeTai").GetString());
        await BomApiKiemThuHelper.XoaAsync(
            client, Route, id, "Xóa đề tài thành công",
            BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Đề tài - Hệ không tồn tại phải trả 404")]
    public async Task TaoMoi_HeKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                heSanPhamId = long.MaxValue,
                maDeTai = BomApiKiemThuHelper.TaoMa("DT"),
                tenDeTai = "Đề tài sai hệ"
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy hệ sản phẩm");
    }

    [Fact(DisplayName = "B.O.M dùng chung - Đề tài - Trùng mã trong cùng hệ phải trả 409")]
    public async Task TaoMoi_TrungMaTrongCungHe_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("DT-TRUNG");
        var request = new { heSanPhamId = DuLieuKiemThu.HeSanPham66Id, maDeTai = ma, tenDeTai = "Đề tài trùng" };

        await BomApiKiemThuHelper.TaoMoiAsync(client, Route, request, "Tạo đề tài thành công");
        using var response = await client.PostAsJsonAsync(Route, request);

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "đã tồn tại");
    }
}
