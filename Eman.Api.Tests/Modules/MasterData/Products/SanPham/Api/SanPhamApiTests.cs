using System.Net;
using System.Net.Http.Json;
using Eman.Api.Tests.Infrastructure;
using Eman.Application.Modules.MasterData.Products.SanPham.Dtos;
using Eman.Domain.Common.Enums;
using SanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.SanPham;
using Eman.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Api.Tests.Modules.MasterData.Products.SanPham.Api;

/// <summary>
/// Kiểm tra API Sản phẩm theo đúng DataAnnotations và quy tắc trong SanPhamService.
/// </summary>
public sealed class SanPhamApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/master-data/san-pham";

    [Fact(DisplayName = "Sản phẩm - Tạo đầy đủ dữ liệu hợp lệ phải thành công")]
    public async Task TaoSanPham_DuLieuHopLe_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = TaoMa("SP");
        var request = TaoRequestHopLe(
            ma,
            nhomNangLucId: DuLieuKiemThu.NhomNangLucHoatDongId,
            khoMacDinhId: DuLieuKiemThu.KhoMacDinhId,
            khoTonId: DuLieuKiemThu.KhoTonId,
            xuongMacDinhId: DuLieuKiemThu.PhanXuong1Id,
            thueId: DuLieuKiemThu.ThueHoatDongId,
            chieuDaiCm: 10,
            chieuRongCm: 20,
            chieuCaoCm: 30,
            trongLuong: 5,
            laBanThanhPham: true);

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Created, "Tạo sản phẩm thành công");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(ma, data.GetProperty("maSanPham").GetString());
        Assert.True(data.GetProperty("laBanThanhPham").GetBoolean());
        Assert.Equal(DuLieuKiemThu.KhoTonId, data.GetProperty("khoTonId").GetGuid());
    }

    [Theory(DisplayName = "Sản phẩm - Thiếu trường chuỗi bắt buộc phải trả 400")]
    [InlineData("ma")]
    [InlineData("moTa")]
    public async Task TaoSanPham_ThieuChuoiBatBuoc_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "ma" => TaoRequestHopLe(string.Empty),
            "moTa" => TaoRequestHopLe(TaoMa("SP"), moTaTiengViet: string.Empty),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Theory(DisplayName = "Sản phẩm - Chuỗi vượt độ dài phải trả 400")]
    [InlineData("ma")]
    [InlineData("moTaViet")]
    [InlineData("moTaAnh")]
    [InlineData("noiGiao")]
    [InlineData("ghiChu")]
    [InlineData("nguoiTao")]
    public async Task TaoSanPham_ChuoiVuotDoDai_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "ma" => TaoRequestHopLe(new string('A', 101)),
            "moTaViet" => TaoRequestHopLe(TaoMa("SP"), moTaTiengViet: new string('A', 501)),
            "moTaAnh" => TaoRequestHopLe(TaoMa("SP"), moTaTiengAnh: new string('A', 501)),
            "noiGiao" => TaoRequestHopLe(TaoMa("SP"), noiGiaoHang: new string('A', 501)),
            "ghiChu" => TaoRequestHopLe(TaoMa("SP"), ghiChu: new string('A', 1001)),
            "nguoiTao" => TaoRequestHopLe(TaoMa("SP"), createdByMsnv: new string('A', 51)),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Theory(DisplayName = "Sản phẩm - Các số âm phải trả 400")]
    [InlineData("dai")]
    [InlineData("rong")]
    [InlineData("cao")]
    [InlineData("trongLuong")]
    [InlineData("dienTich")]
    [InlineData("doKho")]
    [InlineData("tiTrong")]
    [InlineData("cbm")]
    public async Task TaoSanPham_SoAm_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "dai" => TaoRequestHopLe(TaoMa("SP"), chieuDaiCm: -1),
            "rong" => TaoRequestHopLe(TaoMa("SP"), chieuRongCm: -1),
            "cao" => TaoRequestHopLe(TaoMa("SP"), chieuCaoCm: -1),
            "trongLuong" => TaoRequestHopLe(TaoMa("SP"), trongLuong: -1),
            "dienTich" => TaoRequestHopLe(TaoMa("SP"), dienTich: -1),
            "doKho" => TaoRequestHopLe(TaoMa("SP"), doKho: -1),
            "tiTrong" => TaoRequestHopLe(TaoMa("SP"), heSoTiTrong: -1),
            "cbm" => TaoRequestHopLe(TaoMa("SP"), cbmMacDinh: -1),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Fact(DisplayName = "Sản phẩm - Thiếu đơn vị tính phải trả 400")]
    public async Task TaoSanPham_ThieuDonViTinh_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route, TaoRequestHopLe(TaoMa("SP"), donViTinhId: Guid.Empty));
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Đơn vị tính là bắt buộc");
    }

    [Fact(DisplayName = "Sản phẩm - Đơn vị tính không tồn tại phải trả 404")]
    public async Task TaoSanPham_DonViTinhKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route, TaoRequestHopLe(TaoMa("SP"), donViTinhId: Guid.NewGuid()));
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy đơn vị tính");
    }

    [Fact(DisplayName = "Sản phẩm - Đơn vị tính ngừng hoạt động phải bị từ chối")]
    public async Task TaoSanPham_DonViTinhNgung_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route, TaoRequestHopLe(TaoMa("SP"), donViTinhId: DuLieuKiemThu.DonViTinhNgungId));
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Đơn vị tính đã ngừng hoạt động");
    }

    [Theory(DisplayName = "Sản phẩm - Danh mục tùy chọn không tồn tại phải trả 404")]
    [InlineData("nhom")]
    [InlineData("khoMacDinh")]
    [InlineData("khoTon")]
    [InlineData("xuong")]
    [InlineData("thue")]
    public async Task TaoSanPham_DanhMucTuyChonKhongTonTai_PhaiTra404(string truong)
    {
        using var client = factory.CreateClient();
        var id = Guid.NewGuid();
        var request = truong switch
        {
            "nhom" => TaoRequestHopLe(TaoMa("SP"), nhomNangLucId: id),
            "khoMacDinh" => TaoRequestHopLe(TaoMa("SP"), khoMacDinhId: id),
            "khoTon" => TaoRequestHopLe(TaoMa("SP"), khoTonId: id),
            "xuong" => TaoRequestHopLe(TaoMa("SP"), xuongMacDinhId: id),
            "thue" => TaoRequestHopLe(TaoMa("SP"), thueId: id),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy");
    }

    [Theory(DisplayName = "Sản phẩm - Danh mục tùy chọn ngừng hoạt động phải bị từ chối")]
    [InlineData("nhom")]
    [InlineData("khoMacDinh")]
    [InlineData("khoTon")]
    [InlineData("xuong")]
    [InlineData("thue")]
    public async Task TaoSanPham_DanhMucTuyChonNgung_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "nhom" => TaoRequestHopLe(TaoMa("SP"), nhomNangLucId: DuLieuKiemThu.NhomNangLucNgungId),
            "khoMacDinh" => TaoRequestHopLe(TaoMa("SP"), khoMacDinhId: DuLieuKiemThu.KhoNgungId),
            "khoTon" => TaoRequestHopLe(TaoMa("SP"), khoTonId: DuLieuKiemThu.KhoNgungId),
            "xuong" => TaoRequestHopLe(TaoMa("SP"), xuongMacDinhId: DuLieuKiemThu.PhanXuongNgungId),
            "thue" => TaoRequestHopLe(TaoMa("SP"), thueId: DuLieuKiemThu.ThueNgungId),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "đã ngừng hoạt động");
    }

    [Fact(DisplayName = "Sản phẩm - Kho tồn không đánh dấu hàng tồn phải bị từ chối")]
    public async Task TaoSanPham_KhoTonKhongPhaiHangTon_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route, TaoRequestHopLe(TaoMa("SP"), khoTonId: DuLieuKiemThu.KhoKhongPhaiHangTonId));
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "phải là kho có đánh dấu hàng tồn");
    }

    [Fact(DisplayName = "Sản phẩm - Kho mặc định và kho tồn giống nhau phải bị từ chối")]
    public async Task TaoSanPham_KhoMacDinhVaKhoTonGiongNhau_PhaiTra400()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route,
            TaoRequestHopLe(
                TaoMa("SP"),
                khoMacDinhId: DuLieuKiemThu.KhoTonId,
                khoTonId: DuLieuKiemThu.KhoTonId));
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "không được giống nhau");
    }

    [Fact(DisplayName = "Sản phẩm - Mã trùng phải trả 409")]
    public async Task TaoSanPham_MaTrung_PhaiTra409()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route, TaoRequestHopLe(DuLieuKiemThu.MaSanPhamCoSan.ToLowerInvariant()));
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "đã tồn tại");
    }

    [Fact(DisplayName = "Sản phẩm - ID từ Trading bị trùng phải trả 409")]
    public async Task TaoSanPham_IdTrung_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = TaoRequestHopLe(TaoMa("SP"), id: DuLieuKiemThu.SanPhamCoSanId);
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "ID sản phẩm");
    }

    [Fact(DisplayName = "Sản phẩm - Lấy ID không tồn tại phải trả 404")]
    public async Task LaySanPham_IdKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync($"{Route}/{Guid.NewGuid()}");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy sản phẩm");
    }

    [Theory(DisplayName = "Sản phẩm - Phân trang hoặc trạng thái sai phải trả 400")]
    [InlineData("?page=0")]
    [InlineData("?pageSize=0")]
    [InlineData("?pageSize=201")]
    [InlineData("?trangThai=2")]
    public async Task LayDanhSach_ThamSoKhongHopLe_PhaiTra400(string query)
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync(Route + query);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Fact(DisplayName = "Sản phẩm - Cập nhật ID không tồn tại phải trả 404")]
    public async Task CapNhatSanPham_IdKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        var request = TaoCapNhatHopLe(TaoMa("SP"), DuLieuKiemThu.RowVersionHopLe);
        using var response = await client.PutAsJsonAsync($"{Route}/{Guid.NewGuid()}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy sản phẩm");
    }

    [Fact(DisplayName = "Sản phẩm - RowVersion sai định dạng phải trả 400")]
    public async Task CapNhatSanPham_RowVersionSaiDinhDang_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var request = TaoCapNhatHopLe(TaoMa("SP"), "khong-phai-base64");
        using var response = await client.PutAsJsonAsync(
            $"{Route}/{DuLieuKiemThu.SanPhamCoSanId}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "RowVersion không đúng định dạng Base64");
    }

    [Fact(DisplayName = "Sản phẩm - RowVersion cũ phải trả 409")]
    public async Task CapNhatSanPham_RowVersionCu_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = TaoCapNhatHopLe(TaoMa("SP"), "CQkJCQ==");
        using var response = await client.PutAsJsonAsync(
            $"{Route}/{DuLieuKiemThu.SanPhamCoSanId}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "Dữ liệu đã được người khác cập nhật");
    }

    [Fact(DisplayName = "Sản phẩm - Cập nhật hợp lệ phải lưu dữ liệu mới")]
    public async Task CapNhatSanPham_HopLe_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var id = await ThemSanPhamTrucTiepAsync(TaoMa("SP-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var maMoi = TaoMa("SP-NEW");
        var request = TaoCapNhatHopLe(
            maMoi,
            rowVersion,
            moTaTiengViet: "Mô tả sau cập nhật",
            trangThai: 0);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.OK, "Cập nhật sản phẩm thành công");
        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(maMoi, data.GetProperty("maSanPham").GetString());
        Assert.Equal(0, data.GetProperty("trangThai").GetByte());
    }

    [Fact(DisplayName = "Sản phẩm - Cập nhật sang mã đã tồn tại phải trả 409")]
    public async Task CapNhatSanPham_MaTrung_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var id = await ThemSanPhamTrucTiepAsync(TaoMa("SP-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(DuLieuKiemThu.MaSanPhamCoSan, rowVersion);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "đã tồn tại");
    }

    [Fact(DisplayName = "Sản phẩm - Xóa hợp lệ thì GET lại phải trả 404")]
    public async Task XoaSanPham_HopLe_PhaiKhongConDuLieu()
    {
        using var client = factory.CreateClient();
        var id = await ThemSanPhamTrucTiepAsync(TaoMa("SP-DEL"));
        var rowVersion = await LayRowVersionAsync(client, id);

        using var deleteResponse = await client.DeleteAsync(
            $"{Route}/{id}?rowVersion={Uri.EscapeDataString(rowVersion)}");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            deleteResponse, HttpStatusCode.OK, "Xóa sản phẩm thành công");

        using var getResponse = await client.GetAsync($"{Route}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static TaoSanPhamRequest TaoRequestHopLe(
        string maSanPham,
        string moTaTiengViet = "Sản phẩm kiểm thử",
        Guid? id = null,
        string? moTaTiengAnh = null,
        Guid? donViTinhId = null,
        Guid? nhomNangLucId = null,
        decimal? chieuDaiCm = null,
        decimal? chieuRongCm = null,
        decimal? chieuCaoCm = null,
        decimal? trongLuong = null,
        decimal? dienTich = null,
        decimal? doKho = null,
        decimal? heSoTiTrong = null,
        decimal? cbmMacDinh = null,
        Guid? khoMacDinhId = null,
        Guid? khoTonId = null,
        Guid? xuongMacDinhId = null,
        Guid? thueId = null,
        bool laBanThanhPham = false,
        string? noiGiaoHang = null,
        string? ghiChu = null,
        string? createdByMsnv = "TEST")
        => new()
        {
            Id = id,
            MaSanPham = maSanPham,
            MoTaTiengViet = moTaTiengViet,
            MoTaTiengAnh = moTaTiengAnh,
            DonViTinhId = donViTinhId ?? DuLieuKiemThu.DonViTinhHoatDongId,
            NhomNangLucId = nhomNangLucId,
            ChieuDaiCm = chieuDaiCm,
            ChieuRongCm = chieuRongCm,
            ChieuCaoCm = chieuCaoCm,
            TrongLuong = trongLuong,
            DienTich = dienTich,
            DoKho = doKho,
            HeSoTiTrong = heSoTiTrong,
            CbmMacDinh = cbmMacDinh,
            KhoMacDinhId = khoMacDinhId,
            KhoTonId = khoTonId,
            XuongMacDinhId = xuongMacDinhId,
            ThueId = thueId,
            LaBanThanhPham = laBanThanhPham,
            NoiGiaoHang = noiGiaoHang,
            GhiChu = ghiChu,
            CreatedByMsnv = createdByMsnv
        };

    private static CapNhatSanPhamRequest TaoCapNhatHopLe(
        string maSanPham,
        string rowVersion,
        string moTaTiengViet = "Sản phẩm cập nhật",
        byte trangThai = 1)
        => new()
        {
            MaSanPham = maSanPham,
            MoTaTiengViet = moTaTiengViet,
            DonViTinhId = DuLieuKiemThu.DonViTinhHoatDongId,
            TrangThai = trangThai,
            UpdatedByMsnv = "TEST",
            RowVersion = rowVersion
        };

    private async Task<Guid> ThemSanPhamTrucTiepAsync(string ma)
    {
        var id = Guid.NewGuid();
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmanDbContext>();
        dbContext.SanPhams.Add(new SanPhamEntity
        {
            Id = id,
            MaSanPham = ma,
            MoTaTiengViet = "Sản phẩm trực tiếp",
            DonViTinhId = DuLieuKiemThu.DonViTinhHoatDongId,
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(DuLieuKiemThu.RowVersionHopLe)
        });
        await dbContext.SaveChangesAsync();
        return id;
    }

    private static async Task<string> LayRowVersionAsync(HttpClient client, Guid id)
    {
        using var response = await client.GetAsync($"{Route}/{id}");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(response, HttpStatusCode.OK);
        var data = await ApiKiemThuHelper.LayDataAsync(response);
        return data.GetProperty("rowVersion").GetString()
            ?? throw new InvalidOperationException("API không trả RowVersion.");
    }

    private static string TaoMa(string prefix)
        => $"{prefix}-{Guid.NewGuid():N}"[..Math.Min(100, prefix.Length + 33)].ToUpperInvariant();
}
