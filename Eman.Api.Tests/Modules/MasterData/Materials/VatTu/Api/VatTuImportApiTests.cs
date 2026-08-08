using System.Net;
using System.Net.Http.Headers;
using ClosedXML.Excel;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.MasterData.Materials.VatTu.Api;

/// <summary>
/// Kiểm tra trực tiếp luồng xem trước file import vật tư.
/// </summary>
public sealed class VatTuImportApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/master-data/vat-tu";

    [Fact(DisplayName = "Import vật tư - Phạm vi, tồn tối thiểu, kho và MOQ để trống vẫn hợp lệ khi hạn sử dụng bằng 0")]
    public async Task XemTruoc_CacTruongTuyChonDeTrongVaHanSuDungBang0_PhaiHopLe()
    {
        using var client = factory.CreateClient();

        using var templateResponse = await client.GetAsync($"{Route}/import/template");
        Assert.Equal(HttpStatusCode.OK, templateResponse.StatusCode);

        var templateBytes = await templateResponse.Content.ReadAsByteArrayAsync();
        using var templateStream = new MemoryStream(templateBytes);
        using var workbook = new XLWorkbook(templateStream);
        var worksheet = workbook.Worksheet("Import vật tư");

        worksheet.Cell(2, 1).Value = TaoMa();
        worksheet.Cell(2, 2).Value = "Vật tư import kiểm thử";
        worksheet.Cell(2, 4).Value = "DVT-TEST";
        worksheet.Cell(2, 7).Value = "NVT-TEST";
        worksheet.Cell(2, 9).Value = "1 - Chỉ mua ngoài";
        worksheet.Cell(2, 10).Value = "CSM-TEST";
        worksheet.Cell(2, 12).Value = 0;
        worksheet.Cell(2, 13).Value = 0;
        worksheet.Cell(2, 15).Value = "VAT10";

        using var outputStream = new MemoryStream();
        workbook.SaveAs(outputStream);

        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(outputStream.ToArray());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        form.Add(fileContent, "File", "vat-tu-kiem-thu.xlsx");

        using var response = await client.PostAsync($"{Route}/import/preview", form);
        await ApiKiemThuHelper.KiemTraTrangThaiVaThongDiepAsync(
            response, HttpStatusCode.OK, "File hợp lệ");

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(1, data.GetProperty("tongSoDong").GetInt32());
        Assert.Equal(1, data.GetProperty("soDongHopLe").GetInt32());
        Assert.Equal(0, data.GetProperty("soDongLoi").GetInt32());
    }

    [Fact(DisplayName = "Import vật tư - Thời gian mua nhập ngày tháng phải báo lỗi")]
    public async Task XemTruoc_ThoiGianMuaNhapNgayThang_PhaiBaoLoi()
    {
        using var client = factory.CreateClient();

        using var templateResponse = await client.GetAsync($"{Route}/import/template");
        Assert.Equal(HttpStatusCode.OK, templateResponse.StatusCode);

        var templateBytes = await templateResponse.Content.ReadAsByteArrayAsync();
        using var templateStream = new MemoryStream(templateBytes);
        using var workbook = new XLWorkbook(templateStream);
        var worksheet = workbook.Worksheet("Import vật tư");

        worksheet.Cell(2, 1).Value = TaoMa();
        worksheet.Cell(2, 2).Value = "Vật tư import sai thời gian";
        worksheet.Cell(2, 4).Value = "DVT-TEST";
        worksheet.Cell(2, 6).Value = "1";
        worksheet.Cell(2, 7).Value = "NVT-TEST";
        worksheet.Cell(2, 9).Value = "1";
        worksheet.Cell(2, 10).Value = "CSM-TEST";
        worksheet.Cell(2, 12).Value = "04/08/2026";
        worksheet.Cell(2, 13).Value = 0;
        worksheet.Cell(2, 15).Value = "VAT10";
        worksheet.Cell(2, 16).Value = 0;
        worksheet.Cell(2, 17).Value = "KHO-LUU";

        using var outputStream = new MemoryStream();
        workbook.SaveAs(outputStream);

        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(outputStream.ToArray());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        form.Add(fileContent, "File", "vat-tu-sai-thoi-gian.xlsx");

        using var response = await client.PostAsync($"{Route}/import/preview", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(1, data.GetProperty("soDongLoi").GetInt32());

        var dongLoi = data.GetProperty("danhSach").EnumerateArray().First();
        var loi = dongLoi.GetProperty("loi").EnumerateArray().First().GetString();

        Assert.Contains("Thời gian mua hàng phải là số nguyên lớn hơn hoặc bằng 0 ngày", loi);
    }

    [Fact(DisplayName = "Import vật tư - Thiếu hạn sử dụng phải báo lỗi")]
    public async Task XemTruoc_ThieuHanSuDung_PhaiBaoLoi()
    {
        using var client = factory.CreateClient();

        using var templateResponse = await client.GetAsync($"{Route}/import/template");
        Assert.Equal(HttpStatusCode.OK, templateResponse.StatusCode);

        var templateBytes = await templateResponse.Content.ReadAsByteArrayAsync();
        using var templateStream = new MemoryStream(templateBytes);
        using var workbook = new XLWorkbook(templateStream);
        var worksheet = workbook.Worksheet("Import vật tư");

        worksheet.Cell(2, 1).Value = TaoMa();
        worksheet.Cell(2, 2).Value = "Vật tư thiếu hạn sử dụng";
        worksheet.Cell(2, 4).Value = "DVT-TEST";
        worksheet.Cell(2, 7).Value = "NVT-TEST";
        worksheet.Cell(2, 9).Value = "3 - Chỉ tự sản xuất";

        using var outputStream = new MemoryStream();
        workbook.SaveAs(outputStream);

        using var form = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(outputStream.ToArray());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        form.Add(fileContent, "File", "vat-tu-thieu-han-su-dung.xlsx");

        using var response = await client.PostAsync($"{Route}/import/preview", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(1, data.GetProperty("soDongLoi").GetInt32());

        var dongLoi = data.GetProperty("danhSach").EnumerateArray().First();
        var loi = string.Join(" | ", dongLoi.GetProperty("loi").EnumerateArray().Select(item => item.GetString()));
        Assert.Contains("Hạn sử dụng là bắt buộc", loi);
    }

    private static string TaoMa()
        => $"VT-IMPORT-{Guid.NewGuid():N}"[..42].ToUpperInvariant();
}
