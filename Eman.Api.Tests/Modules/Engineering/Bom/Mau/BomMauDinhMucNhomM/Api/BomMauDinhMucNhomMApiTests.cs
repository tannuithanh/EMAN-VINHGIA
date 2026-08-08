using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Api;

public sealed class BomMauDinhMucNhomMApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/mau/dinh-muc-nhom-m";

    [Fact(DisplayName = "B.O.M màu - Định mức nhóm M - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M màu - Định mức nhóm M - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client, Route,
            new { buocNhomMauId = DuLieuKiemThu.BuocNhomTheoMauId, nhomMId = DuLieuKiemThu.NhomMBomId, dinhMuc = 1.25m, ghiChu = "Tạo từ API test" },
            "Tạo định mức nhóm M thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal(1.25m, detail.GetProperty("dinhMuc").GetDecimal());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client, Route, id,
            new { buocNhomMauId = DuLieuKiemThu.BuocNhomTheoMauId, nhomMId = DuLieuKiemThu.NhomMBomId, dinhMuc = 1.5m, ghiChu = "Đã sửa", isActive = true, rowVersion },
            "Cập nhật định mức nhóm M thành công");

        Assert.Equal(1.5m, updated.GetProperty("dinhMuc").GetDecimal());
        await BomApiKiemThuHelper.XoaAsync(client, Route, id, "Xóa định mức nhóm M thành công", BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M màu - Định mức nhóm M - Nhóm B.O.M thô phải trả 400")]
    public async Task TaoMoi_NhomMThuocBomTho_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new
            {
                buocNhomMauId = DuLieuKiemThu.BuocNhomTheoMauId,
                nhomMId = DuLieuKiemThu.NhomMThoBomId,
                dinhMuc = 1m
            });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "chỉ được sử dụng nhóm M thuộc phạm vi BOM_MAU");
    }

    [Fact(DisplayName = "B.O.M màu - Định mức nhóm M - Giá trị âm phải trả 400")]
    public async Task TaoMoi_DinhMucAm_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            new { buocNhomMauId = DuLieuKiemThu.BuocNhomTheoMauId, nhomMId = DuLieuKiemThu.NhomMBomId, dinhMuc = -1m });

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }
}
