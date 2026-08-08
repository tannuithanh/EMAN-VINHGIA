using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.DungChung.MauSac.Api;

public sealed class MauSacApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/dung-chung/mau-sac";

    [Fact(DisplayName = "B.O.M dùng chung - Màu sắc - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Màu sắc - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("MAU");
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                maMau = ma,
                tenMau = "Màu kiểm thử",
                maCotTho = "b1",
                moTa = "Tạo từ API test"
            },
            "Tạo màu sắc thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal(DuLieuKiemThu.HeSanPham66Id, detail.GetProperty("heSanPhamId").GetInt64());
        Assert.Equal(DuLieuKiemThu.DeTaiHe66Id, detail.GetProperty("deTaiId").GetInt64());
        Assert.Equal("B1", detail.GetProperty("maCotTho").GetString());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                maMau = ma,
                tenMau = "Màu đã cập nhật",
                maCotTho = (string?)null,
                moTa = "Đã sửa",
                isActive = true,
                rowVersion
            },
            "Cập nhật màu sắc thành công");

        Assert.Equal("Màu đã cập nhật", updated.GetProperty("tenMau").GetString());
        Assert.Equal(System.Text.Json.JsonValueKind.Null, updated.GetProperty("maCotTho").ValueKind);
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa màu sắc thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Màu sắc - Đề tài không tồn tại phải trả 404")]
    public async Task TaoMoi_DeTaiKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = long.MaxValue,
                maMau = BomApiKiemThuHelper.TaoMa("MAU"),
                tenMau = "Màu sai đề tài"
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy đề tài");
    }

    [Fact(DisplayName = "B.O.M dùng chung - Màu sắc - Đề tài không thuộc hệ phải trả 400")]
    public async Task TaoMoi_DeTaiKhongThuocHe_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham68Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                maMau = BomApiKiemThuHelper.TaoMa("MAU-SAI-HE"),
                tenMau = "Màu sai hệ"
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Đề tài không thuộc hệ sản phẩm đã chọn");
    }

    [Fact(DisplayName = "B.O.M dùng chung - Màu sắc - Trùng mã trong cùng hệ và đề tài phải trả 409")]
    public async Task TaoMoi_TrungMaTrongCungHeVaDeTai_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = new
        {
            heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
            deTaiId = DuLieuKiemThu.DeTaiHe66Id,
            maMau = BomApiKiemThuHelper.TaoMa("MAU-TRUNG"),
            tenMau = "Màu trùng"
        };

        await BomApiKiemThuHelper.TaoMoiAsync(client, Route, request, "Tạo màu sắc thành công");
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.Conflict, "đã tồn tại");
    }
}
