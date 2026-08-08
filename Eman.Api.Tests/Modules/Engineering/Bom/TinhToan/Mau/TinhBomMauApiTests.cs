using System.Net;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.TinhToan.Mau;

public sealed class TinhBomMauApiTests(EmanApiFactory factory)
    : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/tinh-toan/mau/test";
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task KiemThu_DuCauHinh_TraChiTietTinhToanDayDu()
    {
        var url = $"{Route}?maSanPham={Uri.EscapeDataString(DuLieuKiemThu.MaSanPhamTinhBomMau)}";

        using var response = await _client.GetAsync(url);

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.OK,
            "Tính thử B.O.M màu thành công");
        var data = await ApiKiemThuHelper.LayDataAsync(response);

        Assert.True(data.GetProperty("daTinhThanhCong").GetBoolean());
        Assert.Equal("THANH_CONG", data.GetProperty("trangThai").GetString());
        Assert.Equal("66", data.GetProperty("phanTichMaSanPham").GetProperty("maHe").GetString());
        Assert.Equal("20", data.GetProperty("phanTichMaSanPham").GetProperty("maDeTai").GetString());
        Assert.Equal("02", data.GetProperty("phanTichMaSanPham").GetProperty("maMau").GetString());
        Assert.Equal("66-52220-01", data.GetProperty("phanTichMaSanPham").GetProperty("maHangNen").GetString());
        Assert.Equal("M-TINH-MAU", data.GetProperty("nhomM").GetProperty("maNhomM").GetString());

        var cacBuoc = data.GetProperty("cacBuoc");
        Assert.Equal(2, cacBuoc.GetArrayLength());
        Assert.All(cacBuoc.EnumerateArray(), item => Assert.True(item.GetProperty("daTinhDuoc").GetBoolean()));
        Assert.Equal(1.32m, cacBuoc[0].GetProperty("luongTieuHao").GetDecimal());
        Assert.Equal(0.5625m, cacBuoc[1].GetProperty("luongTieuHao").GetDecimal());

        var chauInserts = data.GetProperty("chauInserts");
        Assert.Single(chauInserts.EnumerateArray());
        Assert.Equal(2, chauInserts[0].GetProperty("soLuong").GetInt32());
        Assert.True(data.GetProperty("phen").GetProperty("coPhen").GetBoolean());
        Assert.Equal(
            "66-52220-01-B",
            data.GetProperty("cotTho").GetProperty("maHangCotThoDuKien").GetString());
        Assert.True(data.GetProperty("cotTho").GetProperty("tonTaiTrongDanhMuc").GetBoolean());
    }

    [Fact]
    public async Task KiemThu_MaKhongDungCauTruc_TraLoiChanDoan()
    {
        using var response = await _client.GetAsync(
            $"{Route}?maSanPham={Uri.EscapeDataString("66-SAI")}");

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.OK,
            "chưa thể tính đầy đủ");
        var data = await ApiKiemThuHelper.LayDataAsync(response);

        Assert.False(data.GetProperty("daTinhThanhCong").GetBoolean());
        Assert.Equal(
            "MA_SAN_PHAM_KHONG_HOP_LE",
            data.GetProperty("trangThai").GetString());
        Assert.NotEmpty(data.GetProperty("loiCauHinh").EnumerateArray());
    }
}
