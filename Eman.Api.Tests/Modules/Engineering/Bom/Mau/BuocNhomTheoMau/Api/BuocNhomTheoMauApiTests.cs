using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Api;

public sealed class BuocNhomTheoMauApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/buoc-nhom-theo-mau";

    [Fact(DisplayName = "B.O.M màu - Bước nhóm theo màu - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Bước nhóm theo màu - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var tenBuoc = $"Bước {BomApiKiemThuHelper.TaoMa("NHOM")}";
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                mauSacId = DuLieuKiemThu.MauSacHe66Id,
                maBuoc = "BUOC-NHOM-API",
                tenBuoc,
                maHonHopId = 7101,
                maHonHop = "HH-API-7101",
                ghiChu = "Tạo từ API test"
            },
            "Tạo bước nhóm theo màu thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                mauSacId = DuLieuKiemThu.MauSacHe66Id,
                maBuoc = "BUOC-NHOM-API",
                tenBuoc,
                maHonHopId = 7101,
                maHonHop = "HH-API-7101-UPD",
                ghiChu = "Đã sửa",
                isActive = true,
                rowVersion
            },
            "Cập nhật bước nhóm theo màu thành công");

        Assert.Equal("HH-API-7101-UPD", updated.GetProperty("maHonHop").GetString());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa bước nhóm theo màu thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M màu - Bước nhóm theo màu - Màu không thuộc đề tài phải trả 400")]
    public async Task TaoMoi_MauKhongThuocDeTai_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                heSanPhamId = DuLieuKiemThu.HeSanPham68Id,
                deTaiId = DuLieuKiemThu.DeTaiHe66Id,
                mauSacId = DuLieuKiemThu.MauSacHe68Id,
                maBuoc = "BUOC-SAI-QUAN-HE",
                tenBuoc = "Bước sai quan hệ",
                maHonHopId = 7201,
                maHonHop = "HH-SAI-7201"
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.BadRequest, "không thuộc đề tài");
    }

    [Fact(DisplayName = "B.O.M màu - Bước nhóm theo màu - Trùng khóa phải trả 409")]
    public async Task TaoMoi_TrungKhoa_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = new
        {
            heSanPhamId = DuLieuKiemThu.HeSanPham66Id,
            deTaiId = DuLieuKiemThu.DeTaiHe66Id,
            mauSacId = DuLieuKiemThu.MauSacHe66Id,
            maBuoc = BomApiKiemThuHelper.TaoMa("BUOC-TRUNG"),
            tenBuoc = $"Bước {BomApiKiemThuHelper.TaoMa("TRUNG")}",
            maHonHopId = 7301,
            maHonHop = "HH-TRUNG-7301"
        };

        await BomApiKiemThuHelper.TaoMoiAsync(client, Route, request, "Tạo bước nhóm theo màu thành công");
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.Conflict, "đã tồn tại");
    }
}
