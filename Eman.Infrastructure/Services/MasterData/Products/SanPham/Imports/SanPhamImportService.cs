using ClosedXML.Excel;
using Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;
using Eman.Application.Modules.MasterData.Products.SanPham.Imports.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using SanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.SanPham;

namespace Eman.Infrastructure.Services.MasterData.Products.SanPham.Imports;

/// <summary>
/// Đọc, kiểm tra và ghi dữ liệu sản phẩm từ file Excel vào md_san_pham.
/// </summary>
internal sealed class SanPhamImportService(
    EmanDbContext dbContext,
    SanPhamImportTemplateBuilder templateBuilder) : ISanPhamImportService
{
    private const string SheetName = "Import sản phẩm";
    private const int HeaderRow = 1;
    private const int DataStartRow = 2;
    private const long MaxFileSize = 20 * 1024 * 1024;

    public Task<SanPhamImportFileDto> TaoTemplateAsync(
        CancellationToken cancellationToken = default)
        => templateBuilder.BuildAsync(cancellationToken);

    public async Task<SanPhamImportPreviewDto> XemTruocAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        CancellationToken cancellationToken = default)
    {
        var ketQua = await DocVaKiemTraAsync(
            fileStream,
            fileName,
            fileSize,
            cancellationToken);

        return TaoPreview(ketQua.Rows);
    }

    public async Task<SanPhamImportResultDto> ImportAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        string? createdByMsnv,
        CancellationToken cancellationToken = default)
    {
        var ketQua = await DocVaKiemTraAsync(
            fileStream,
            fileName,
            fileSize,
            cancellationToken);
        var preview = TaoPreview(ketQua.Rows);

        if (!preview.CoTheImport)
        {
            return new SanPhamImportResultDto
            {
                ThanhCong = false,
                Message = preview.TongSoDong == 0
                    ? "File import không có dòng dữ liệu sản phẩm."
                    : $"Không có dòng hợp lệ để import. File có {preview.SoDongLoi} dòng lỗi.",
                TongSoDong = preview.TongSoDong,
                SoDongDaImport = 0,
                SoDongBoQua = preview.SoDongLoi,
                XemTruoc = preview
            };
        }

        var nguoiTao = ChuanHoaTuyChon(createdByMsnv);
        if (nguoiTao?.Length > 50)
        {
            return new SanPhamImportResultDto
            {
                ThanhCong = false,
                Message = "Mã nhân viên người import không được vượt quá 50 ký tự.",
                TongSoDong = preview.TongSoDong,
                SoDongDaImport = 0,
                SoDongBoQua = preview.SoDongLoi,
                XemTruoc = preview
            };
        }

        var thoiDiemImport = DateTime.UtcNow;
        var dongHopLe = ketQua.Rows
            .Where(row => row.Loi.Count == 0)
            .ToList();

        var entities = dongHopLe.Select(row => new SanPhamEntity
        {
            Id = Guid.NewGuid(),
            MaSanPham = row.MaSanPham!,
            MoTaTiengViet = row.MoTaTiengViet!,
            MoTaTiengAnh = row.MoTaTiengAnh,
            DonViTinhId = row.DonViTinhId!.Value,
            NhomNangLucId = row.NhomNangLucId,
            ChieuDaiCm = row.ChieuDaiCm,
            ChieuRongCm = row.ChieuRongCm,
            ChieuCaoCm = row.ChieuCaoCm,
            TrongLuong = row.TrongLuong,
            DienTich = row.DienTich,
            DoKho = row.DoKho,
            HeSoTiTrong = row.HeSoTiTrong,
            CbmMacDinh = TinhCbm(row.ChieuDaiCm, row.ChieuRongCm, row.ChieuCaoCm),
            KhoMacDinhId = row.KhoMacDinhId,
            KhoTonId = row.KhoTonId,
            XuongMacDinhId = row.XuongMacDinhId,
            ThueId = row.ThueId,
            LaBanThanhPham = row.LaBanThanhPham!.Value,
            NoiGiaoHang = row.NoiGiaoHang,
            GhiChu = row.GhiChu,
            TrangThai = TrangThaiHoatDong.HoatDong,
            CreatedAt = thoiDiemImport,
            CreatedByMsnv = nguoiTao
        }).ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken);

        try
        {
            await dbContext.SanPhams.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var message = preview.SoDongLoi > 0
            ? $"Import thành công {entities.Count} sản phẩm; đã bỏ qua {preview.SoDongLoi} dòng lỗi."
            : $"Import thành công {entities.Count} sản phẩm vào danh mục sản phẩm EMAN.";

        return new SanPhamImportResultDto
        {
            ThanhCong = true,
            Message = message,
            TongSoDong = preview.TongSoDong,
            SoDongDaImport = entities.Count,
            SoDongBoQua = preview.SoDongLoi
        };
    }

    private async Task<ImportValidationResult> DocVaKiemTraAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        CancellationToken cancellationToken)
    {
        KiemTraFile(fileStream, fileName, fileSize);

        using var workbook = TaoWorkbook(fileStream);
        var worksheet = workbook.Worksheets.FirstOrDefault(item =>
            string.Equals(item.Name, SheetName, StringComparison.OrdinalIgnoreCase));

        if (worksheet is null)
        {
            throw new InvalidOperationException(
                $"Không tìm thấy sheet '{SheetName}'. Vui lòng tải đúng file mẫu từ hệ thống.");
        }

        KiemTraHeader(worksheet);

        var lastRow = worksheet.LastRowUsed(XLCellsUsedOptions.Contents)?.RowNumber()
            ?? HeaderRow;
        if (lastRow < DataStartRow)
        {
            return new ImportValidationResult(Array.Empty<ImportRow>());
        }


        var rows = new List<ImportRow>();
        for (var rowNumber = DataStartRow; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (LaDongTrong(worksheet, rowNumber))
            {
                continue;
            }

            rows.Add(DocDong(worksheet, rowNumber));
        }

        await KiemTraNghiepVuAsync(rows, cancellationToken);
        return new ImportValidationResult(rows);
    }

    private async Task KiemTraNghiepVuAsync(
        List<ImportRow> rows,
        CancellationToken cancellationToken)
    {
        var donViTinhs = (await dbContext.DonViTinhs
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .ToDictionary(
                item => item.MaDonViTinh,
                StringComparer.OrdinalIgnoreCase);
        var nhomNangLucs = (await dbContext.NhomNangLucs
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .ToDictionary(
                item => item.MaNhomNangLuc,
                StringComparer.OrdinalIgnoreCase);
        var khos = (await dbContext.Khos
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .ToDictionary(
                item => item.MaKho,
                StringComparer.OrdinalIgnoreCase);
        var phanXuongs = (await dbContext.PhanXuongs
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .ToDictionary(
                item => item.MaPhanXuong,
                StringComparer.OrdinalIgnoreCase);
        var thues = (await dbContext.ThueSanPhams
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .ToDictionary(
                item => item.MaThue,
                StringComparer.OrdinalIgnoreCase);
        var maSanPhamDaCo = (await dbContext.SanPhams
                .AsNoTracking()
                .Select(item => item.MaSanPham)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            KiemTraBatBuocVaDoDai(row);
            KiemTraSo(row);

            if (!string.IsNullOrWhiteSpace(row.MaDonViTinh))
            {
                if (!donViTinhs.TryGetValue(row.MaDonViTinh, out var donViTinh))
                {
                    row.Loi.Add($"Không tìm thấy đơn vị tính có mã '{row.MaDonViTinh}' trong EMAN.");
                }
                else if (donViTinh.TrangThai != TrangThaiHoatDong.HoatDong)
                {
                    row.Loi.Add($"Đơn vị tính mã '{row.MaDonViTinh}' đang ngừng hoạt động.");
                }
                else
                {
                    row.DonViTinhId = donViTinh.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(row.MaNhomNangLuc))
            {
                if (!nhomNangLucs.TryGetValue(row.MaNhomNangLuc, out var nhomNangLuc))
                {
                    row.Loi.Add($"Không tìm thấy nhóm năng lực có mã '{row.MaNhomNangLuc}' trong EMAN.");
                }
                else if (nhomNangLuc.TrangThai != TrangThaiHoatDong.HoatDong)
                {
                    row.Loi.Add($"Nhóm năng lực mã '{row.MaNhomNangLuc}' đang ngừng hoạt động.");
                }
                else
                {
                    row.NhomNangLucId = nhomNangLuc.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(row.MaKhoMacDinh))
            {
                if (!khos.TryGetValue(row.MaKhoMacDinh, out var khoMacDinh))
                {
                    row.Loi.Add($"Không tìm thấy kho mặc định có mã '{row.MaKhoMacDinh}' trong EMAN.");
                }
                else if (khoMacDinh.TrangThai != TrangThaiHoatDong.HoatDong)
                {
                    row.Loi.Add($"Kho mặc định mã '{row.MaKhoMacDinh}' đang ngừng hoạt động.");
                }
                else
                {
                    row.KhoMacDinhId = khoMacDinh.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(row.MaKhoTon))
            {
                if (!khos.TryGetValue(row.MaKhoTon, out var khoTon))
                {
                    row.Loi.Add($"Không tìm thấy kho tồn có mã '{row.MaKhoTon}' trong EMAN.");
                }
                else if (khoTon.TrangThai != TrangThaiHoatDong.HoatDong)
                {
                    row.Loi.Add($"Kho tồn mã '{row.MaKhoTon}' đang ngừng hoạt động.");
                }
                else if (!khoTon.HangTon)
                {
                    row.Loi.Add($"Kho mã '{row.MaKhoTon}' không được đánh dấu là kho tồn.");
                }
                else
                {
                    row.KhoTonId = khoTon.Id;
                }
            }

            if (GiongNhau(row.MaKhoMacDinh, row.MaKhoTon))
            {
                row.Loi.Add(
                    $"Kho mặc định và Kho tồn không được giống nhau (cùng mã '{row.MaKhoMacDinh}').");
            }

            if (!string.IsNullOrWhiteSpace(row.MaXuongMacDinh))
            {
                if (!phanXuongs.TryGetValue(row.MaXuongMacDinh, out var xuongMacDinh))
                {
                    row.Loi.Add($"Không tìm thấy xưởng mặc định có mã '{row.MaXuongMacDinh}' trong EMAN.");
                }
                else if (xuongMacDinh.TrangThai != TrangThaiHoatDong.HoatDong)
                {
                    row.Loi.Add($"Xưởng mặc định mã '{row.MaXuongMacDinh}' đang ngừng hoạt động.");
                }
                else
                {
                    row.XuongMacDinhId = xuongMacDinh.Id;
                }
            }


            if (!string.IsNullOrWhiteSpace(row.MaThue))
            {
                if (!thues.TryGetValue(row.MaThue, out var thue))
                {
                    row.Loi.Add($"Không tìm thấy thuế có mã '{row.MaThue}' trong EMAN.");
                }
                else if (thue.TrangThai != TrangThaiHoatDong.HoatDong)
                {
                    row.Loi.Add($"Thuế mã '{row.MaThue}' đang ngừng hoạt động.");
                }
                else
                {
                    row.ThueId = thue.Id;
                }
            }

            if (!string.IsNullOrWhiteSpace(row.MaSanPham) &&
                maSanPhamDaCo.Contains(row.MaSanPham))
            {
                row.Loi.Add($"Dòng {row.Dong}: Mã hàng VG '{row.MaSanPham}' đã tồn tại trong EMAN nên không được import lại.");
            }
        }

        var nhomTrung = rows
            .Where(item => !string.IsNullOrWhiteSpace(item.MaSanPham))
            .GroupBy(item => item.MaSanPham!, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1);

        foreach (var group in nhomTrung)
        {
            var cacDong = string.Join(", ", group.Select(item => item.Dong));
            foreach (var row in group)
            {
                row.Loi.Add(
                    $"Mã hàng VG '{group.Key}' bị trùng trong file tại các dòng {cacDong}.");
            }
        }
    }

    private static void KiemTraBatBuocVaDoDai(ImportRow row)
    {
        KiemTraBatBuoc(row.MaNhomNangLuc, "Nhóm năng lực", row.Dong, row.Loi);
        KiemTraBatBuoc(row.MaDonViTinh, "ĐVT", row.Dong, row.Loi);
        KiemTraBatBuoc(row.MaSanPham, "Mã hàng VG", row.Dong, row.Loi);
        KiemTraBatBuoc(row.MoTaTiengViet, "Mô tả tiếng Việt", row.Dong, row.Loi);
        KiemTraBatBuoc(row.MoTaTiengAnh, "Mô tả tiếng Anh", row.Dong, row.Loi);

        KiemTraSoBatBuoc(row.ChieuDaiCm, "L", row.Dong, row.Loi);
        KiemTraSoBatBuoc(row.ChieuRongCm, "W", row.Dong, row.Loi);
        KiemTraSoBatBuoc(row.ChieuCaoCm, "H", row.Dong, row.Loi);
        KiemTraSoBatBuoc(row.TrongLuong, "Trọng lượng", row.Dong, row.Loi);
        KiemTraSoBatBuoc(row.DienTich, "D/TÍCH", row.Dong, row.Loi);
        KiemTraSoBatBuoc(row.DoKho, "Độ khó", row.Dong, row.Loi);
        KiemTraSoBatBuoc(row.HeSoTiTrong, "Hệ số tỉ trọng", row.Dong, row.Loi);

        KiemTraBatBuoc(row.MaKhoMacDinh, "Kho mặc định", row.Dong, row.Loi);
        KiemTraBatBuoc(row.MaKhoTon, "Kho tồn", row.Dong, row.Loi);
        KiemTraBatBuoc(row.MaThue, "Thuế", row.Dong, row.Loi);
        KiemTraBatBuoc(row.NoiGiaoHang, "Nơi giao hàng", row.Dong, row.Loi);

        if (!row.LaBanThanhPham.HasValue &&
            !row.Loi.Any(item => item.StartsWith(
                "Cột Bán TP",
                StringComparison.OrdinalIgnoreCase)))
        {
            row.Loi.Add($"Dòng {row.Dong} không có dữ liệu Bán TP.");
        }

        KiemTraDoDai(row.MaSanPham, 100, "MÃ HÀNG VG", row.Loi);
        KiemTraDoDai(row.MoTaTiengViet, 500, "MÔ TẢ TV", row.Loi);
        KiemTraDoDai(row.MoTaTiengAnh, 500, "MÔ TẢ TA", row.Loi);
        KiemTraDoDai(row.NoiGiaoHang, 500, "Nơi giao hàng", row.Loi);
        KiemTraDoDai(row.GhiChu, 1000, "GHI CHÚ", row.Loi);
    }

    private static void KiemTraSo(ImportRow row)
    {
        foreach (var item in new[]
                 {
                     (row.ChieuDaiCm, "L (cm)"),
                     (row.ChieuRongCm, "W (cm)"),
                     (row.ChieuCaoCm, "H (cm)"),
                     (row.TrongLuong, "Trọng lượng"),
                     (row.DienTich, "D/TÍCH"),
                     (row.DoKho, "Độ Khó (%)"),
                     (row.HeSoTiTrong, "Hệ số tỉ trọng riêng")
                 })
        {
            if (item.Item1.HasValue && item.Item1.Value < 0)
            {
                row.Loi.Add($"Cột {item.Item2} không được nhập số âm.");
            }
        }

        if (row.DoKho.HasValue && row.DoKho.Value > 100)
        {
            row.Loi.Add("Cột Độ Khó (%) phải nằm trong khoảng từ 0 đến 100.");
        }
    }

    private static ImportRow DocDong(IXLWorksheet worksheet, int rowNumber)
    {
        var row = new ImportRow
        {
            Dong = rowNumber,
            MaSanPham = ChuanHoaMa(LayChuoi(worksheet, rowNumber, 1)),
            MoTaTiengViet = ChuanHoaTuyChon(LayChuoi(worksheet, rowNumber, 2)),
            MoTaTiengAnh = ChuanHoaTuyChon(LayChuoi(worksheet, rowNumber, 3)),
            MaDonViTinh = ChuanHoaMa(LayChuoi(worksheet, rowNumber, 4)),
            MaNhomNangLuc = ChuanHoaMa(LayChuoi(worksheet, rowNumber, 5)),
            MaKhoMacDinh = ChuanHoaMa(LayChuoi(worksheet, rowNumber, 13)),
            MaKhoTon = ChuanHoaMa(LayChuoi(worksheet, rowNumber, 14)),
            MaXuongMacDinh = ChuanHoaMa(LayChuoi(worksheet, rowNumber, 15)),
            MaThue = ChuanHoaMa(LayChuoi(worksheet, rowNumber, 16)),
            NoiGiaoHang = ChuanHoaTuyChon(LayChuoi(worksheet, rowNumber, 18)),
            GhiChu = ChuanHoaTuyChon(LayChuoi(worksheet, rowNumber, 19))
        };

        row.ChieuDaiCm = DocSo(worksheet.Cell(rowNumber, 6), "L (cm)", row.Loi);
        row.ChieuRongCm = DocSo(worksheet.Cell(rowNumber, 7), "W (cm)", row.Loi);
        row.ChieuCaoCm = DocSo(worksheet.Cell(rowNumber, 8), "H (cm)", row.Loi);
        row.TrongLuong = DocSo(worksheet.Cell(rowNumber, 9), "Trọng lượng", row.Loi);
        row.DienTich = DocSo(worksheet.Cell(rowNumber, 10), "D/TÍCH", row.Loi);
        row.DoKho = DocSo(worksheet.Cell(rowNumber, 11), "Độ Khó (%)", row.Loi);
        row.HeSoTiTrong = DocSo(
            worksheet.Cell(rowNumber, 12),
            "Hệ số tỉ trọng riêng",
            row.Loi);
        row.LaBanThanhPham = DocBanThanhPham(
            worksheet.Cell(rowNumber, 17),
            row.Loi);

        return row;
    }

    private static SanPhamImportPreviewDto TaoPreview(IReadOnlyList<ImportRow> rows)
    {
        var danhSach = rows.Select(row => new SanPhamImportRowPreviewDto
        {
            Dong = row.Dong,
            MaSanPham = row.MaSanPham,
            MoTaTiengViet = row.MoTaTiengViet,
            MaDonViTinh = row.MaDonViTinh,
            MaNhomNangLuc = row.MaNhomNangLuc,
            MaKhoMacDinh = row.MaKhoMacDinh,
            MaKhoTon = row.MaKhoTon,
            MaXuongMacDinh = row.MaXuongMacDinh,
            MaThue = row.MaThue,
            BanThanhPham = row.LaBanThanhPham.HasValue
                ? row.LaBanThanhPham.Value ? 1 : 0
                : null,
            NoiGiaoHang = row.NoiGiaoHang,
            Loi = row.Loi.ToArray()
        }).ToList();

        var soDongLoi = danhSach.Count(item => !item.HopLe);
        return new SanPhamImportPreviewDto
        {
            TongSoDong = danhSach.Count,
            SoDongHopLe = danhSach.Count - soDongLoi,
            SoDongLoi = soDongLoi,
            DanhSach = danhSach
        };
    }

    private static XLWorkbook TaoWorkbook(Stream fileStream)
    {
        try
        {
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }

            return new XLWorkbook(fileStream);
        }
        catch (Exception exception) when (
            exception is not OperationCanceledException)
        {
            throw new InvalidOperationException(
                "Không thể đọc file Excel. File có thể bị hỏng hoặc không đúng định dạng .xlsx.",
                exception);
        }
    }

    private static void KiemTraFile(
        Stream fileStream,
        string fileName,
        long fileSize)
    {
        if (fileStream is null || !fileStream.CanRead || fileSize <= 0)
        {
            throw new InvalidOperationException("Vui lòng chọn file Excel cần import.");
        }

        if (fileSize > MaxFileSize)
        {
            throw new InvalidOperationException(
                "File import vượt quá dung lượng cho phép 20 MB.");
        }

        if (!string.Equals(
                Path.GetExtension(fileName),
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "File import phải có định dạng .xlsx.");
        }
    }

    private static void KiemTraHeader(IXLWorksheet worksheet)
    {
        var errors = new List<string>();
        for (var index = 0; index < SanPhamImportTemplateBuilder.Headers.Length; index++)
        {
            var actual = worksheet.Cell(HeaderRow, index + 1).GetString().Trim();
            var expected = SanPhamImportTemplateBuilder.Headers[index];
            if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(
                    $"Cột {index + 1} phải là '{expected}' nhưng file đang là '{actual}'.");
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Cấu trúc file import không đúng mẫu: " + string.Join(" ", errors));
        }
    }

    private static bool LaDongTrong(IXLWorksheet worksheet, int rowNumber)
    {
        for (var column = 1; column <= SanPhamImportTemplateBuilder.Headers.Length; column++)
        {
            if (!worksheet.Cell(rowNumber, column).IsEmpty())
            {
                return false;
            }
        }

        return true;
    }

    private static string LayChuoi(
        IXLWorksheet worksheet,
        int row,
        int column)
        => worksheet.Cell(row, column).GetFormattedString().Trim();

    private static decimal? DocSo(
        IXLCell cell,
        string tenCot,
        ICollection<string> errors)
    {
        if (cell.IsEmpty())
        {
            return null;
        }

        if (cell.TryGetValue<decimal>(out var numericValue))
        {
            return numericValue;
        }

        var raw = cell.GetFormattedString().Trim();
        if (decimal.TryParse(
                raw,
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("vi-VN"),
                out var viValue))
        {
            return viValue;
        }

        if (decimal.TryParse(
                raw,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var invariantValue))
        {
            return invariantValue;
        }

        errors.Add($"Cột {tenCot} phải là số, giá trị hiện tại là '{raw}'.");
        return null;
    }

    private static bool? DocBanThanhPham(
        IXLCell cell,
        ICollection<string> errors)
    {
        if (cell.IsEmpty())
        {
            return null;
        }

        var raw = cell.GetFormattedString().Trim();
        if (raw == "0")
        {
            return false;
        }

        if (raw == "1")
        {
            return true;
        }

        errors.Add($"Cột Bán TP chỉ nhận 0 hoặc 1, giá trị hiện tại là '{raw}'.");
        return null;
    }

    private static void KiemTraBatBuoc(
        string? value,
        string tenTruong,
        int dong,
        ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"Dòng {dong} không có dữ liệu {tenTruong}.");
        }
    }

    private static void KiemTraSoBatBuoc(
        decimal? value,
        string tenTruong,
        int dong,
        ICollection<string> errors)
    {
        if (value.HasValue)
        {
            return;
        }

        var tenCotTrongLoi = tenTruong switch
        {
            "L" => "L (cm)",
            "W" => "W (cm)",
            "H" => "H (cm)",
            "Độ khó" => "Độ Khó (%)",
            "Hệ số tỉ trọng" => "Hệ số tỉ trọng riêng",
            _ => tenTruong
        };

        var daCoLoiSaiDinhDang = errors.Any(item => item.StartsWith(
            $"Cột {tenCotTrongLoi} phải là số",
            StringComparison.OrdinalIgnoreCase));

        if (!daCoLoiSaiDinhDang)
        {
            errors.Add($"Dòng {dong} không có dữ liệu {tenTruong}.");
        }
    }

    private static void KiemTraDoDai(
        string? value,
        int maxLength,
        string tenCot,
        ICollection<string> errors)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
        {
            errors.Add($"Cột {tenCot} không được vượt quá {maxLength} ký tự.");
        }
    }

    private static bool GiongNhau(string? left, string? right)
        => !string.IsNullOrWhiteSpace(left) &&
           !string.IsNullOrWhiteSpace(right) &&
           string.Equals(left, right, StringComparison.OrdinalIgnoreCase);

    private static string? ChuanHoaTuyChon(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    private static string? ChuanHoaMa(string? value)
    {
        var normalized = ChuanHoaTuyChon(value);
        return normalized?.ToUpperInvariant();
    }

    private static decimal? TinhCbm(
        decimal? chieuDaiCm,
        decimal? chieuRongCm,
        decimal? chieuCaoCm)
    {
        if (!chieuDaiCm.HasValue ||
            !chieuRongCm.HasValue ||
            !chieuCaoCm.HasValue)
        {
            return null;
        }

        return Math.Round(
            chieuDaiCm.Value * chieuRongCm.Value * chieuCaoCm.Value / 1_000_000m,
            6,
            MidpointRounding.AwayFromZero);
    }

    private sealed class ImportRow
    {
        public int Dong { get; init; }
        public string? MaSanPham { get; init; }
        public string? MoTaTiengViet { get; init; }
        public string? MoTaTiengAnh { get; init; }
        public string? MaDonViTinh { get; init; }
        public string? MaNhomNangLuc { get; init; }
        public decimal? ChieuDaiCm { get; set; }
        public decimal? ChieuRongCm { get; set; }
        public decimal? ChieuCaoCm { get; set; }
        public decimal? TrongLuong { get; set; }
        public decimal? DienTich { get; set; }
        public decimal? DoKho { get; set; }
        public decimal? HeSoTiTrong { get; set; }
        public string? MaKhoMacDinh { get; init; }
        public string? MaKhoTon { get; init; }
        public string? MaXuongMacDinh { get; init; }
        public string? MaThue { get; init; }
        public bool? LaBanThanhPham { get; set; }
        public string? NoiGiaoHang { get; init; }
        public string? GhiChu { get; init; }
        public Guid? DonViTinhId { get; set; }
        public Guid? NhomNangLucId { get; set; }
        public Guid? KhoMacDinhId { get; set; }
        public Guid? KhoTonId { get; set; }
        public Guid? XuongMacDinhId { get; set; }
        public Guid? ThueId { get; set; }
        public List<string> Loi { get; } = new();
    }

    private sealed record ImportValidationResult(IReadOnlyList<ImportRow> Rows);
}
