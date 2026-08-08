using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.BomMauHeSoDeTai.Api;

public sealed class BomMauHeSoDeTaiApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/he-so-de-tai";

    [Fact(DisplayName = "B.O.M màu - Hệ số đề tài - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Hệ số đề tài - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { heSanPhamId = DuLieuKiemThu.HeSanPham66Id, deTaiId = DuLieuKiemThu.DeTaiHe66Id, buocId = DuLieuKiemThu.BomMauBuocId, heSo = 1.1m, ghiChu = "Tạo từ API test" },
            "Tạo hệ số đề tài thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal("66", detail.GetProperty("maHe").GetString());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new { heSanPhamId = DuLieuKiemThu.HeSanPham66Id, deTaiId = DuLieuKiemThu.DeTaiHe66Id, buocId = DuLieuKiemThu.BomMauBuocId, heSo = 1.2m, ghiChu = "Đã sửa", isActive = true, rowVersion },
            "Cập nhật hệ số đề tài thành công");

        Assert.Equal(1.2m, updated.GetProperty("heSo").GetDecimal());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa hệ số đề tài thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M màu - Hệ số đề tài - Đề tài không thuộc hệ phải trả 400")]
    public async Task TaoMoi_DeTaiKhongThuocHe_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new { heSanPhamId = DuLieuKiemThu.HeSanPham66Id, deTaiId = DuLieuKiemThu.DeTaiHe68Id, buocId = DuLieuKiemThu.BomMauBuocId, heSo = 1m });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.BadRequest, "không thuộc hệ sản phẩm");
    }
}
