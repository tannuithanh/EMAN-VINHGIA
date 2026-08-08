using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Eman.Api.Tests.Infrastructure;
using Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;
using Eman.Domain.Common.Enums;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Eman.Api.Tests.Modules.MasterData.Materials.VatTu.Api;

/// <summary>
/// Kiểm tra API Vật tư theo đúng DataAnnotations và quy tắc trong VatTuService.
/// </summary>
public sealed class VatTuApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/master-data/vat-tu";

    [Fact(DisplayName = "Vật tư - Tạo vật tư tự sản xuất hợp lệ phải thành công")]
    public async Task TaoVatTu_TuSanXuatHopLe_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var ma = TaoMa("VT-TSX");
        using var response = await client.PostAsJsonAsync(Route, TaoRequestHopLe(ma));

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Created, "Tạo vật tư thành công");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(ma, data.GetProperty("maVatTu").GetString());
        Assert.Equal((byte)PhuongThucCungUngVatTu.ChiTuSanXuat,
            data.GetProperty("phuongThucCungUng").GetByte());
        Assert.Equal(JsonValueKind.Null, data.GetProperty("coSoMuaVatTuId").ValueKind);
    }

    [Fact(DisplayName = "Vật tư - Tạo vật tư mua ngoài cho phân xưởng cụ thể phải thành công")]
    public async Task TaoVatTu_MuaNgoaiVaPhanXuongCuThe_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var request = TaoRequestHopLe(
            TaoMa("VT-MUA"),
            phamViSuDung: 2,
            phanXuongIds: [DuLieuKiemThu.PhanXuong1Id, DuLieuKiemThu.PhanXuong2Id],
            phuongThucCungUng: 1,
            coSoMuaVatTuId: DuLieuKiemThu.CoSoMuaHoatDongId,
            nhaCungCapMacDinhId: DuLieuKiemThu.NhaCungCapId,
            ngayMuaHang: 7,
            moq: 10,
            thueVatId: DuLieuKiemThu.ThueHoatDongId);

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Created, "Tạo vật tư thành công");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(2, data.GetProperty("phanXuongs").GetArrayLength());
        Assert.Equal(DuLieuKiemThu.NhaCungCapId,
            data.GetProperty("nhaCungCapMacDinhId").GetGuid());
    }

    [Fact(DisplayName = "Vật tư - Thời gian mua hàng và tồn tối thiểu bằng 0 phải thành công")]
    public async Task TaoVatTu_ThoiGianMuaVaTonToiThieuBang0_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var request = TaoRequestHopLe(
            TaoMa("VT-MUA-0"),
            phuongThucCungUng: 1,
            coSoMuaVatTuId: DuLieuKiemThu.CoSoMuaHoatDongId,
            ngayMuaHang: 0,
            moq: null,
            thueVatId: DuLieuKiemThu.ThueHoatDongId,
            tonToiThieu: 0);

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Created, "Tạo vật tư thành công");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(0, data.GetProperty("ngayMuaHang").GetInt32());
        Assert.Equal(0m, data.GetProperty("tonToiThieu").GetDecimal());
    }

    [Fact(DisplayName = "Vật tư - Hạn sử dụng bằng 0 phải tạo thành công")]
    public async Task TaoVatTu_HanSuDungBang0_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var request = TaoRequestHopLe(
            TaoMa("VT-HSD-0"),
            hanSuDungNgay: 0);

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Created, "Tạo vật tư thành công");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(0, data.GetProperty("hanSuDungNgay").GetInt32());
    }

    [Fact(DisplayName = "Vật tư - Thiếu hạn sử dụng phải trả 400")]
    public async Task TaoVatTu_ThieuHanSuDung_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var request = TaoRequestHopLe(
            TaoMa("VT-THIEU-HSD"),
            hanSuDungNgay: null);

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Hạn sử dụng là bắt buộc");
    }

    [Theory(DisplayName = "Vật tư - Thiếu trường chuỗi bắt buộc phải trả 400")]
    [InlineData("ma")]
    [InlineData("ten")]
    public async Task TaoVatTu_ThieuChuoiBatBuoc_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "ma" => TaoRequestHopLe(string.Empty),
            "ten" => TaoRequestHopLe(TaoMa("VT"), tenVatTu: string.Empty),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Fact(DisplayName = "Vật tư - Không có phạm vi, tồn tối thiểu và kho lưu trữ vẫn phải tạo thành công")]
    public async Task TaoVatTu_KhongCoPhamViTonToiThieuVaKho_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var request = TaoRequestHopLe(TaoMa("VT-TOI-GIAN"));
        var json = JsonSerializer.SerializeToNode(
            request,
            new JsonSerializerOptions(JsonSerializerDefaults.Web))?.AsObject()
            ?? throw new InvalidOperationException("Không thể tạo payload kiểm thử.");

        json.Remove("phamViSuDung");
        json.Remove("phanXuongIds");
        json.Remove("tonToiThieu");
        json.Remove("khoLuuTruId");

        using var response = await client.PostAsJsonAsync(Route, json);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Created, "Tạo vật tư thành công");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(JsonValueKind.Null, data.GetProperty("phamViSuDung").ValueKind);
        Assert.Equal(JsonValueKind.Null, data.GetProperty("tonToiThieu").ValueKind);
        Assert.Equal(JsonValueKind.Null, data.GetProperty("khoLuuTruId").ValueKind);
        Assert.Equal(JsonValueKind.Null, data.GetProperty("maKhoLuuTru").ValueKind);
    }

    [Theory(DisplayName = "Vật tư - Chuỗi vượt độ dài phải trả 400")]
    [InlineData("ma")]
    [InlineData("ten")]
    [InlineData("tiengAnh")]
    [InlineData("quyCach")]
    [InlineData("mucDich")]
    [InlineData("nguoiTao")]
    public async Task TaoVatTu_ChuoiVuotDoDai_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "ma" => TaoRequestHopLe(new string('A', 101)),
            "ten" => TaoRequestHopLe(TaoMa("VT"), tenVatTu: new string('A', 301)),
            "tiengAnh" => TaoRequestHopLe(TaoMa("VT"), tenTiengAnh: new string('A', 301)),
            "quyCach" => TaoRequestHopLe(TaoMa("VT"), quyCachDongGoi: new string('A', 501)),
            "mucDich" => TaoRequestHopLe(TaoMa("VT"), mucDichSuDung: new string('A', 1001)),
            "nguoiTao" => TaoRequestHopLe(TaoMa("VT"), createdByMsnv: new string('A', 51)),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Theory(DisplayName = "Vật tư - Enum hoặc số không hợp lệ phải trả 400")]
    [InlineData("phamVi0")]
    [InlineData("phamVi3")]
    [InlineData("phuongThuc0")]
    [InlineData("phuongThuc4")]
    [InlineData("ngayMuaAm")]
    [InlineData("hanSuDungAm")]
    [InlineData("tonAm")]
    [InlineData("moq0")]
    public async Task TaoVatTu_GiaTriBienKhongHopLe_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "phamVi0" => TaoRequestHopLe(TaoMa("VT"), phamViSuDung: 0),
            "phamVi3" => TaoRequestHopLe(TaoMa("VT"), phamViSuDung: 3),
            "phuongThuc0" => TaoRequestHopLe(TaoMa("VT"), phuongThucCungUng: 0),
            "phuongThuc4" => TaoRequestHopLe(TaoMa("VT"), phuongThucCungUng: 4),
            "ngayMuaAm" => TaoRequestHopLe(TaoMa("VT"), ngayMuaHang: -1),
            "hanSuDungAm" => TaoRequestHopLe(TaoMa("VT"), hanSuDungNgay: -1),
            "tonAm" => TaoRequestHopLe(TaoMa("VT"), tonToiThieu: -1),
            "moq0" => TaoRequestHopLe(TaoMa("VT"), moq: 0),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Theory(DisplayName = "Vật tư - Thiếu khóa ngoại bắt buộc phải trả 400")]
    [InlineData("dvt")]
    [InlineData("nhom")]
    public async Task TaoVatTu_ThieuDanhMucBatBuoc_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "dvt" => TaoRequestHopLe(TaoMa("VT"), donViTinhId: Guid.Empty),
            "nhom" => TaoRequestHopLe(TaoMa("VT"), nhomVatTuId: Guid.Empty),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "là bắt buộc");
    }

    [Theory(DisplayName = "Vật tư - Khóa ngoại không tồn tại phải trả 404")]
    [InlineData("dvt")]
    [InlineData("nhom")]
    [InlineData("kho")]
    public async Task TaoVatTu_DanhMucKhongTonTai_PhaiTra404(string truong)
    {
        using var client = factory.CreateClient();
        var idKhongTonTai = Guid.NewGuid();
        var request = truong switch
        {
            "dvt" => TaoRequestHopLe(TaoMa("VT"), donViTinhId: idKhongTonTai),
            "nhom" => TaoRequestHopLe(TaoMa("VT"), nhomVatTuId: idKhongTonTai),
            "kho" => TaoRequestHopLe(TaoMa("VT"), khoLuuTruId: idKhongTonTai),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy");
    }

    [Theory(DisplayName = "Vật tư - Danh mục ngừng hoạt động phải bị từ chối")]
    [InlineData("dvt")]
    [InlineData("nhom")]
    [InlineData("kho")]
    public async Task TaoVatTu_DanhMucNgungHoatDong_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = truong switch
        {
            "dvt" => TaoRequestHopLe(TaoMa("VT"), donViTinhId: DuLieuKiemThu.DonViTinhNgungId),
            "nhom" => TaoRequestHopLe(TaoMa("VT"), nhomVatTuId: DuLieuKiemThu.NhomVatTuNgungId),
            "kho" => TaoRequestHopLe(TaoMa("VT"), khoLuuTruId: DuLieuKiemThu.KhoNgungId),
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "đã ngừng hoạt động");
    }

    [Theory(DisplayName = "Vật tư - Mua ngoài thiếu thông tin bắt buộc phải trả 400")]
    [InlineData("coSo")]
    [InlineData("ngayMua")]
    [InlineData("thue")]
    public async Task TaoVatTu_MuaNgoaiThieuThongTin_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        var request = TaoRequestMuaNgoai(truong);

        using var response = await client.PostAsJsonAsync(Route, request);
        const string thongDiepMongDoi = "bắt buộc";
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, thongDiepMongDoi);
    }

    [Theory(DisplayName = "Vật tư - Danh mục mua ngoài không tồn tại phải trả 404")]
    [InlineData("coSo")]
    [InlineData("thue")]
    [InlineData("nhaCungCap")]
    public async Task TaoVatTu_DanhMucMuaNgoaiKhongTonTai_PhaiTra404(string truong)
    {
        using var client = factory.CreateClient();
        var id = Guid.NewGuid();
        var request = TaoRequestHopLe(
            TaoMa("VT-MUA"),
            phuongThucCungUng: 1,
            coSoMuaVatTuId: truong == "coSo" ? id : DuLieuKiemThu.CoSoMuaHoatDongId,
            nhaCungCapMacDinhId: truong == "nhaCungCap" ? id : DuLieuKiemThu.NhaCungCapId,
            ngayMuaHang: 7,
            moq: 1,
            thueVatId: truong == "thue" ? id : DuLieuKiemThu.ThueHoatDongId);

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy");
    }

    [Fact(DisplayName = "Vật tư - Đối tác không phải nhà cung cấp phải bị từ chối")]
    public async Task TaoVatTu_DoiTacKhongPhaiNhaCungCap_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var request = TaoRequestHopLe(
            TaoMa("VT-MUA"),
            phuongThucCungUng: 1,
            coSoMuaVatTuId: DuLieuKiemThu.CoSoMuaHoatDongId,
            nhaCungCapMacDinhId: DuLieuKiemThu.DoiTacKhongPhaiNhaCungCapId,
            ngayMuaHang: 7,
            moq: 1,
            thueVatId: DuLieuKiemThu.ThueHoatDongId);

        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "không phải là nhà cung cấp");
    }

    [Theory(DisplayName = "Vật tư - Phạm vi phân xưởng cụ thể không hợp lệ phải bị từ chối")]
    [InlineData("rong")]
    [InlineData("trung")]
    [InlineData("ngung")]
    [InlineData("khongTonTai")]
    public async Task TaoVatTu_PhanXuongCuTheKhongHopLe_PhaiTra400(string truong)
    {
        using var client = factory.CreateClient();
        IReadOnlyCollection<Guid> ids = truong switch
        {
            "rong" => Array.Empty<Guid>(),
            "trung" => [DuLieuKiemThu.PhanXuong1Id, DuLieuKiemThu.PhanXuong1Id],
            "ngung" => [DuLieuKiemThu.PhanXuongNgungId],
            "khongTonTai" => [Guid.NewGuid()],
            _ => throw new InvalidOperationException("Trường kiểm thử không hợp lệ.")
        };

        var request = TaoRequestHopLe(TaoMa("VT"), phamViSuDung: 2, phanXuongIds: ids);
        using var response = await client.PostAsJsonAsync(Route, request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "phân xưởng");
    }

    [Fact(DisplayName = "Vật tư - Mã trùng phải trả 409")]
    public async Task TaoVatTu_MaTrung_PhaiTra409()
    {
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync(
            Route, TaoRequestHopLe(DuLieuKiemThu.MaVatTuCoSan.ToLowerInvariant()));

        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "đã tồn tại");
    }

    [Fact(DisplayName = "Vật tư - Lấy ID không tồn tại phải trả 404")]
    public async Task LayVatTu_IdKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync($"{Route}/{Guid.NewGuid()}");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy vật tư");
    }

    [Theory(DisplayName = "Vật tư - Phân trang hoặc bộ lọc sai phải trả 400")]
    [InlineData("?page=0")]
    [InlineData("?pageSize=0")]
    [InlineData("?pageSize=201")]
    [InlineData("?phamViSuDung=3")]
    [InlineData("?phuongThucCungUng=4")]
    [InlineData("?trangThai=2")]
    public async Task LayDanhSach_ThamSoKhongHopLe_PhaiTra400(string query)
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync(Route + query);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Dữ liệu không hợp lệ");
    }

    [Fact(DisplayName = "Vật tư - Cập nhật ID không tồn tại phải trả 404")]
    public async Task CapNhatVatTu_IdKhongTonTai_PhaiTra404()
    {
        using var client = factory.CreateClient();
        var request = TaoCapNhatHopLe(TaoMa("VT"), DuLieuKiemThu.RowVersionHopLe);
        using var response = await client.PutAsJsonAsync($"{Route}/{Guid.NewGuid()}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.NotFound, "Không tìm thấy vật tư");
    }

    [Fact(DisplayName = "Vật tư - RowVersion sai định dạng phải trả 400")]
    public async Task CapNhatVatTu_RowVersionSaiDinhDang_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var request = TaoCapNhatHopLe(TaoMa("VT"), "khong-phai-base64");
        using var response = await client.PutAsJsonAsync(
            $"{Route}/{DuLieuKiemThu.VatTuCoSanId}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "RowVersion không đúng định dạng Base64");
    }

    [Fact(DisplayName = "Vật tư - RowVersion cũ phải trả 409")]
    public async Task CapNhatVatTu_RowVersionCu_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var request = TaoCapNhatHopLe(TaoMa("VT"), "CQkJCQ==");
        using var response = await client.PutAsJsonAsync(
            $"{Route}/{DuLieuKiemThu.VatTuCoSanId}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "Dữ liệu đã được người khác cập nhật");
    }

    [Fact(DisplayName = "Vật tư - Cập nhật thiếu mã phải trả thông báo lỗi chi tiết")]
    public async Task CapNhatVatTu_ThieuMa_PhaiTra400VaThongBaoChiTiet()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(string.Empty, rowVersion);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.BadRequest, "Mã vật tư là bắt buộc");

        var content = await ApiKiemThuHelper.DocNoiDungAsync(response);
        using var document = JsonDocument.Parse(content);
        Assert.False(document.RootElement.GetProperty("success").GetBoolean());
        Assert.Contains(
            "Mã vật tư là bắt buộc",
            document.RootElement.GetProperty("message").GetString() ?? string.Empty,
            StringComparison.OrdinalIgnoreCase);
        Assert.True(document.RootElement.TryGetProperty("errors", out var errors));
        Assert.True(errors.TryGetProperty("maVatTu", out _));
    }

    [Fact(DisplayName = "Vật tư - Cập nhật tất cả phân xưởng nhưng vẫn gửi phân xưởng cụ thể phải trả 400")]
    public async Task CapNhatVatTu_TatCaPhanXuongNhungCoDanhSachCuThe_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(
            TaoMa("VT-NEW"),
            rowVersion,
            phamViSuDung: 1,
            phanXuongIds: [DuLieuKiemThu.PhanXuong1Id]);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "không được chọn phân xưởng cụ thể");
    }

    [Fact(DisplayName = "Vật tư - Cập nhật phân xưởng cụ thể nhưng không chọn phân xưởng phải trả 400")]
    public async Task CapNhatVatTu_PhanXuongCuTheNhungDanhSachRong_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(
            TaoMa("VT-NEW"),
            rowVersion,
            phamViSuDung: 2,
            phanXuongIds: Array.Empty<Guid>());

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "Vui lòng chọn ít nhất một phân xưởng");
    }

    [Fact(DisplayName = "Vật tư - Cập nhật mua ngoài thiếu thông tin bắt buộc phải trả 400")]
    public async Task CapNhatVatTu_MuaNgoaiThieuThongTin_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(
            TaoMa("VT-MUA"),
            rowVersion,
            phuongThucCungUng: 1);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "Cơ sở mua vật tư là bắt buộc");
    }

    [Fact(DisplayName = "Vật tư - Cập nhật dùng danh mục ngừng hoạt động phải trả 400")]
    public async Task CapNhatVatTu_DonViTinhNgungHoatDong_PhaiTra400()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(
            TaoMa("VT-NEW"),
            rowVersion,
            donViTinhId: DuLieuKiemThu.DonViTinhNgungId);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response,
            HttpStatusCode.BadRequest,
            "Đơn vị tính đã ngừng hoạt động");
    }

    [Fact(DisplayName = "Vật tư - Cập nhật có thể xóa phạm vi, tồn tối thiểu và kho lưu trữ")]
    public async Task CapNhatVatTu_XoaCacThongTinTuyChon_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD-NULL"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(TaoMa("VT-UPD-NULL-MOI"), rowVersion);
        var json = JsonSerializer.SerializeToNode(
            request,
            new JsonSerializerOptions(JsonSerializerDefaults.Web))?.AsObject()
            ?? throw new InvalidOperationException("Không thể tạo payload kiểm thử.");

        json.Remove("phamViSuDung");
        json.Remove("phanXuongIds");
        json.Remove("tonToiThieu");
        json.Remove("khoLuuTruId");

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", json);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.OK, "Cập nhật vật tư thành công");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(JsonValueKind.Null, data.GetProperty("phamViSuDung").ValueKind);
        Assert.Equal(JsonValueKind.Null, data.GetProperty("tonToiThieu").ValueKind);
        Assert.Equal(JsonValueKind.Null, data.GetProperty("khoLuuTruId").ValueKind);
        Assert.Equal(0, data.GetProperty("phanXuongs").GetArrayLength());
    }

    [Fact(DisplayName = "Vật tư - Cập nhật hợp lệ phải lưu dữ liệu mới")]
    public async Task CapNhatVatTu_HopLe_PhaiThanhCong()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var maMoi = TaoMa("VT-NEW");
        var request = TaoCapNhatHopLe(
            maMoi,
            rowVersion,
            tenVatTu: "Tên vật tư sau cập nhật",
            trangThai: 0);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.OK, "Cập nhật vật tư thành công");
        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(maMoi, data.GetProperty("maVatTu").GetString());
        Assert.Equal(0, data.GetProperty("trangThai").GetByte());
    }

    [Fact(DisplayName = "Vật tư - Cập nhật sang mã đã tồn tại phải trả 409")]
    public async Task CapNhatVatTu_MaTrung_PhaiTra409()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-UPD"));
        var rowVersion = await LayRowVersionAsync(client, id);
        var request = TaoCapNhatHopLe(DuLieuKiemThu.MaVatTuCoSan, rowVersion);

        using var response = await client.PutAsJsonAsync($"{Route}/{id}", request);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.Conflict, "đã tồn tại");
    }

    [Fact(DisplayName = "Vật tư - Xóa hợp lệ thì GET lại phải trả 404")]
    public async Task XoaVatTu_HopLe_PhaiKhongConDuLieu()
    {
        using var client = factory.CreateClient();
        var id = await ThemVatTuTrucTiepAsync(TaoMa("VT-DEL"));
        var rowVersion = await LayRowVersionAsync(client, id);

        using var deleteResponse = await client.DeleteAsync(
            $"{Route}/{id}?rowVersion={Uri.EscapeDataString(rowVersion)}");
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            deleteResponse, HttpStatusCode.OK, "Xóa vật tư thành công");

        using var getResponse = await client.GetAsync($"{Route}/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static TaoVatTuRequest TaoRequestHopLe(
        string maVatTu,
        string tenVatTu = "Vật tư kiểm thử",
        string? tenTiengAnh = null,
        Guid? donViTinhId = null,
        string? quyCachDongGoi = null,
        byte phamViSuDung = 1,
        IReadOnlyCollection<Guid>? phanXuongIds = null,
        Guid? nhomVatTuId = null,
        string? mucDichSuDung = null,
        byte phuongThucCungUng = 3,
        Guid? coSoMuaVatTuId = null,
        Guid? nhaCungCapMacDinhId = null,
        int? ngayMuaHang = null,
        int? hanSuDungNgay = 30,
        decimal? moq = null,
        Guid? thueVatId = null,
        decimal tonToiThieu = 0,
        Guid? khoLuuTruId = null,
        string? createdByMsnv = "TEST")
        => new()
        {
            MaVatTu = maVatTu,
            TenVatTu = tenVatTu,
            TenTiengAnh = tenTiengAnh,
            DonViTinhId = donViTinhId ?? DuLieuKiemThu.DonViTinhHoatDongId,
            QuyCachDongGoi = quyCachDongGoi,
            PhamViSuDung = phamViSuDung,
            PhanXuongIds = phanXuongIds ?? Array.Empty<Guid>(),
            NhomVatTuId = nhomVatTuId ?? DuLieuKiemThu.NhomVatTuHoatDongId,
            MucDichSuDung = mucDichSuDung,
            PhuongThucCungUng = phuongThucCungUng,
            CoSoMuaVatTuId = coSoMuaVatTuId,
            NhaCungCapMacDinhId = nhaCungCapMacDinhId,
            NgayMuaHang = ngayMuaHang,
            HanSuDungNgay = hanSuDungNgay,
            Moq = moq,
            ThueVatId = thueVatId,
            TonToiThieu = tonToiThieu,
            KhoLuuTruId = khoLuuTruId ?? DuLieuKiemThu.KhoLuuTruId,
            CreatedByMsnv = createdByMsnv
        };

    private static TaoVatTuRequest TaoRequestMuaNgoai(string truongThieu)
        => TaoRequestHopLe(
            TaoMa("VT-MUA"),
            phuongThucCungUng: 1,
            coSoMuaVatTuId: truongThieu == "coSo" ? null : DuLieuKiemThu.CoSoMuaHoatDongId,
            ngayMuaHang: truongThieu == "ngayMua" ? null : 7,
            moq: null,
            thueVatId: truongThieu == "thue" ? null : DuLieuKiemThu.ThueHoatDongId);

    private static CapNhatVatTuRequest TaoCapNhatHopLe(
        string maVatTu,
        string rowVersion,
        string tenVatTu = "Vật tư cập nhật",
        Guid? donViTinhId = null,
        byte phamViSuDung = 1,
        IReadOnlyCollection<Guid>? phanXuongIds = null,
        Guid? nhomVatTuId = null,
        byte phuongThucCungUng = 3,
        Guid? coSoMuaVatTuId = null,
        Guid? nhaCungCapMacDinhId = null,
        int? ngayMuaHang = null,
        decimal? moq = null,
        Guid? thueVatId = null,
        Guid? khoLuuTruId = null,
        byte trangThai = 1)
        => new()
        {
            MaVatTu = maVatTu,
            TenVatTu = tenVatTu,
            DonViTinhId = donViTinhId ?? DuLieuKiemThu.DonViTinhHoatDongId,
            PhamViSuDung = phamViSuDung,
            PhanXuongIds = phanXuongIds ?? Array.Empty<Guid>(),
            NhomVatTuId = nhomVatTuId ?? DuLieuKiemThu.NhomVatTuHoatDongId,
            PhuongThucCungUng = phuongThucCungUng,
            CoSoMuaVatTuId = coSoMuaVatTuId,
            NhaCungCapMacDinhId = nhaCungCapMacDinhId,
            NgayMuaHang = ngayMuaHang,
            HanSuDungNgay = 30,
            Moq = moq,
            ThueVatId = thueVatId,
            TonToiThieu = 0,
            KhoLuuTruId = khoLuuTruId ?? DuLieuKiemThu.KhoLuuTruId,
            TrangThai = trangThai,
            UpdatedByMsnv = "TEST",
            RowVersion = rowVersion
        };

    private async Task<Guid> ThemVatTuTrucTiepAsync(string ma)
    {
        var id = Guid.NewGuid();
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EmanDbContext>();
        dbContext.VatTus.Add(new VatTuEntity
        {
            Id = id,
            MaVatTu = ma,
            TenVatTu = "Vật tư trực tiếp",
            DonViTinhId = DuLieuKiemThu.DonViTinhHoatDongId,
            PhamViSuDung = PhamViSuDungVatTu.TatCaPhanXuong,
            NhomVatTuId = DuLieuKiemThu.NhomVatTuHoatDongId,
            PhuongThucCungUng = PhuongThucCungUngVatTu.ChiTuSanXuat,
            HanSuDungNgay = 30,
            TonToiThieu = 0,
            KhoLuuTruId = DuLieuKiemThu.KhoLuuTruId,
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
