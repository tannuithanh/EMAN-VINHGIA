using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.DungChung.MaHang.Api;

public sealed class MaHangApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/dung-chung/ma-hang";

    [Fact(DisplayName = "B.O.M dùng chung - Mã hàng - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Mã hàng - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = $"66-{BomApiKiemThuHelper.TaoMa("MH")}";
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new
            {
                maHang = ma,
                dienTich = 1.5m,
                hinhDangBomThoId = DuLieuKiemThu.HinhDangBomId,
                hinhDangBomMauId = DuLieuKiemThu.HinhDangBomId,
                moTa = "Tạo từ API test"
            },
            "Tạo mã hàng thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal(ma, detail.GetProperty("maHang").GetString());
        Assert.Equal(DuLieuKiemThu.HinhDangBomId, detail.GetProperty("hinhDangBomThoId").GetInt64());
        Assert.Equal(DuLieuKiemThu.HinhDangBomId, detail.GetProperty("hinhDangBomMauId").GetInt64());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client,
            Route,
            id,
            new
            {
                maHang = ma,
                dienTich = 1.75m,
                hinhDangBomThoId = DuLieuKiemThu.HinhDangBomId,
                hinhDangBomMauId = (long?)null,
                moTa = "Đã sửa",
                isActive = true,
                rowVersion
            },
            "Cập nhật mã hàng thành công");

        Assert.Equal(1.75m, updated.GetProperty("dienTich").GetDecimal());
        Assert.Equal(DuLieuKiemThu.HinhDangBomId, updated.GetProperty("hinhDangBomThoId").GetInt64());
        Assert.Equal(System.Text.Json.JsonValueKind.Null, updated.GetProperty("hinhDangBomMauId").ValueKind);

        await BomApiKiemThuHelper.XoaAsync(
            client,
            Route,
            id,
            "Xóa mã hàng thành công",
            BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Mã hàng - Hình dáng không tồn tại phải trả 404")]
    public async Task TaoMoi_HinhDangKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                maHang = $"66-{BomApiKiemThuHelper.TaoMa("MH")}",
                dienTich = 1m,
                hinhDangBomThoId = DuLieuKiemThu.HinhDangBomId,
                hinhDangBomMauId = long.MaxValue
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.NotFound,
            "Không tìm thấy hình dáng B.O.M màu");
    }

    [Fact(DisplayName = "B.O.M dùng chung - Mã hàng - Không có hình dáng phải trả 400")]
    public async Task TaoMoi_KhongCoHinhDang_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                maHang = $"66-{BomApiKiemThuHelper.TaoMa("MH")}",
                dienTich = 1m
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "phải có ít nhất một hình dáng");
    }

    [Fact(DisplayName = "B.O.M dùng chung - Mã hàng - Diện tích âm phải trả 400")]
    public async Task TaoMoi_DienTichAm_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                maHang = $"66-{BomApiKiemThuHelper.TaoMa("MH")}",
                dienTich = -1m,
                hinhDangBomThoId = DuLieuKiemThu.HinhDangBomId
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "Dữ liệu không hợp lệ");
    }
}
