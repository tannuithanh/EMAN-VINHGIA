using ClosedXML.Excel;
using Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Dtos;
using Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using Eman.Infrastructure.Services.Imports.Common.Excel;
using Eman.Infrastructure.Services.MasterData.Materials.VatTu.Imports;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Infrastructure.Services.MasterData.Materials.VatTu.Exports;

/// <summary>
/// Xuất danh mục vật tư ra Excel theo đúng cấu trúc và quy chuẩn trình bày của form import vật tư.
/// Dữ liệu xuất không bị giới hạn bởi phân trang của màn hình danh sách.
/// </summary>
internal sealed class VatTuExportService(IVatTuRepository repository) : IVatTuExportService
{
    private static readonly HashSet<int> CotBatBuoc = [1, 2, 4, 7, 9, 13];

    public async Task<VatTuExportFileDto> XuatExcelAsync(
        BoLocXuatVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var phamVi = request.PhamViSuDung.HasValue
            ? (PhamViSuDungVatTu?)request.PhamViSuDung.Value
            : null;
        var phuongThuc = request.PhuongThucCungUng.HasValue
            ? (PhuongThucCungUngVatTu?)request.PhuongThucCungUng.Value
            : null;
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var vatTus = await repository.LayDanhSachXuatAsync(
            request.Keyword,
            ChuanHoaGuidTuyChon(request.DonViTinhId),
            ChuanHoaGuidTuyChon(request.NhomVatTuId),
            ChuanHoaGuidTuyChon(request.CoSoMuaVatTuId),
            ChuanHoaGuidTuyChon(request.NhaCungCapMacDinhId),
            ChuanHoaGuidTuyChon(request.ThueVatId),
            ChuanHoaGuidTuyChon(request.KhoLuuTruId),
            ChuanHoaGuidTuyChon(request.PhanXuongId),
            phamVi,
            phuongThuc,
            trangThai,
            cancellationToken);

        using var workbook = new XLWorkbook();
        var vatTuSheet = workbook.Worksheets.Add("Import vật tư");
        var phanXuongSheet = workbook.Worksheets.Add("Phân xưởng sử dụng");

        TaoSheetVatTu(vatTuSheet, vatTus);
        TaoSheetPhanXuong(phanXuongSheet, vatTus);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return new VatTuExportFileDto
        {
            Content = stream.ToArray(),
            FileName = $"Vat-Tu-EMAN_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
        };
    }

