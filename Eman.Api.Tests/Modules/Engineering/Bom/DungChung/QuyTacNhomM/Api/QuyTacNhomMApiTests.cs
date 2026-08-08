using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Api;

public sealed class QuyTacNhomMApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/dung-chung/quy-tac-nhom-m";

    [Fact(DisplayName = "B.O.M dùng chung - Quy tắc nhóm M - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Quy tắc nhóm M - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new
            {
                hinhDangId = DuLieuKiemThu.HinhDangBomId,
                dienTichTu = 0m,
                dienTichDen = 2m,
                baoGomTu = true,
                baoGomDen = false,
                nhomMId = DuLieuKiemThu.NhomMBomId,
                ghiChu = "Quy tắc kiểm thử"
            },
            "Tạo quy tắc nhóm M thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal("BOM_MAU", detail.GetProperty("phamViBom").GetString());
        Assert.Equal("M-TEST", detail.GetProperty("maNhomM").GetString());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client,
            Route,
            id,
            new
            {
                hinhDangId = DuLieuKiemThu.HinhDangBomId,
                dienTichTu = 0m,
                dienTichDen = 2.5m,
                baoGomTu = true,
                baoGomDen = true,
                nhomMId = DuLieuKiemThu.NhomMBomId,
                ghiChu = "Đã cập nhật",
                isActive = true,
                rowVersion
            },
            "Cập nhật quy tắc nhóm M thành công");

        Assert.Equal(2.5m, updated.GetProperty("dienTichDen").GetDecimal());
        Assert.Equal("Đã cập nhật", updated.GetProperty("ghiChu").GetString());

        await BomApiKiemThuHelper.XoaAsync(
            client,
            Route,
            id,
            "Xóa quy tắc nhóm M thành công",
            BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Quy tắc nhóm M - Khoảng diện tích chồng lấn phải trả 409")]
    public async Task TaoMoi_KhoangDienTichChongLan_PhaiTra409()
    {
        using var client = factory.CreateClient();
        const string routeNhomM = "/api/engineering/bom/dung-chung/nhom-m";

        var nhomMKhac = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            routeNhomM,
            new
            {
                phamViBom = "BOM_MAU",
                maNhomM = BomApiKiemThuHelper.TaoMa("M-CHONG-LAN"),
                tenNhomM = "Nhóm M kiểm tra chồng lấn",
                thuTu = 103
            },
            "Tạo nhóm M thành công");

        var quyTacGoc = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new
            {
                hinhDangId = DuLieuKiemThu.HinhDangBomId,
                dienTichTu = 0m,
                dienTichDen = 1m,
                baoGomTu = true,
                baoGomDen = true,
                nhomMId = DuLieuKiemThu.NhomMBomId
            },
            "Tạo quy tắc nhóm M thành công");

        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                hinhDangId = DuLieuKiemThu.HinhDangBomId,
                dienTichTu = 0.5m,
                dienTichDen = 1.5m,
                baoGomTu = true,
                baoGomDen = true,
                nhomMId = BomApiKiemThuHelper.LayId(nhomMKhac)
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.Conflict,
            "chồng lấn");

        await BomApiKiemThuHelper.XoaAsync(
            client,
            Route,
            BomApiKiemThuHelper.LayId(quyTacGoc),
            "Xóa quy tắc nhóm M thành công",
            BomApiKiemThuHelper.LayRowVersion(quyTacGoc));

        await BomApiKiemThuHelper.XoaAsync(
            client,
            routeNhomM,
            BomApiKiemThuHelper.LayId(nhomMKhac),
            "Xóa nhóm M thành công",
            BomApiKiemThuHelper.LayRowVersion(nhomMKhac));
    }

    [Theory(DisplayName = "B.O.M dùng chung - Quy tắc nhóm M - Khoảng diện tích sai phải trả 400")]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    public async Task TaoMoi_KhoangDienTichKhongHopLe_PhaiTra400(decimal tu, decimal den)
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                hinhDangId = DuLieuKiemThu.HinhDangBomId,
                dienTichTu = tu,
                dienTichDen = den,
                baoGomTu = true,
                baoGomDen = false,
                nhomMId = DuLieuKiemThu.NhomMBomId
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest);
    }
}
