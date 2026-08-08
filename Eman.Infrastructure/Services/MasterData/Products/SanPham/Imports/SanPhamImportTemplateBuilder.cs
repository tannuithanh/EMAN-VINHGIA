using ClosedXML.Excel;
using Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Eman.Infrastructure.Services.Imports.Common.Excel;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Services.MasterData.Products.SanPham.Imports;

/// <summary>
/// Tạo file mẫu import sản phẩm theo quy chuẩn Excel của Trading.
/// </summary>
internal sealed class SanPhamImportTemplateBuilder(EmanDbContext dbContext)
{
    private const int HeaderRowIndex = 1;
    private const int DataStartRowIndex = 2;
    private const int DataEndRowIndex = 12001;

    internal static readonly string[] Headers =
    {
        "MÃ HÀNG VG",
        "MÔ TẢ TV",
        "MÔ TẢ TA",
        "ĐVT",
        "Nhóm Năng Lực",
        "L (cm)",
        "W (cm)",
        "H (cm)",
        "Trọng lượng",
        "D/TÍCH",
        "Độ Khó (%)",
        "Hệ số tỉ trọng riêng",
        "Kho mặc định",
        "Kho tồn",
        "Xưởng mặc định",
        "Thuế",
        "Bán TP",
        "Nơi giao hàng",
        "GHI CHÚ"
    };

    private static readonly HashSet<int> CotBatBuoc =
    [
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,
        13, 14, 16, 17, 18
    ];

    public async Task<SanPhamImportFileDto> BuildAsync(
        CancellationToken cancellationToken)
    {
        var danhMuc = await LayDanhMucAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var importSheet = workbook.Worksheets.Add("Import sản phẩm");
        var huongDanSheet = workbook.Worksheets.Add("Hướng dẫn");
        var danhMucSheet = workbook.Worksheets.Add("Danh mục");

        TaoSheetImport(importSheet);
        TaoSheetHuongDan(huongDanSheet);
        TaoSheetDanhMuc(danhMucSheet, danhMuc);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new SanPhamImportFileDto
        {
            Content = stream.ToArray()
        };
    }

