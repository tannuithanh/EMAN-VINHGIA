using System.Net;
using System.Net.Http.Headers;
using ClosedXML.Excel;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.Engineering.Bom.VatTu.Api;

/// <summary>
/// Kiểm tra template, preview và import B.O.M vật tư từ Excel.
/// </summary>
public sealed class BomVatTuImportApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/engineering/bom/vat-tu";

    [Fact(DisplayName = "B.O.M vật tư - Template dùng Cambria và đủ cột import")]
    public async Task Template_PhaiDungStyleVaHeaderHeThong()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync($"{Route}/import/template");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet("Import B.O.M vật tư");

        Assert.Equal("MÃ VẬT TƯ ĐẦU RA", worksheet.Cell(1, 1).GetString());
        Assert.Equal("MÃ VẬT TƯ THÀNH PHẦN", worksheet.Cell(1, 2).GetString());
        Assert.Equal("SỐ LƯỢNG", worksheet.Cell(1, 3).GetString());
        Assert.Equal("GHI CHÚ", worksheet.Cell(1, 4).GetString());
        Assert.Equal("Cambria", worksheet.Cell(1, 1).Style.Font.FontName);
    }

    [Fact(DisplayName = "B.O.M vật tư - Preview hợp lệ gom nhiều dòng thành một B.O.M")]
    public async Task Preview_NhieuThanhPhanCungDauRa_PhaiThanhMotBomHopLe()
    {
        using var client = factory.CreateClient();
        var file = await TaoFileAsync(client,
            (DuLieuKiemThu.MaBomVatTuDauRa, DuLieuKiemThu.MaBomVatTuThanhPhan1, 0.6m),
            (DuLieuKiemThu.MaBomVatTuDauRa, DuLieuKiemThu.MaBomVatTuThanhPhan2, 0.4m));

        using var form = TaoForm(file, "bom-vat-tu-preview.xlsx");
        using var response = await client.PostAsync($"{Route}/import/preview", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(2, data.GetProperty("tongSoDong").GetInt32());
        Assert.Equal(1, data.GetProperty("tongSoBom").GetInt32());
        Assert.Equal(1, data.GetProperty("soBomCoTheImport").GetInt32());
        Assert.Equal(0, data.GetProperty("soBomLoi").GetInt32());

        var bom = data.GetProperty("danhSachBom").EnumerateArray().Single();
        Assert.Equal(DuLieuKiemThu.MaBomVatTuDauRa, bom.GetProperty("maVatTuDauRa").GetString());
        Assert.Equal(2, bom.GetProperty("tongSoThanhPhan").GetInt32());
        Assert.True(bom.GetProperty("coTheImport").GetBoolean());
    }

    [Fact(DisplayName = "B.O.M vật tư - Import tạo phiên bản Nháp và chi tiết thành phần")]
    public async Task Import_HopLe_PhaiTaoPhienBanNhap()
    {
        using var client = factory.CreateClient();
        var file = await TaoFileAsync(client,
            (DuLieuKiemThu.MaBomVatTuDauRa, DuLieuKiemThu.MaBomVatTuThanhPhan1, 0.6m),
            (DuLieuKiemThu.MaBomVatTuDauRa, DuLieuKiemThu.MaBomVatTuThanhPhan2, 0.4m));

        using var form = TaoForm(file, "bom-vat-tu-import.xlsx");
        form.Add(new StringContent("NV-TEST"), "CreatedByMsnv");
        using var response = await client.PostAsync($"{Route}/import", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(1, data.GetProperty("soBomDaImport").GetInt32());
        Assert.Equal(2, data.GetProperty("soDongDaImport").GetInt32());

        using var listResponse = await client.GetAsync(
            $"{Route}/phien-ban?vatTuId={DuLieuKiemThu.BomVatTuDauRaId}&page=1&pageSize=20");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var listData = await ApiKiemThuHelper.LayDataAsync(listResponse);
        var items = listData.GetProperty("items").EnumerateArray().ToList();
        Assert.Single(items);
        Assert.Equal(1, items[0].GetProperty("soPhienBan").GetInt32());
        Assert.Equal(0, items[0].GetProperty("trangThai").GetInt32());
        Assert.Equal(2, items[0].GetProperty("soThanhPhan").GetInt32());
    }

    [Fact(DisplayName = "B.O.M vật tư - Một thành phần lỗi làm cả B.O.M không thể import")]
    public async Task Preview_MotDongLoi_PhaiKhoaToanBoBom()
    {
        using var client = factory.CreateClient();
        var file = await TaoFileAsync(client,
            (DuLieuKiemThu.MaBomVatTuDauRa, DuLieuKiemThu.MaBomVatTuThanhPhan1, 0.6m),
            (DuLieuKiemThu.MaBomVatTuDauRa, "VT-KHONG-TON-TAI", 0.4m));

        using var form = TaoForm(file, "bom-vat-tu-loi.xlsx");
        using var response = await client.PostAsync($"{Route}/import/preview", form);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await ApiKiemThuHelper.LayDataAsync(response);
        Assert.Equal(1, data.GetProperty("tongSoBom").GetInt32());
        Assert.Equal(0, data.GetProperty("soBomCoTheImport").GetInt32());
        Assert.Equal(1, data.GetProperty("soBomLoi").GetInt32());

        var bom = data.GetProperty("danhSachBom").EnumerateArray().Single();
        Assert.False(bom.GetProperty("coTheImport").GetBoolean());
    }

    private static async Task<byte[]> TaoFileAsync(
        HttpClient client,
        params (string DauRa, string ThanhPhan, decimal SoLuong)[] rows)
    {
        using var templateResponse = await client.GetAsync($"{Route}/import/template");
        Assert.Equal(HttpStatusCode.OK, templateResponse.StatusCode);
        var templateBytes = await templateResponse.Content.ReadAsByteArrayAsync();

        using var input = new MemoryStream(templateBytes);
        using var workbook = new XLWorkbook(input);
        var worksheet = workbook.Worksheet("Import B.O.M vật tư");
        for (var index = 0; index < rows.Length; index++)
        {
            var rowNumber = index + 2;
            worksheet.Cell(rowNumber, 1).Value = rows[index].DauRa;
            worksheet.Cell(rowNumber, 2).Value = rows[index].ThanhPhan;
            worksheet.Cell(rowNumber, 3).Value = rows[index].SoLuong;
        }

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }

    private static MultipartFormDataContent TaoForm(byte[] file, string fileName)
    {
        var form = new MultipartFormDataContent();
        var content = new ByteArrayContent(file);
        content.Headers.ContentType = new MediaTypeHeaderValue(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        form.Add(content, "File", fileName);
        return form;
    }
}
