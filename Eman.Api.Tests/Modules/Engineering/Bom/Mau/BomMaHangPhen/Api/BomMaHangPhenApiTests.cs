using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.BomMaHangPhen.Api;

public sealed class BomMaHangPhenApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/ma-hang-phen";

    [Fact(DisplayName = "B.O.M màu - Phên theo mã hàng - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Phên theo mã hàng - API mã hàng có phên phải trả đúng dữ liệu")]
    public async Task LayDanhSachMaHangCoPhen_PhaiTraDungDuLieu()
    {
        using var client = factory.CreateClient();
        var maPhen = ($"PHEN-TONG-HOP-{Guid.NewGuid():N}".ToUpperInvariant())[..28];
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new
            {
                maHangId = DuLieuKiemThu.MaHangBomId,
                maHangPhen = maPhen,
                ghiChu = "Kiểm tra API mã hàng có phên"
            },
            "Tạo phên theo mã hàng thành công");

        try
        {
            using var response = await client.GetAsync(
                $"{Route}/ma-hang-co-phen?maHangId={DuLieuKiemThu.MaHangBomId}&page=1&pageSize=20");
            await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.OK);

            var data = await ApiKiemThuHelper.LayDataAsync(response);
            var items = data.GetProperty("items");
            Assert.Single(items.EnumerateArray());

            var item = items[0];
            Assert.Equal(DuLieuKiemThu.MaHangBomId, item.GetProperty("maHangId").GetInt64());
            Assert.Equal("66-TEST-BOM", item.GetProperty("maHang").GetString());
            Assert.Equal(maPhen, item.GetProperty("maHangPhen").GetString());
            Assert.True(Guid.TryParse(item.GetProperty("cauHinhPhenId").GetString(), out _));
        }
        finally
        {
            await BomApiKiemThuHelper.XoaAsync(
                client,
                Route,
                BomApiKiemThuHelper.LayGuidId(created),
                "Xóa phên theo mã hàng thành công",
                BomApiKiemThuHelper.LayRowVersion(created));
        }
    }

    [Fact(DisplayName = "B.O.M màu - Phên theo mã hàng - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var maPhen = ($"PHEN-{Guid.NewGuid():N}".ToUpperInvariant())[..20];
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { maHangId = DuLieuKiemThu.MaHangBomId, maHangPhen = maPhen, ghiChu = "Tạo từ API test" },
            "Tạo phên theo mã hàng thành công");

        Assert.Equal(System.Text.Json.JsonValueKind.String, created.GetProperty("id").ValueKind);
        var id = BomApiKiemThuHelper.LayGuidId(created);
        Assert.NotEqual(Guid.Empty, id);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);

        var maCapNhat = ($"PHEN-UPD-{Guid.NewGuid():N}".ToUpperInvariant())[..24];
        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new { maHangId = DuLieuKiemThu.MaHangBomId, maHangPhen = maCapNhat, ghiChu = "Đã sửa", isActive = true, rowVersion },
            "Cập nhật phên theo mã hàng thành công");

        Assert.Equal(maCapNhat, updated.GetProperty("maHangPhen").GetString());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa phên theo mã hàng thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M màu - Phên theo mã hàng - Mã hàng không tồn tại phải trả 404")]
    public async Task TaoMoi_MaHangKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new { maHangId = long.MaxValue, maHangPhen = "PHEN-KHONG-TON-TAI" });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.NotFound, "Không tìm thấy mã hàng");
    }
}
