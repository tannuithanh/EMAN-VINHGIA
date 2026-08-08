using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.DungChung.NhomM.Api;

public sealed class NhomMApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/dung-chung/nhom-m";

    [Fact(DisplayName = "B.O.M dùng chung - Nhóm M - Danh sách phải hoạt động")]
    public async Task LayDanhSach_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        await BomApiKiemThuHelper.KiemTraDanhSachAsync(client, Route);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Nhóm M - CRUD đầy đủ phải thành công")]
    public async Task CrudDayDu_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("M");
        var created = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new
            {
                phamViBom = "BOM_MAU",
                maNhomM = ma,
                tenNhomM = "Nhóm M kiểm thử",
                thuTu = 99,
                moTa = "Tạo từ API test"
            },
            "Tạo nhóm M thành công");

        var id = BomApiKiemThuHelper.LayId(created);
        var rowVersion = BomApiKiemThuHelper.LayRowVersion(created);
        var detail = await BomApiKiemThuHelper.LayTheoIdAsync(client, Route, id);
        Assert.Equal("BOM_MAU", detail.GetProperty("phamViBom").GetString());

        var updated = await BomApiKiemThuHelper.CapNhatAsync(
            client,
            Route,
            id,
            new
            {
                phamViBom = "BOM_MAU",
                maNhomM = ma,
                tenNhomM = "Nhóm M đã cập nhật",
                thuTu = 100,
                moTa = "Đã sửa",
                isActive = true,
                rowVersion
            },
            "Cập nhật nhóm M thành công");

        Assert.Equal("Nhóm M đã cập nhật", updated.GetProperty("tenNhomM").GetString());
        Assert.Equal(100, updated.GetProperty("thuTu").GetInt32());

        await BomApiKiemThuHelper.XoaAsync(
            client,
            Route,
            id,
            "Xóa nhóm M thành công",
            BomApiKiemThuHelper.LayRowVersion(updated));
        await BomApiKiemThuHelper.KiemTraDaXoaAsync(client, Route, id);
    }

    [Fact(DisplayName = "B.O.M dùng chung - Nhóm M - Trùng mã trong cùng phạm vi phải trả 409")]
    public async Task TaoMoi_TrungMaTrongCungPhamVi_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = new
        {
            phamViBom = "BOM_MAU",
            maNhomM = BomApiKiemThuHelper.TaoMa("M-TRUNG"),
            tenNhomM = "Nhóm M trùng",
            thuTu = 101
        };

        await BomApiKiemThuHelper.TaoMoiAsync(client, Route, request, "Tạo nhóm M thành công");
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.Conflict,
            "đã tồn tại");
    }

    [Fact(DisplayName = "B.O.M dùng chung - Nhóm M - Cùng mã khác phạm vi được phép tạo")]
    public async Task TaoMoi_CungMaKhacPhamVi_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = BomApiKiemThuHelper.TaoMa("M-2PV");

        var nhomMau = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new { phamViBom = "BOM_MAU", maNhomM = ma, tenNhomM = "Nhóm màu", thuTu = 102 },
            "Tạo nhóm M thành công");

        var nhomTho = await BomApiKiemThuHelper.TaoMoiAsync(
            client,
            Route,
            new { phamViBom = "BOM_THO", maNhomM = ma, tenNhomM = "Nhóm thô", thuTu = 102 },
            "Tạo nhóm M thành công");

        Assert.NotEqual(
            BomApiKiemThuHelper.LayId(nhomMau),
            BomApiKiemThuHelper.LayId(nhomTho));
    }
}