    private static void TaoSheetImport(IXLWorksheet worksheet)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);

        for (var index = 0; index < Headers.Length; index++)
        {
            var columnIndex = index + 1;
            var cell = worksheet.Cell(HeaderRowIndex, columnIndex);
            cell.Value = Headers[index];

            var mauNen = columnIndex switch
            {
                >= 1 and <= 12 => ImportExcelStyle.ProductFill,
                >= 13 and <= 16 => ImportExcelStyle.CatalogFill,
                17 => ImportExcelStyle.StatusFill,
                18 => ImportExcelStyle.DestinationFill,
                _ => ImportExcelStyle.ProductFill
            };

            ImportExcelStyle.ApDungHeader(
                cell,
                mauNen,
                chuDo: CotBatBuoc.Contains(columnIndex));
        }

        var dataRange = worksheet.Range(
            DataStartRowIndex,
            1,
            DataEndRowIndex,
            Headers.Length);
        ImportExcelStyle.ApDungVungDuLieu(dataRange);

        worksheet.Range(DataStartRowIndex, 1, DataEndRowIndex, 5)
            .Style.NumberFormat.Format = "@";
        worksheet.Range(DataStartRowIndex, 13, DataEndRowIndex, 19)
            .Style.NumberFormat.Format = "@";

        foreach (var columnIndex in new[] { 6, 7, 8, 9 })
        {
            worksheet.Column(columnIndex).Style.NumberFormat.Format = "0.###";
        }

        worksheet.Column(10).Style.NumberFormat.Format = "0.####";
        worksheet.Column(11).Style.NumberFormat.Format = "0.####";
        worksheet.Column(12).Style.NumberFormat.Format = "0.######";
        worksheet.Column(17).Style.NumberFormat.Format = "0";

        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, 1, Headers.Length).SetAutoFilter();
        worksheet.Row(1).Height = 58;
        worksheet.Row(2).Height = 22;

        SetColumnWidths(worksheet);

        foreach (var columnIndex in new[]
                 {
                     1, 4, 5, 6, 7, 8, 9, 10, 11, 12,
                     13, 14, 15, 16, 17, 18
                 })
        {
            worksheet.Column(columnIndex).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
        }

        worksheet.Column(2).Style.Alignment.WrapText = true;
        worksheet.Column(3).Style.Alignment.WrapText = true;
        worksheet.Column(19).Style.Alignment.WrapText = true;
    }

    private static void TaoSheetHuongDan(IXLWorksheet worksheet)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);

        worksheet.Cell(1, 1).Value = "Quy tắc import sản phẩm EMAN";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontName = ImportExcelStyle.FontName;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        var huongDans = new[]
        {
            "1. Không đổi tên, vị trí hoặc xóa cột trong sheet Import sản phẩm.",
            "2. ĐVT, Nhóm Năng Lực, Kho, Xưởng và Thuế phải nhập theo MÃ trong sheet Danh mục; không nhập GUID.",
            "3. Bán TP chỉ nhận 0 hoặc 1: 0 là thành phẩm, 1 là bán thành phẩm.",
            "4. L, W, H dùng đơn vị cm. Các cột số chỉ nhập số, không nhập kèm chữ hoặc ký hiệu phần trăm.",
            "5. Độ Khó (%) nhập giá trị từ 0 đến 100.",
            "6. Kho tồn phải là kho được đánh dấu Hàng tồn trong danh mục EMAN.",
            "7. Kho mặc định và Kho tồn không được giống nhau.",
            "8. Nơi giao hàng là nội dung tự do, không ánh xạ với danh mục phân xưởng.",
            "9. Mã hàng VG không được trùng trong cùng file và không được tồn tại trước trong EMAN.",
            "10. Có thể nhập thêm dữ liệu bên dưới vùng được định dạng sẵn; Backend không giới hạn số dòng import.",
            "11. Các cột có chữ màu đỏ là bắt buộc phải có dữ liệu.",
            "12. Khi import chính thức, hệ thống chỉ ghi các dòng hợp lệ và tự động bỏ qua các dòng lỗi.",
            "13. Mã hàng VG đã tồn tại trong EMAN sẽ bị bỏ qua và không được import lại.",
            "14. Hãy chạy Xem trước trước khi thực hiện Import chính thức."
        };

        for (var index = 0; index < huongDans.Length; index++)
        {
            worksheet.Cell(index + 2, 1).Value = huongDans[index];
        }

        worksheet.Column(1).Width = 135;
        worksheet.Column(1).Style.Alignment.WrapText = true;
    }

    private static void TaoSheetDanhMuc(
        IXLWorksheet worksheet,
        DanhMucTemplateData danhMuc)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);

        GhiDanhMuc(worksheet, 1, "MÃ ĐVT", "TÊN ĐVT", danhMuc.DonViTinhs);
        GhiDanhMuc(
            worksheet,
            4,
            "MÃ NHÓM NĂNG LỰC",
            "TÊN NHÓM NĂNG LỰC",
            danhMuc.NhomNangLucs);
        GhiDanhMuc(worksheet, 7, "MÃ KHO", "TÊN KHO", danhMuc.Khos);
        GhiDanhMuc(
            worksheet,
            10,
            "MÃ KHO TỒN",
            "TÊN KHO TỒN",
            danhMuc.KhoTons);
        GhiDanhMuc(
            worksheet,
            13,
            "MÃ XƯỞNG",
            "TÊN XƯỞNG",
            danhMuc.PhanXuongs);
        GhiDanhMuc(worksheet, 16, "MÃ THUẾ", "TÊN THUẾ", danhMuc.Thues);

        worksheet.Cell(1, 19).Value = "GIÁ TRỊ BÁN TP";
        worksheet.Cell(1, 20).Value = "Ý NGHĨA";
        ImportExcelStyle.ApDungHeaderDanhMuc(worksheet.Range(1, 19, 1, 20));
        worksheet.Cell(2, 19).Value = 0;
        worksheet.Cell(2, 20).Value = "Thành phẩm";
        worksheet.Cell(3, 19).Value = 1;
        worksheet.Cell(3, 20).Value = "Bán thành phẩm";
        ImportExcelStyle.ApDungVungDuLieu(worksheet.Range(2, 19, 3, 20));

        foreach (var columnIndex in new[] { 1, 4, 7, 10, 13, 16, 19 })
        {
            worksheet.Column(columnIndex).Width = 22;
            worksheet.Column(columnIndex).Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;
        }

        foreach (var columnIndex in new[] { 2, 5, 8, 11, 14, 17, 20 })
        {
            worksheet.Column(columnIndex).Width = 34;
        }

        worksheet.SheetView.FreezeRows(1);
    }

    private static void GhiDanhMuc(
        IXLWorksheet worksheet,
        int startColumn,
        string maHeader,
        string tenHeader,
        IReadOnlyList<DanhMucCodeName> items)
    {
        worksheet.Cell(1, startColumn).Value = maHeader;
        worksheet.Cell(1, startColumn + 1).Value = tenHeader;
        ImportExcelStyle.ApDungHeaderDanhMuc(
            worksheet.Range(1, startColumn, 1, startColumn + 1));

        for (var index = 0; index < items.Count; index++)
        {
            worksheet.Cell(index + 2, startColumn).Value = items[index].Ma;
            worksheet.Cell(index + 2, startColumn + 1).Value = items[index].Ten;
        }

        if (items.Count > 0)
        {
            ImportExcelStyle.ApDungVungDuLieu(
                worksheet.Range(
                    2,
                    startColumn,
                    items.Count + 1,
                    startColumn + 1));
        }
    }

    private async Task<DanhMucTemplateData> LayDanhMucAsync(
        CancellationToken cancellationToken)
    {
        var donViTinhs = await dbContext.DonViTinhs
            .AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaDonViTinh)
            .Select(item => new DanhMucCodeName(item.MaDonViTinh, item.TenDonViTinh))
            .ToListAsync(cancellationToken);

        var nhomNangLucs = await dbContext.NhomNangLucs
            .AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaNhomNangLuc)
            .Select(item => new DanhMucCodeName(item.MaNhomNangLuc, item.TenNhomNangLuc))
            .ToListAsync(cancellationToken);

        var khos = await dbContext.Khos
            .AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaKho)
            .Select(item => new DanhMucCodeName(item.MaKho, item.TenKho))
            .ToListAsync(cancellationToken);

        var khoTons = await dbContext.Khos
            .AsNoTracking()
            .Where(item =>
                item.TrangThai == TrangThaiHoatDong.HoatDong &&
                item.HangTon)
            .OrderBy(item => item.MaKho)
            .Select(item => new DanhMucCodeName(item.MaKho, item.TenKho))
            .ToListAsync(cancellationToken);

        var phanXuongs = await dbContext.PhanXuongs
            .AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaPhanXuong)
            .Select(item => new DanhMucCodeName(item.MaPhanXuong, item.TenPhanXuong))
            .ToListAsync(cancellationToken);

        var thues = await dbContext.ThueSanPhams
            .AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaThue)
            .Select(item => new DanhMucCodeName(item.MaThue, item.TenThue))
            .ToListAsync(cancellationToken);

        return new DanhMucTemplateData(
            donViTinhs,
            nhomNangLucs,
            khos,
            khoTons,
            phanXuongs,
            thues);
    }

    private static void SetColumnWidths(IXLWorksheet worksheet)
    {
        var widths = new double[]
        {
            20, 34, 34, 10, 20, 8, 8, 8, 12, 11,
            13, 19, 16, 14, 23, 12, 10, 23, 32
        };

        for (var index = 0; index < widths.Length; index++)
        {
            worksheet.Column(index + 1).Width = widths[index];
        }
    }

    private sealed record DanhMucCodeName(string Ma, string Ten);

    private sealed record DanhMucTemplateData(
        IReadOnlyList<DanhMucCodeName> DonViTinhs,
        IReadOnlyList<DanhMucCodeName> NhomNangLucs,
        IReadOnlyList<DanhMucCodeName> Khos,
        IReadOnlyList<DanhMucCodeName> KhoTons,
        IReadOnlyList<DanhMucCodeName> PhanXuongs,
        IReadOnlyList<DanhMucCodeName> Thues);
}
