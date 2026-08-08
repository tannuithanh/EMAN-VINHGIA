using System.Net;
using ClosedXML.Excel;
using Eman.Api.Tests.Infrastructure;

namespace Eman.Api.Tests.Modules.MasterData.Materials.VatTu.Api;

/// <summary>
/// Kiểm tra chức năng xuất danh mục vật tư ra Excel theo bộ lọc hiện tại.
/// </summary>
public sealed class VatTuExportApiTests(EmanApiFactory factory) : IClassFixture<EmanApiFactory>
{
    private const string Route = "/api/master-data/vat-tu";

    [Fact(DisplayName = "Xuất vật tư - File Excel phải đúng form import và đúng dữ liệu theo bộ lọc")]
    public async Task XuatExcel_TheoTuKhoa_PhaiDungFormVaDungDuLieu()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync(
            $"{Route}/export?keyword={Uri.EscapeDataString(DuLieuKiemThu.MaVatTuCoSan)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            response.Content.Headers.ContentType?.MediaType);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.NotEmpty(bytes);

        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);

        var vatTuSheet = workbook.Worksheet("Import vật tư");
        Assert.Equal("MÃ VẬT TƯ", vatTuSheet.Cell(1, 1).GetString());
        Assert.Equal("TÊN VẬT TƯ", vatTuSheet.Cell(1, 2).GetString());
        Assert.Equal("HẠN SỬ DỤNG (NGÀY)", vatTuSheet.Cell(1, 13).GetString());
        Assert.Equal("KHO LƯU TRỮ", vatTuSheet.Cell(1, 17).GetString());
        Assert.Equal("Cambria", vatTuSheet.Cell(1, 1).Style.Font.FontName);
        Assert.Equal(12d, vatTuSheet.Cell(1, 1).Style.Font.FontSize);

        Assert.Equal(DuLieuKiemThu.MaVatTuCoSan, vatTuSheet.Cell(2, 1).GetString());
        Assert.Equal("Vật tư có sẵn", vatTuSheet.Cell(2, 2).GetString());
        Assert.Equal("1 - Tất cả phân xưởng", vatTuSheet.Cell(2, 6).GetString());
        Assert.Equal("3 - Chỉ tự sản xuất", vatTuSheet.Cell(2, 9).GetString());
        Assert.Equal(30, vatTuSheet.Cell(2, 13).GetValue<int>());

        Assert.True(vatTuSheet.Cell(3, 1).IsEmpty());
        Assert.NotNull(workbook.Worksheet("Phân xưởng sử dụng"));
    }

    [Fact(DisplayName = "Xuất vật tư - Không tìm thấy dữ liệu vẫn phải trả file có header")]
    public async Task XuatExcel_KhongCoDuLieu_PhaiTraFileCoHeader()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync($"{Route}/export?keyword=KHONG-CO-MA-NAY");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet("Import vật tư");

        Assert.Equal("MÃ VẬT TƯ", worksheet.Cell(1, 1).GetString());
        Assert.True(worksheet.Cell(2, 1).IsEmpty());
    }
}
