using ClosedXML.Excel;

namespace Eman.Infrastructure.Services.Imports.Common.Excel;

/// <summary>
/// Quy chuẩn trình bày dùng chung cho toàn bộ form import EMAN.
/// Màu sắc và font được đồng bộ theo các template import của Trading.
/// </summary>
internal static class ImportExcelStyle
{
    public const string FontName = "Cambria";
    public const double BodyFontSize = 11;
    public const double HeaderFontSize = 12;
    public const string ProductFill = "#FCE4D6";
    public const string CatalogFill = "#F8CBAD";
    public const string StatusFill = "#FFD966";
    public const string DestinationFill = "#E2F0D9";

    public static void ApDungFontToanSheet(IXLWorksheet worksheet)
    {
        worksheet.Style.Font.FontName = FontName;
        worksheet.Style.Font.FontSize = BodyFontSize;
    }

    public static void ApDungHeader(
        IXLCell cell,
        string mauNen,
        bool chuDo = false)
    {
        cell.Style.Font.FontName = FontName;
        cell.Style.Font.FontSize = HeaderFontSize;
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontColor = chuDo ? XLColor.Red : XLColor.Black;
        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(mauNen);
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        cell.Style.Alignment.WrapText = true;
        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }

    public static void ApDungVungDuLieu(IXLRange range)
    {
        range.Style.Font.FontName = FontName;
        range.Style.Font.FontSize = BodyFontSize;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        range.Style.Fill.BackgroundColor = XLColor.NoColor;
    }

    public static void ApDungHeaderDanhMuc(IXLRange range)
    {
        range.Style.Font.FontName = FontName;
        range.Style.Font.FontSize = BodyFontSize;
        range.Style.Font.Bold = true;
        range.Style.Fill.BackgroundColor = XLColor.FromHtml(ProductFill);
        range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        range.Style.Alignment.WrapText = true;
        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    }
}
