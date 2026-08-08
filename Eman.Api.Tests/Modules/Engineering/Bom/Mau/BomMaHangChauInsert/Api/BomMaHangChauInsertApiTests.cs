using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Api;

public sealed class BomMaHangChauInsertApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/ma-hang-chau-insert";

    [Fact(DisplayName = "B.O.M màu - Chậu insert theo mã hàng - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Chậu insert theo mã hàng - API mã hàng có insert phải trả đúng số lượng")]
    public async Task LayDanhSachMaHangCoChauInsert_PhaiTraDungSoLuong()
    {
        using var client = factory.CreateClient();
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new
            {
                maHangId = DuLieuKiemThu.MaHangBomId,
                chauInsertId = DuLieuKiemThu.ChauInsertBomId,
                soLuong = 3,
                ghiChu = "Kiểm tra API tổng hợp chậu insert"
            },
            "Tạo chậu insert theo mã hàng thành công");

        try
        {
            using var response = await client.GetAsync(
                $"{Route}/ma-hang-co-chau-insert?maHangId={DuLieuKiemThu.MaHangBomId}&page=1&pageSize=20");
            await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.OK);

            var data = await ApiKiemThuHelper.LayDataAsync(response);
            var items = data.GetProperty("items");
            Assert.Single(items.EnumerateArray());

            var item = items[0];
            Assert.Equal("66-TEST-BOM", item.GetProperty("maHang").GetString());
            Assert.Equal(1, item.GetProperty("soLoaiChauInsert").GetInt32());
            Assert.Equal(3, item.GetProperty("tongSoLuongChauInsert").GetInt32());

            var danhSach = item.GetProperty("danhSachChauInsert");
            Assert.Single(danhSach.EnumerateArray());
            Assert.Equal(3, danhSach[0].GetProperty("soLuong").GetInt32());
            Assert.True(Guid.TryParse(danhSach[0].GetProperty("cauHinhChauInsertId").GetString(), out _));
            Assert.True(Guid.TryParse(danhSach[0].GetProperty("chauInsertId").GetString(), out _));
        }
        finally
        {
            await BomApiKiemThuHelper.XoaAsync(
                client,
                Route,
                BomApiKiemThuHelper.LayGuidId(created),
                "Xóa chậu insert theo mã hàng thành công",
                BomApiKiemThuHelper.LayRowVersion(created));
        }
    }

    [Fact(DisplayName = "B.O.M màu - Chậu insert theo mã hàng - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new
            {
                maHangId = DuLieuKiemThu.MaHangBomId,
                chauInsertId = DuLieuKiemThu.ChauInsertBomId,
                soLuong = 2,
                ghiChu = "Tạo từ API test"
            },
            "Tạo chậu insert theo mã hàng thành công");

        Assert.Equal(System.Text.Json.JsonValueKind.String, created.GetProperty("id").ValueKind);
        Assert.True(Guid.TryParse(created.GetProperty("chauInsertId").GetString(), out _));
        var id = BomApiKiemThuHelper.LayGuidId(created);
        Assert.NotEqual(Guid.Empty, id);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal("66-TEST-BOM", detail.GetProperty("maHang").GetString());
        Assert.Equal(2, detail.GetProperty("soLuong").GetInt32());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new
            {
                maHangId = DuLieuKiemThu.MaHangBomId,
                chauInsertId = DuLieuKiemThu.ChauInsertBomId,
                soLuong = 4,
                ghiChu = "Đã sửa",
                isActive = true,
                rowVersion
            },
            "Cập nhật chậu insert theo mã hàng thành công");

        Assert.Equal("Đã sửa", updated.GetProperty("ghiChu").GetString());
        Assert.Equal(4, updated.GetProperty("soLuong").GetInt32());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa chậu insert theo mã hàng thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Theory(DisplayName = "B.O.M màu - Chậu insert theo mã hàng - Khóa ngoại không tồn tại phải trả 404")]
    [InlineData("maHang")]
    [InlineData("chauInsert")]
    public async Task TaoMoi_KhoaNgoaiKhongTonTai_PhaiTra404(string truong)
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                maHangId = truong == "maHang" ? long.MaxValue : DuLieuKiemThu.MaHangBomId,
                chauInsertId = truong == "chauInsert" ? Guid.NewGuid() : DuLieuKiemThu.ChauInsertBomId,
                soLuong = 1,
                ghiChu = "Sai khóa ngoại"
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.NotFound, "Không tìm thấy");
    }

    [Fact(DisplayName = "B.O.M màu - Chậu insert theo mã hàng - Số lượng bằng 0 phải trả 400")]
    public async Task TaoMoi_SoLuongBangKhong_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                maHangId = DuLieuKiemThu.MaHangBomId,
                chauInsertId = DuLieuKiemThu.ChauInsertBomId,
                soLuong = 0
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "Số lượng chậu insert phải lớn hơn hoặc bằng 1");
    }
}
