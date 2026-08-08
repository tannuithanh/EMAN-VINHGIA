using ClosedXML.Excel;
using Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using Eman.Infrastructure.Persistence;
using Eman.Infrastructure.Services.Imports.Common.Excel;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Services.Engineering.Bom.VatTu.Imports;

/// <summary>
/// Tạo file mẫu import B.O.M vật tư theo dạng mỗi dòng là một vật tư thành phần trực tiếp.
/// </summary>
internal sealed class BomVatTuImportTemplateBuilder(EmanDbContext dbContext)
{
    private const int HeaderRowIndex = 1;
    private const int DataStartRowIndex = 2;
    private const int DataEndRowIndex = 12001;

    internal static readonly string[] Headers =
    {
        "MÃ VẬT TƯ ĐẦU RA",
        "MÃ VẬT TƯ THÀNH PHẦN",
        "SỐ LƯỢNG",
        "GHI CHÚ"
    };

    public async Task<BomVatTuImportFileDto> BuildAsync(CancellationToken cancellationToken)
    {
        var vatTus = await dbContext.VatTus
            .AsNoTracking()
            .Include(item => item.DonViTinh)
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaVatTu)
            .Select(item => new DanhMucVatTu(
                item.MaVatTu,
                item.TenVatTu,
                item.DonViTinh.MaDonViTinh,
                item.PhuongThucCungUng))
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var importSheet = workbook.Worksheets.Add("Import B.O.M vật tư");
        var huongDanSheet = workbook.Worksheets.Add("Hướng dẫn");
        var danhMucSheet = workbook.Worksheets.Add("Danh mục vật tư");