    private static void TaoSheetVatTu(IXLWorksheet worksheet, IReadOnlyList<VatTuEntity> vatTus)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);

        for (var index = 0; index < VatTuImportTemplateBuilder.Headers.Length; index++)
        {
            var columnIndex = index + 1;
            var cell = worksheet.Cell(1, columnIndex);
            cell.Value = VatTuImportTemplateBuilder.Headers[index];
            var mauNen = columnIndex switch
            {
                <= 9 => ImportExcelStyle.ProductFill,
                <= 15 => ImportExcelStyle.CatalogFill,
                _ => ImportExcelStyle.DestinationFill
            };
            ImportExcelStyle.ApDungHeader(cell, mauNen, CotBatBuoc.Contains(columnIndex));
        }

        for (var index = 0; index < vatTus.Count; index++)
        {
            var rowIndex = index + 2;
            var vatTu = vatTus[index];

            worksheet.Cell(rowIndex, 1).Value = vatTu.MaVatTu;
            worksheet.Cell(rowIndex, 2).Value = vatTu.TenVatTu;
            worksheet.Cell(rowIndex, 3).Value = vatTu.TenTiengAnh ?? string.Empty;
            worksheet.Cell(rowIndex, 4).Value = vatTu.DonViTinh.MaDonViTinh;
            worksheet.Cell(rowIndex, 5).Value = vatTu.QuyCachDongGoi ?? string.Empty;
            worksheet.Cell(rowIndex, 6).Value = LayPhamVi(vatTu.PhamViSuDung);
            worksheet.Cell(rowIndex, 7).Value = vatTu.NhomVatTu.MaNhomVatTu;
            worksheet.Cell(rowIndex, 8).Value = vatTu.MucDichSuDung ?? string.Empty;
            worksheet.Cell(rowIndex, 9).Value = LayPhuongThuc(vatTu.PhuongThucCungUng);
            worksheet.Cell(rowIndex, 10).Value = vatTu.CoSoMuaVatTu?.MaCoSoMuaVatTu ?? string.Empty;
            worksheet.Cell(rowIndex, 11).Value = vatTu.NhaCungCapMacDinh?.MaDoiTac ?? string.Empty;

            if (vatTu.NgayMuaHang.HasValue)
            {
                worksheet.Cell(rowIndex, 12).Value = vatTu.NgayMuaHang.Value;
            }

            worksheet.Cell(rowIndex, 13).Value = vatTu.HanSuDungNgay;

            if (vatTu.Moq.HasValue)
            {
                worksheet.Cell(rowIndex, 14).Value = vatTu.Moq.Value;
            }

            worksheet.Cell(rowIndex, 15).Value = vatTu.ThueVat?.MaThue ?? string.Empty;

            if (vatTu.TonToiThieu.HasValue)
            {
                worksheet.Cell(rowIndex, 16).Value = vatTu.TonToiThieu.Value;
            }

            worksheet.Cell(rowIndex, 17).Value = vatTu.KhoLuuTru?.MaKho ?? string.Empty;
        }

        if (vatTus.Count > 0)
        {
            var lastRow = vatTus.Count + 1;
            var dataRange = worksheet.Range(2, 1, lastRow, VatTuImportTemplateBuilder.Headers.Length);
            ImportExcelStyle.ApDungVungDuLieu(dataRange);
            worksheet.Range(2, 1, lastRow, 11).Style.NumberFormat.Format = "@";
            worksheet.Range(2, 15, lastRow, 17).Style.NumberFormat.Format = "@";
        }

        worksheet.Column(12).Style.NumberFormat.Format = "0";
        worksheet.Column(13).Style.NumberFormat.Format = "0";
        worksheet.Column(14).Style.NumberFormat.Format = "0.###";
        worksheet.Column(16).Style.NumberFormat.Format = "0.###";

        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, Math.Max(1, vatTus.Count + 1), VatTuImportTemplateBuilder.Headers.Length)
            .SetAutoFilter();
        worksheet.Row(1).Height = 58;

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

    private static void TaoSheetPhanXuong(IXLWorksheet worksheet, IReadOnlyList<VatTuEntity> vatTus)
    {
        ImportExcelStyle.ApDungFontToanSheet(worksheet);

        for (var index = 0; index < VatTuImportTemplateBuilder.WorkshopHeaders.Length; index++)
        {
            var cell = worksheet.Cell(1, index + 1);
            cell.Value = VatTuImportTemplateBuilder.WorkshopHeaders[index];
            ImportExcelStyle.ApDungHeader(cell, ImportExcelStyle.CatalogFill, chuDo: true);
        }

        var rowIndex = 2;
        foreach (var vatTu in vatTus.OrderBy(item => item.MaVatTu))
        {
            foreach (var link in vatTu.PhanXuongs.OrderBy(item => item.PhanXuong.MaPhanXuong))
            {
                worksheet.Cell(rowIndex, 1).Value = vatTu.MaVatTu;
                worksheet.Cell(rowIndex, 2).Value = link.PhanXuong.MaPhanXuong;
                rowIndex++;
            }
        }

        if (rowIndex > 2)
        {
            ImportExcelStyle.ApDungVungDuLieu(worksheet.Range(2, 1, rowIndex - 1, 2));
            worksheet.Range(2, 1, rowIndex - 1, 2).Style.NumberFormat.Format = "@";
        }

        worksheet.Column(1).Width = 24;
        worksheet.Column(2).Width = 24;
        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, Math.Max(1, rowIndex - 1), 2).SetAutoFilter();
    }

    private static string LayPhamVi(PhamViSuDungVatTu? value) => value switch
    {
        PhamViSuDungVatTu.TatCaPhanXuong => "1 - Tất cả phân xưởng",
        PhamViSuDungVatTu.PhanXuongCuThe => "2 - Phân xưởng cụ thể",
        _ => string.Empty
    };

    private static string LayPhuongThuc(PhuongThucCungUngVatTu value) => value switch
    {
        PhuongThucCungUngVatTu.ChiMuaNgoai => "1 - Chỉ mua ngoài",
        PhuongThucCungUngVatTu.MuaHoacTuSanXuat => "2 - Mua hoặc tự sản xuất",
        PhuongThucCungUngVatTu.ChiTuSanXuat => "3 - Chỉ tự sản xuất",
        _ => string.Empty
    };

    private static Guid? ChuanHoaGuidTuyChon(Guid? value)
        => value.HasValue && value.Value != Guid.Empty ? value : null;
}
