using ClosedXML.Excel;
using Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Eman.Infrastructure.Services.Imports.Common.Excel;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Services.MasterData.Materials.VatTu.Imports;

/// <summary>
/// Tạo file mẫu import vật tư và danh sách mã danh mục hiện hành.
/// </summary>
internal sealed class VatTuImportTemplateBuilder(EmanDbContext dbContext)
{
    private const int HeaderRowIndex = 1;
    private const int DataStartRowIndex = 2;
    private const int DataEndRowIndex = 12001;

    internal static readonly string[] Headers =
    {
        "MÃ VẬT TƯ",
        "TÊN VẬT TƯ",
        "TÊN TIẾNG ANH",
        "ĐVT",
        "QUY CÁCH ĐÓNG GÓI",
        "PHẠM VI SỬ DỤNG",
        "NHÓM VẬT TƯ",
        "MỤC ĐÍCH SỬ DỤNG",
        "PHƯƠNG THỨC CUNG ỨNG",
        "CƠ SỞ MUA",
        "NCC MẶC ĐỊNH",
        "THỜI GIAN MUA HÀNG (NGÀY)",
        "HẠN SỬ DỤNG (NGÀY)",
        "MOQ",
        "THUẾ VAT",
        "TỒN TỐI THIỂU",
        "KHO LƯU TRỮ"
    };

    internal static readonly string[] WorkshopHeaders =
    {
        "MÃ VẬT TƯ",
        "MÃ PHÂN XƯỞNG"
    };

    private static readonly HashSet<int> CotBatBuoc =
    [
        1, 2, 4, 7, 9, 13
    ];