        TaoSheetImport(importSheet);
        TaoSheetHuongDan(huongDanSheet);
        TaoSheetDanhMuc(danhMucSheet, vatTus);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new BomVatTuImportFileDto { Content = stream.ToArray() };
    }

    private static void TaoSheetImport(IXLWorksheet worksheet)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);

        for (var index = 0; index < Headers.Length; index++)
        {
            var cell = worksheet.Cell(HeaderRowIndex, index + 1);
            cell.Value = Headers[index];
            var mauNen = index switch
            {
                0 => ImportExcelStyle.ProductFill,
                1 or 2 => ImportExcelStyle.CatalogFill,
                _ => ImportExcelStyle.DestinationFill
            };
            ImportExcelStyle.ApDungHeader(cell, mauNen, chuDo: index < 3);
        }

        var dataRange = worksheet.Range(DataStartRowIndex, 1, DataEndRowIndex, Headers.Length);
        ImportExcelStyle.ApDungVungDuLieu(dataRange);
        worksheet.Range(DataStartRowIndex, 1, DataEndRowIndex, 2).Style.NumberFormat.Format = "@";
        worksheet.Column(3).Style.NumberFormat.Format = "0.######";
        worksheet.Column(4).Style.Alignment.WrapText = true;

        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, 1, Headers.Length).SetAutoFilter();
        worksheet.Row(1).Height = 58;
        worksheet.Column(1).Width = 26;
        worksheet.Column(2).Width = 28;
        worksheet.Column(3).Width = 18;
        worksheet.Column(4).Width = 42;

        foreach (var columnIndex in new[] { 1, 2, 3 })
        {
            worksheet.Column(columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
    }

    private static void TaoSheetHuongDan(IXLWorksheet worksheet)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);
        worksheet.Cell(1, 1).Value = "Quy tắc import B.O.M vật tư EMAN";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontName = ImportExcelStyle.FontName;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        var huongDans = new[]
        {
            "1. Mỗi dòng trong sheet Import B.O.M vật tư là một vật tư thành phần trực tiếp của một mã vật tư đầu ra.",
            "2. Không đổi tên, vị trí hoặc xóa cột trong sheet Import B.O.M vật tư.",
            "3. Mã vật tư đầu ra và mã vật tư thành phần phải tồn tại, đang hoạt động trong Danh mục vật tư EMAN.",
            "4. Một mã vật tư đầu ra có nhiều thành phần thì lặp lại Mã vật tư đầu ra trên từng dòng.",
            "5. Số lượng vật tư thành phần phải là số lớn hơn 0 và được lưu tối đa 6 chữ số thập phân.",
            "6. Trong cùng một B.O.M, một mã vật tư thành phần chỉ được xuất hiện một lần.",
            "7. Vật tư đầu ra không được đồng thời là vật tư thành phần của chính nó và hệ thống sẽ kiểm tra vòng lặp B.O.M nhiều cấp.",
            "8. Khi preview, nếu một dòng của một B.O.M bị lỗi thì toàn bộ B.O.M của mã đầu ra đó sẽ không được import.",
            "9. Khi import chính thức, Backend tự sinh số phiên bản tiếp theo của từng mã vật tư đầu ra và luôn tạo ở trạng thái Nháp.",
            "10. Import không tự Hiệu lực B.O.M. Người phụ trách phải kiểm tra phiên bản Nháp rồi Hiệu lực thủ công.",
            "11. Phiên bản mới không làm thay đổi phiên bản B.O.M đang Hiệu lực hiện tại.",
            "12. Backend không bắt tổng các thành phần phải bằng 1 hoặc 100% vì quy tắc này chưa được nghiệp vụ xác nhận.",
            "13. Cột Ghi chú không bắt buộc. Các cột có chữ màu đỏ là bắt buộc.",
            "14. Dung lượng file tối đa 20 MB và chỉ hỗ trợ định dạng .xlsx."
        };

        for (var index = 0; index < huongDans.Length; index++)
        {
            worksheet.Cell(index + 3, 1).Value = huongDans[index];
        }

        var viDuStart = huongDans.Length + 5;
        worksheet.Cell(viDuStart, 1).Value = "Ví dụ";
        worksheet.Cell(viDuStart, 1).Style.Font.Bold = true;
        var headers = new[] { "MÃ VẬT TƯ ĐẦU RA", "MÃ VẬT TƯ THÀNH PHẦN", "SỐ LƯỢNG" };
        for (var index = 0; index < headers.Length; index++)
        {
            var cell = worksheet.Cell(viDuStart + 1, index + 1);
            cell.Value = headers[index];
            ImportExcelStyle.ApDungHeader(cell, ImportExcelStyle.ProductFill);
        }

        worksheet.Cell(viDuStart + 2, 1).Value = "66KEO200";
        worksheet.Cell(viDuStart + 2, 2).Value = "28SON063";
        worksheet.Cell(viDuStart + 2, 3).Value = 0.6m;
        worksheet.Cell(viDuStart + 3, 1).Value = "66KEO200";
        worksheet.Cell(viDuStart + 3, 2).Value = "28SON064";
        worksheet.Cell(viDuStart + 3, 3).Value = 0.2m;
        worksheet.Cell(viDuStart + 4, 1).Value = "66KEO200";
        worksheet.Cell(viDuStart + 4, 2).Value = "28SON054";
        worksheet.Cell(viDuStart + 4, 3).Value = 0.2m;
        ImportExcelStyle.ApDungVungDuLieu(worksheet.Range(viDuStart + 2, 1, viDuStart + 4, 3));

        worksheet.Column(1).Width = 96;
        worksheet.Columns(2, 3).Width = 24;
        worksheet.Column(1).Style.Alignment.WrapText = true;
        worksheet.SheetView.FreezeRows(1);
    }

    private static void TaoSheetDanhMuc(IXLWorksheet worksheet, IReadOnlyList<DanhMucVatTu> vatTus)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);
        var headers = new[] { "MÃ VẬT TƯ", "TÊN VẬT TƯ", "ĐVT", "PHƯƠNG THỨC CUNG ỨNG" };
        for (var index = 0; index < headers.Length; index++)
        {
            worksheet.Cell(1, index + 1).Value = headers[index];
        }
        ImportExcelStyle.ApDungHeaderDanhMuc(worksheet.Range(1, 1, 1, headers.Length));

        for (var index = 0; index < vatTus.Count; index++)
        {
            var row = index + 2;
            var item = vatTus[index];
            worksheet.Cell(row, 1).Value = item.MaVatTu;
            worksheet.Cell(row, 2).Value = item.TenVatTu;
            worksheet.Cell(row, 3).Value = item.MaDonViTinh;
            worksheet.Cell(row, 4).Value = LayTenPhuongThuc(item.PhuongThucCungUng);
        }

        if (vatTus.Count > 0)
        {
            ImportExcelStyle.ApDungVungDuLieu(worksheet.Range(2, 1, vatTus.Count + 1, headers.Length));
        }
        worksheet.Range(1, 1, Math.Max(1, vatTus.Count + 1), headers.Length).SetAutoFilter();
        worksheet.SheetView.FreezeRows(1);
        worksheet.Column(1).Width = 24;
        worksheet.Column(2).Width = 42;
        worksheet.Column(3).Width = 14;
        worksheet.Column(4).Width = 26;
        worksheet.Column(2).Style.Alignment.WrapText = true;
    }

    private static string LayTenPhuongThuc(PhuongThucCungUngVatTu value)
        => value switch
        {
            PhuongThucCungUngVatTu.ChiMuaNgoai => "1 - Chỉ mua ngoài",
            PhuongThucCungUngVatTu.MuaHoacTuSanXuat => "2 - Mua hoặc tự sản xuất",
            PhuongThucCungUngVatTu.ChiTuSanXuat => "3 - Chỉ tự sản xuất",
            _ => ((byte)value).ToString()
        };

    private sealed record DanhMucVatTu(
        string MaVatTu,
        string TenVatTu,
        string MaDonViTinh,
        PhuongThucCungUngVatTu PhuongThucCungUng);
}
