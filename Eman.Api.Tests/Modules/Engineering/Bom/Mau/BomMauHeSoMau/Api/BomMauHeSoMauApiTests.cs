using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Api;

public sealed class BomMauHeSoMauApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/he-so-mau";

    [Fact(DisplayName = "B.O.M màu - Hệ số màu - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Hệ số màu - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                mauSacId = DuLieuKiemThu.MauSacHe66Id,
                buocId = DuLieuKiemThu.BomMauBuocId,
                heSo = 1.15m,
                ghiChu = "Tạo từ API test"
            },
            "Tạo hệ số màu thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal("MAU-TEST-66", detail.GetProperty("maMau").GetString());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                mauSacId = DuLieuKiemThu.MauSacHe66Id,
                buocId = DuLieuKiemThu.BomMauBuocId,
                heSo = 1.25m,
                ghiChu = "Đã sửa",
                isActive = true,
                rowVersion
            },
            "Cập nhật hệ số màu thành công");

        Assert.Equal(1.25m, updated.GetProperty("heSo").GetDecimal());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa hệ số màu thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M màu - Hệ số màu - Màu không thuộc đề tài phải trả 400")]
    public async Task TaoMoi_MauKhongThuocDeTai_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                mauSacId = DuLieuKiemThu.MauSacHe68Id,
                buocId = DuLieuKiemThu.BomMauBuocId,
                heSo = 1m
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.BadRequest, "không thuộc đề tài");
    }
}