    public async Task<VatTuImportFileDto> BuildAsync(CancellationToken cancellationToken)
    {
        var danhMuc = await LayDanhMucAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var importSheet = workbook.Worksheets.Add("Import vật tư");
        var phanXuongSheet = workbook.Worksheets.Add("Phân xưởng sử dụng");
        var huongDanSheet = workbook.Worksheets.Add("Hướng dẫn");
        var danhMucSheet = workbook.Worksheets.Add("Danh mục");

        TaoSheetImport(importSheet);
        TaoSheetPhanXuong(phanXuongSheet);
        TaoSheetHuongDan(huongDanSheet);
        TaoSheetDanhMuc(danhMucSheet, danhMuc);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return new VatTuImportFileDto { Content = stream.ToArray() };
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
                <= 9 => ImportExcelStyle.ProductFill,
                <= 15 => ImportExcelStyle.CatalogFill,
                _ => ImportExcelStyle.DestinationFill
            };
            ImportExcelStyle.ApDungHeader(cell, mauNen, CotBatBuoc.Contains(columnIndex));
        }

        var dataRange = worksheet.Range(DataStartRowIndex, 1, DataEndRowIndex, Headers.Length);
        ImportExcelStyle.ApDungVungDuLieu(dataRange);
        worksheet.Range(DataStartRowIndex, 1, DataEndRowIndex, 11).Style.NumberFormat.Format = "@";
        worksheet.Column(12).Style.NumberFormat.Format = "0";
        worksheet.Column(13).Style.NumberFormat.Format = "0";
        worksheet.Column(14).Style.NumberFormat.Format = "0.###";
        worksheet.Column(16).Style.NumberFormat.Format = "0.###";
        worksheet.Range(DataStartRowIndex, 15, DataEndRowIndex, 17).Style.NumberFormat.Format = "@";

        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, 1, Headers.Length).SetAutoFilter();
        worksheet.Row(1).Height = 58;
        worksheet.Row(2).Height = 22;

        var widths = new double[]
        {
            20, 30, 28, 12, 28, 24, 20, 32, 28,
            20, 22, 18, 20, 14, 15, 18, 18
        };
        for (var index = 0; index < widths.Length; index++)
        {
            worksheet.Column(index + 1).Width = widths[index];
        }

        foreach (var columnIndex in new[] { 1, 4, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17 })
        {
            worksheet.Column(columnIndex).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
        foreach (var columnIndex in new[] { 2, 3, 5, 8 })
        {
            worksheet.Column(columnIndex).Style.Alignment.WrapText = true;
        }
    }

    private static void TaoSheetPhanXuong(IXLWorksheet worksheet)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);
        for (var index = 0; index < WorkshopHeaders.Length; index++)
        {
            var cell = worksheet.Cell(1, index + 1);
            cell.Value = WorkshopHeaders[index];
            ImportExcelStyle.ApDungHeader(cell, ImportExcelStyle.CatalogFill, true);
        }
        ImportExcelStyle.ApDungVungDuLieu(worksheet.Range(2, 1, DataEndRowIndex, 2));
        worksheet.Range(2, 1, DataEndRowIndex, 2).Style.NumberFormat.Format = "@";
        worksheet.Column(1).Width = 24;
        worksheet.Column(2).Width = 24;
        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, 1, 2).SetAutoFilter();
    }

    private static void TaoSheetHuongDan(IXLWorksheet worksheet)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);
        worksheet.Cell(1, 1).Value = "Quy tắc import vật tư EMAN";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontName = ImportExcelStyle.FontName;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        var huongDans = new[]
        {
            "1. Tất cả vật tư ở mọi cấp đều nhập chung trong sheet Import vật tư; không nhập cấp vật tư và chưa khai báo BOM tại đây.",
            "2. Không đổi tên, vị trí hoặc xóa cột trong hai sheet Import vật tư và Phân xưởng sử dụng.",
            "3. ĐVT, Nhóm vật tư, Cơ sở mua, NCC mặc định, Thuế VAT, Kho và Phân xưởng khi có nhập phải dùng MÃ trong sheet Danh mục; không nhập GUID.",
            "4. Phạm vi sử dụng không bắt buộc. Khi có nhập: 1 = Tất cả phân xưởng; 2 = Phân xưởng cụ thể.",
            "5. Khi Phạm vi sử dụng = 2, phải khai ít nhất một dòng Mã vật tư - Mã phân xưởng trong sheet Phân xưởng sử dụng.",
            "6. Phương thức cung ứng: 1 = Chỉ mua ngoài; 2 = Mua hoặc tự sản xuất; 3 = Chỉ tự sản xuất.",
            "7. Khi phương thức cung ứng là 1 hoặc 2, bắt buộc nhập Cơ sở mua, Thời gian mua hàng và Thuế VAT. MOQ và NCC mặc định không bắt buộc.",
            "8. Khi phương thức cung ứng là 3, hệ thống bỏ qua các thông tin mua hàng và lưu các trường mua hàng bằng NULL.",
            "9. Thời gian mua hàng là số nguyên ngày lớn hơn hoặc bằng 0, tính từ lúc đặt hàng đến khi có hàng; khi phương thức cung ứng là 1 hoặc 2 thì bắt buộc phải nhập. Hạn sử dụng bắt buộc cho mọi vật tư và phải là số nguyên lớn hơn hoặc bằng 0 ngày.",
            "10. MOQ không bắt buộc; nếu nhập phải là số lớn hơn 0. Tồn tối thiểu không bắt buộc; nếu nhập phải lớn hơn hoặc bằng 0.",
            "11. Mã vật tư không được trùng trong cùng file và không được tồn tại trước trong EMAN.",
            "12. Các cột có chữ màu đỏ là bắt buộc cho mọi vật tư, bao gồm Hạn sử dụng. Phạm vi sử dụng, tồn tối thiểu và kho lưu trữ là tùy chọn; các cột mua hàng áp dụng theo điều kiện tại mục 7.",
            "13. Backend không giới hạn số dòng import; giới hạn dung lượng file là 20 MB.",
            "14. Khi import chính thức, hệ thống chỉ ghi các dòng hợp lệ và tự động bỏ qua các dòng lỗi.",
            "15. Hãy chạy endpoint Xem trước trước khi thực hiện Import chính thức."
        };

        for (var index = 0; index < huongDans.Length; index++)
        {
            worksheet.Cell(index + 2, 1).Value = huongDans[index];
        }
        worksheet.Column(1).Width = 140;
        worksheet.Column(1).Style.Alignment.WrapText = true;
    }

    private static void TaoSheetDanhMuc(IXLWorksheet worksheet, DanhMucTemplateData danhMuc)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);
        GhiDanhMuc(worksheet, 1, "MÃ ĐVT", "TÊN ĐVT", danhMuc.DonViTinhs);
        GhiDanhMuc(worksheet, 4, "MÃ NHÓM VẬT TƯ", "TÊN NHÓM VẬT TƯ", danhMuc.NhomVatTus);
        GhiDanhMuc(worksheet, 7, "MÃ CƠ SỞ MUA", "TÊN CƠ SỞ MUA", danhMuc.CoSoMuas);
        GhiDanhMuc(worksheet, 10, "MÃ NCC", "TÊN NCC", danhMuc.NhaCungCaps);
        GhiDanhMuc(worksheet, 13, "MÃ THUẾ", "TÊN THUẾ", danhMuc.Thues);
        GhiDanhMuc(worksheet, 16, "MÃ KHO", "TÊN KHO", danhMuc.Khos);
        GhiDanhMuc(worksheet, 19, "MÃ PHÂN XƯỞNG", "TÊN PHÂN XƯỞNG", danhMuc.PhanXuongs);

        GhiLuaChon(worksheet, 22, "PHẠM VI", new[]
        {
            new DanhMucCodeName("1", "Tất cả phân xưởng"),
            new DanhMucCodeName("2", "Phân xưởng cụ thể")
        });
        GhiLuaChon(worksheet, 25, "PHƯƠNG THỨC", new[]
        {
            new DanhMucCodeName("1", "Chỉ mua ngoài"),
            new DanhMucCodeName("2", "Mua hoặc tự sản xuất"),
            new DanhMucCodeName("3", "Chỉ tự sản xuất")
        });

        for (var columnIndex = 1; columnIndex <= 27; columnIndex++)
        {
            worksheet.Column(columnIndex).Width = columnIndex % 3 == 1 ? 22 : 34;
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
        ImportExcelStyle.ApDungHeaderDanhMuc(worksheet.Range(1, startColumn, 1, startColumn + 1));
        for (var index = 0; index < items.Count; index++)
        {
            worksheet.Cell(index + 2, startColumn).Value = items[index].Ma;
            worksheet.Cell(index + 2, startColumn + 1).Value = items[index].Ten;
        }
        if (items.Count > 0)
        {
            ImportExcelStyle.ApDungVungDuLieu(
                worksheet.Range(2, startColumn, items.Count + 1, startColumn + 1));
        }
    }

    private static void GhiLuaChon(
        IXLWorksheet worksheet,
        int startColumn,
        string header,
        IReadOnlyList<DanhMucCodeName> items)
        => GhiDanhMuc(worksheet, startColumn, $"GIÁ TRỊ {header}", "Ý NGHĨA", items);

    private async Task<DanhMucTemplateData> LayDanhMucAsync(CancellationToken cancellationToken)
    {
        var donViTinhs = await dbContext.DonViTinhs.AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaDonViTinh)
            .Select(item => new DanhMucCodeName(item.MaDonViTinh, item.TenDonViTinh))
            .ToListAsync(cancellationToken);
        var nhomVatTus = await dbContext.NhomVatTus.AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaNhomVatTu)
            .Select(item => new DanhMucCodeName(item.MaNhomVatTu, item.TenNhomVatTu))
            .ToListAsync(cancellationToken);
        var coSoMuas = await dbContext.CoSoMuaVatTus.AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaCoSoMuaVatTu)
            .Select(item => new DanhMucCodeName(item.MaCoSoMuaVatTu, item.TenCoSoMuaVatTu))
            .ToListAsync(cancellationToken);
        var nhaCungCaps = await dbContext.DoiTacKinhDoanhs.AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong && item.LaNhaCungCap)
            .OrderBy(item => item.MaDoiTac)
            .Select(item => new DanhMucCodeName(item.MaDoiTac, item.TenDoiTac))
            .ToListAsync(cancellationToken);
        var thues = await dbContext.ThueSanPhams.AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaThue)
            .Select(item => new DanhMucCodeName(item.MaThue, item.TenThue))
            .ToListAsync(cancellationToken);
        var khos = await dbContext.Khos.AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaKho)
            .Select(item => new DanhMucCodeName(item.MaKho, item.TenKho))
            .ToListAsync(cancellationToken);
        var phanXuongs = await dbContext.PhanXuongs.AsNoTracking()
            .Where(item => item.TrangThai == TrangThaiHoatDong.HoatDong)
            .OrderBy(item => item.MaPhanXuong)
            .Select(item => new DanhMucCodeName(item.MaPhanXuong, item.TenPhanXuong))
            .ToListAsync(cancellationToken);

        return new DanhMucTemplateData(
            donViTinhs, nhomVatTus, coSoMuas, nhaCungCaps, thues, khos, phanXuongs);
    }

    private sealed record DanhMucCodeName(string Ma, string Ten);
    private sealed record DanhMucTemplateData(
        IReadOnlyList<DanhMucCodeName> DonViTinhs,
        IReadOnlyList<DanhMucCodeName> NhomVatTus,
        IReadOnlyList<DanhMucCodeName> CoSoMuas,
        IReadOnlyList<DanhMucCodeName> NhaCungCaps,
        IReadOnlyList<DanhMucCodeName> Thues,
        IReadOnlyList<DanhMucCodeName> Khos,
        IReadOnlyList<DanhMucCodeName> PhanXuongs);
}
