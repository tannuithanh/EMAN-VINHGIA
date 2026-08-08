using ClosedXML.Excel;
using Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;
using Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;
using Eman.Domain.Modules.Engineering.Bom.VatTu.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace Eman.Infrastructure.Services.Engineering.Bom.VatTu.Imports;

/// <summary>
/// Đọc, preview và import B.O.M vật tư từ Excel.
/// Mỗi B.O.M hợp lệ được tạo thành một phiên bản Nháp mới; một B.O.M có lỗi sẽ bị bỏ qua toàn bộ.
/// </summary>
internal sealed class BomVatTuImportService(
    EmanDbContext dbContext,
    BomVatTuImportTemplateBuilder templateBuilder) : IBomVatTuImportService
{
    private const string SheetName = "Import B.O.M vật tư";
    private const int HeaderRow = 1;
    private const int DataStartRow = 2;
    private const long MaxFileSize = 20 * 1024 * 1024;

    public Task<BomVatTuImportFileDto> TaoTemplateAsync(CancellationToken cancellationToken = default)
        => templateBuilder.BuildAsync(cancellationToken);

    public async Task<BomVatTuImportPreviewDto> XemTruocAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        CancellationToken cancellationToken = default)
    {
        var result = await DocVaKiemTraAsync(fileStream, fileName, fileSize, cancellationToken);
        return TaoPreview(result.Groups, result.Rows);
    }

    public async Task<BomVatTuImportResultDto> ImportAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        string? createdByMsnv,
        CancellationToken cancellationToken = default)
    {
        var result = await DocVaKiemTraAsync(fileStream, fileName, fileSize, cancellationToken);
        var preview = TaoPreview(result.Groups, result.Rows);
        if (!preview.CoTheImport)
        {
            return new BomVatTuImportResultDto
            {
                ThanhCong = false,
                Message = preview.TongSoDong == 0
                    ? "File import không có dòng dữ liệu B.O.M vật tư."
                    : $"Không có B.O.M hợp lệ để import. File có {preview.SoBomLoi} B.O.M lỗi.",
                TongSoBom = preview.TongSoBom,
                SoBomDaImport = 0,
                SoBomBoQua = preview.SoBomLoi,
                TongSoDong = preview.TongSoDong,
                SoDongDaImport = 0,
                SoDongBoQua = preview.TongSoDong,
                XemTruoc = preview
            };
        }

        var nguoiTao = ChuanHoaTuyChon(createdByMsnv);
        if (nguoiTao?.Length > 50)
        {
            return new BomVatTuImportResultDto
            {
                ThanhCong = false,
                Message = "Mã nhân viên người import không được vượt quá 50 ký tự.",
                TongSoBom = preview.TongSoBom,
                SoBomDaImport = 0,
                SoBomBoQua = preview.TongSoBom,
                TongSoDong = preview.TongSoDong,
                SoDongDaImport = 0,
                SoDongBoQua = preview.TongSoDong,
                XemTruoc = preview
            };
        }

        var groupsHopLe = result.Groups.Where(group => group.CoTheImport).ToList();
        var vatTuIds = groupsHopLe.Select(group => group.VatTuDauRaId!.Value).Distinct().ToList();
        var thoiDiem = DateTime.UtcNow;
        var entities = new List<BomVatTuPhienBan>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        try
        {
            var phienBanHienTai = await dbContext.BomVatTuPhienBans
                .Where(item => vatTuIds.Contains(item.VatTuId))
                .GroupBy(item => item.VatTuId)
                .Select(group => new
                {
                    VatTuId = group.Key,
                    MaxVersion = group.Max(item => item.SoPhienBan)
                })
                .ToDictionaryAsync(item => item.VatTuId, item => item.MaxVersion, cancellationToken);

            foreach (var group in groupsHopLe.OrderBy(item => item.MaVatTuDauRa))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var vatTuDauRaId = group.VatTuDauRaId!.Value;
                var soPhienBan = phienBanHienTai.TryGetValue(vatTuDauRaId, out var maxVersion)
                    ? maxVersion + 1
                    : 1;
                phienBanHienTai[vatTuDauRaId] = soPhienBan;

                var phienBan = new BomVatTuPhienBan
                {
                    Id = Guid.NewGuid(),
                    VatTuId = vatTuDauRaId,
                    SoPhienBan = soPhienBan,
                    TrangThai = TrangThaiBomVatTuPhienBan.Nhap,
                    CreatedAt = thoiDiem,
                    CreatedByMsnv = nguoiTao
                };

                var thuTu = 1;
                foreach (var row in group.Rows.OrderBy(item => item.RowNumber))
                {
                    phienBan.ChiTiets.Add(new BomVatTuChiTiet
                    {
                        Id = Guid.NewGuid(),
                        BomVatTuPhienBanId = phienBan.Id,
                        VatTuThanhPhanId = row.VatTuThanhPhanId!.Value,
                        SoLuong = row.SoLuong!.Value,
                        ThuTu = thuTu++,
                        GhiChu = row.GhiChu,
                        CreatedAt = thoiDiem,
                        CreatedByMsnv = nguoiTao
                    });
                }

                entities.Add(phienBan);
            }

            await dbContext.BomVatTuPhienBans.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var soDongDaImport = entities.Sum(item => item.ChiTiets.Count);
        var soBomBoQua = preview.TongSoBom - entities.Count;
        var soDongBoQua = preview.TongSoDong - soDongDaImport;
        var message = soBomBoQua > 0
            ? $"Import thành công {entities.Count} B.O.M ở trạng thái Nháp; đã bỏ qua {soBomBoQua} B.O.M có lỗi."
            : $"Import thành công {entities.Count} B.O.M vật tư ở trạng thái Nháp.";

        return new BomVatTuImportResultDto
        {
            ThanhCong = true,
            Message = message,
            TongSoBom = preview.TongSoBom,
            SoBomDaImport = entities.Count,
            SoBomBoQua = soBomBoQua,
            TongSoDong = preview.TongSoDong,
            SoDongDaImport = soDongDaImport,
            SoDongBoQua = soDongBoQua,
            XemTruoc = preview
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
        KiemTraHeader(worksheet, BomVatTuImportTemplateBuilder.Headers, SheetName);

        var lastRow = worksheet.LastRowUsed(XLCellsUsedOptions.Contents)?.RowNumber() ?? HeaderRow;
        if (lastRow < DataStartRow)
        {
            return new ImportValidationResult([], []);
        }

        var rows = new List<ImportRow>();
        for (var rowNumber = DataStartRow; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (LaDongTrong(worksheet, rowNumber, BomVatTuImportTemplateBuilder.Headers.Length))
            {
                continue;
            }
            rows.Add(DocDong(worksheet, rowNumber));
        }

        var groups = rows
            .GroupBy(row => string.IsNullOrWhiteSpace(row.MaVatTuDauRa)
                ? $"__DONG_{row.RowNumber}"
                : row.MaVatTuDauRa!, StringComparer.OrdinalIgnoreCase)
            .Select(group => new ImportGroup(group.Key, group.ToList()))
            .ToList();

        await KiemTraNghiepVuAsync(rows, groups, cancellationToken);
        return new ImportValidationResult(groups, rows);
    }

    private async Task KiemTraNghiepVuAsync(
        List<ImportRow> rows,
        List<ImportGroup> groups,
        CancellationToken cancellationToken)
    {
        foreach (var row in rows)
        {
            KiemTraDongCoBan(row);
        }

        var maCanTra = rows
            .SelectMany(row => new[] { row.MaVatTuDauRa, row.MaVatTuThanhPhan })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var vatTus = await dbContext.VatTus
            .AsNoTracking()
            .Include(item => item.DonViTinh)
            .Where(item => maCanTra.Contains(item.MaVatTu))
            .ToListAsync(cancellationToken);
        var vatTuTheoMa = vatTus.ToDictionary(item => item.MaVatTu, StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row.MaVatTuDauRa))
            {
                if (!vatTuTheoMa.TryGetValue(row.MaVatTuDauRa, out var dauRa))
                {
                    row.Loi.Add($"Mã vật tư đầu ra '{row.MaVatTuDauRa}' không tồn tại trong danh mục vật tư.");
                }
                else
                {
                    row.VatTuDauRaId = dauRa.Id;
                    row.TenVatTuDauRa = dauRa.TenVatTu;
                    row.MaDonViTinhDauRa = dauRa.DonViTinh.MaDonViTinh;
                    if (dauRa.TrangThai != TrangThaiHoatDong.HoatDong)
                    {
                        row.Loi.Add($"Vật tư đầu ra '{row.MaVatTuDauRa}' đang ngừng hoạt động.");
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(row.MaVatTuThanhPhan))
            {
                if (!vatTuTheoMa.TryGetValue(row.MaVatTuThanhPhan, out var thanhPhan))
                {
                    row.Loi.Add($"Mã vật tư thành phần '{row.MaVatTuThanhPhan}' không tồn tại trong danh mục vật tư.");
                }
                else
                {
                    row.VatTuThanhPhanId = thanhPhan.Id;
                    row.TenVatTuThanhPhan = thanhPhan.TenVatTu;
                    row.MaDonViTinhThanhPhan = thanhPhan.DonViTinh.MaDonViTinh;
                    if (thanhPhan.TrangThai != TrangThaiHoatDong.HoatDong)
                    {
                        row.Loi.Add($"Vật tư thành phần '{row.MaVatTuThanhPhan}' đang ngừng hoạt động.");
                    }
                }
            }

            if (row.VatTuDauRaId.HasValue
                && row.VatTuThanhPhanId.HasValue
                && row.VatTuDauRaId.Value == row.VatTuThanhPhanId.Value)
            {
                row.Loi.Add("Vật tư đầu ra không thể đồng thời là vật tư thành phần của chính B.O.M đó.");
            }
        }

        var vatTuDauRaIds = rows
            .Where(row => row.VatTuDauRaId.HasValue)
            .Select(row => row.VatTuDauRaId!.Value)
            .Distinct()
            .ToList();

        var maxVersions = await dbContext.BomVatTuPhienBans
            .AsNoTracking()
            .Where(item => vatTuDauRaIds.Contains(item.VatTuId))
            .GroupBy(item => item.VatTuId)
            .Select(group => new { VatTuId = group.Key, MaxVersion = group.Max(item => item.SoPhienBan) })
            .ToDictionaryAsync(item => item.VatTuId, item => item.MaxVersion, cancellationToken);

        foreach (var group in groups)
        {
            KiemTraNhomBom(group, maxVersions);
        }

        await KiemTraVongLapAsync(groups, cancellationToken);
    }

    private static void KiemTraDongCoBan(ImportRow row)
    {
        KiemTraBatBuoc(row, row.MaVatTuDauRa, "Mã vật tư đầu ra");
        KiemTraBatBuoc(row, row.MaVatTuThanhPhan, "Mã vật tư thành phần");
        KiemTraBatBuoc(row, row.SoLuongRaw, "Số lượng");

        KiemTraDoDai(row, row.MaVatTuDauRa, "Mã vật tư đầu ra", 100);
        KiemTraDoDai(row, row.MaVatTuThanhPhan, "Mã vật tư thành phần", 100);
        KiemTraDoDai(row, row.GhiChu, "Ghi chú", 500);

        if (!string.IsNullOrWhiteSpace(row.SoLuongRaw))
        {
            if (!ThuParseDecimal(row.SoLuongRaw, out var value) || value <= 0)
            {
                row.Loi.Add("Số lượng vật tư thành phần phải là số lớn hơn 0.");
            }
            else
            {
                row.SoLuong = value;
            }
        }
    }

    private static void KiemTraNhomBom(
        ImportGroup group,
        IReadOnlyDictionary<Guid, int> maxVersions)
    {
        if (group.MaVatTuDauRa.StartsWith("__DONG_", StringComparison.Ordinal))
        {
            group.Loi.Add("Không xác định được mã vật tư đầu ra của B.O.M.");
            group.DanhDauKhongTheImport();
            return;
        }

        var firstWithOutput = group.Rows.FirstOrDefault(row => row.VatTuDauRaId.HasValue);
        if (firstWithOutput is not null)
        {
            group.VatTuDauRaId = firstWithOutput.VatTuDauRaId;
            group.TenVatTuDauRa = firstWithOutput.TenVatTuDauRa;
            group.MaDonViTinhDauRa = firstWithOutput.MaDonViTinhDauRa;
            group.SoPhienBanDuKien = maxVersions.TryGetValue(firstWithOutput.VatTuDauRaId!.Value, out var maxVersion)
                ? maxVersion + 1
                : 1;
        }

        foreach (var duplicate in group.Rows
                     .Where(row => !string.IsNullOrWhiteSpace(row.MaVatTuThanhPhan))
                     .GroupBy(row => row.MaVatTuThanhPhan!, StringComparer.OrdinalIgnoreCase)
                     .Where(items => items.Count() > 1))
        {
            var message = $"Vật tư thành phần '{duplicate.Key}' bị trùng trong B.O.M của '{group.MaVatTuDauRa}'.";
            group.Loi.Add(message);
            foreach (var row in duplicate)
            {
                row.Loi.Add(message);
            }
        }

        if (group.Rows.Any(row => row.Loi.Count > 0))
        {
            group.Loi.Add("B.O.M có ít nhất một dòng lỗi nên toàn bộ B.O.M này sẽ không được import.");
        }

        group.CapNhatTrangThaiImport();
    }

    private async Task KiemTraVongLapAsync(
        IReadOnlyList<ImportGroup> groups,
        CancellationToken cancellationToken)
    {
        var groupsCoDuLieu = groups
            .Where(group => group.VatTuDauRaId.HasValue)
            .ToList();
        if (groupsCoDuLieu.Count == 0) return;

        var outputIds = groupsCoDuLieu.Select(group => group.VatTuDauRaId!.Value).ToHashSet();
        var quanHeHieuLuc = await dbContext.BomVatTuChiTiets
            .AsNoTracking()
            .Where(item => item.BomVatTuPhienBan.TrangThai == TrangThaiBomVatTuPhienBan.HieuLuc
                && !outputIds.Contains(item.BomVatTuPhienBan.VatTuId))
            .Select(item => new
            {
                VatTuDauRaId = item.BomVatTuPhienBan.VatTuId,
                item.VatTuThanhPhanId
            })
            .ToListAsync(cancellationToken);

        var doThi = new Dictionary<Guid, List<Guid>>();
        foreach (var relation in quanHeHieuLuc)
        {
            ThemCanh(doThi, relation.VatTuDauRaId, relation.VatTuThanhPhanId);
        }

        foreach (var group in groupsCoDuLieu)
        {
            foreach (var row in group.Rows.Where(row => row.VatTuThanhPhanId.HasValue))
            {
                ThemCanh(doThi, group.VatTuDauRaId!.Value, row.VatTuThanhPhanId!.Value);
            }
        }

        foreach (var group in groupsCoDuLieu)
        {
            if (group.Rows.Any(row => row.Loi.Count > 0)) continue;
            var dauRaId = group.VatTuDauRaId!.Value;
            var rowGayVongLap = group.Rows.FirstOrDefault(row =>
                row.VatTuThanhPhanId.HasValue
                && CoDuongDi(row.VatTuThanhPhanId.Value, dauRaId, doThi));
            if (rowGayVongLap is null) continue;

            var message = $"B.O.M '{group.MaVatTuDauRa}' tạo thành vòng lặp nhiều cấp qua vật tư '{rowGayVongLap.MaVatTuThanhPhan}'.";
            group.Loi.Add(message);
            rowGayVongLap.Loi.Add(message);
            group.DanhDauKhongTheImport();
        }
    }

    private static void ThemCanh(Dictionary<Guid, List<Guid>> doThi, Guid dauRaId, Guid thanhPhanId)
    {
        if (!doThi.TryGetValue(dauRaId, out var list))
        {
            list = [];
            doThi.Add(dauRaId, list);
        }
        if (!list.Contains(thanhPhanId)) list.Add(thanhPhanId);
    }

    private static bool CoDuongDi(Guid batDau, Guid dich, IReadOnlyDictionary<Guid, List<Guid>> doThi)
    {
        var daDuyet = new HashSet<Guid>();
        var nganXep = new Stack<Guid>();
        nganXep.Push(batDau);
        while (nganXep.Count > 0)
        {
            var hienTai = nganXep.Pop();
            if (hienTai == dich) return true;
            if (!daDuyet.Add(hienTai)) continue;
            if (!doThi.TryGetValue(hienTai, out var keTiep)) continue;
            foreach (var item in keTiep) nganXep.Push(item);
        }
        return false;
    }

    private static BomVatTuImportPreviewDto TaoPreview(
        IReadOnlyList<ImportGroup> groups,
        IReadOnlyList<ImportRow> rows)
    {
        var danhSachDong = rows.Select(row => new BomVatTuImportRowPreviewDto
        {
            Dong = row.RowNumber,
            MaVatTuDauRa = row.MaVatTuDauRa,
            TenVatTuDauRa = row.TenVatTuDauRa,
            MaDonViTinhDauRa = row.MaDonViTinhDauRa,
            MaVatTuThanhPhan = row.MaVatTuThanhPhan,
            TenVatTuThanhPhan = row.TenVatTuThanhPhan,
            SoLuong = row.SoLuong,
            MaDonViTinhThanhPhan = row.MaDonViTinhThanhPhan,
            GhiChu = row.GhiChu,
            Loi = row.Loi.Distinct().ToList()
        }).ToList();

        var danhSachBom = groups.Select(group => new BomVatTuImportGroupPreviewDto
        {
            MaVatTuDauRa = group.MaVatTuDauRa.StartsWith("__DONG_", StringComparison.Ordinal)
                ? string.Empty
                : group.MaVatTuDauRa,
            TenVatTuDauRa = group.TenVatTuDauRa,
            MaDonViTinhDauRa = group.MaDonViTinhDauRa,
            SoPhienBanDuKien = group.SoPhienBanDuKien,
            TongSoThanhPhan = group.Rows.Count,
            SoDongLoi = group.Rows.Count(row => row.Loi.Count > 0),
            CoTheImport = group.CoTheImport,
            Loi = group.Loi.Distinct().ToList()
        }).ToList();

        return new BomVatTuImportPreviewDto
        {
            TongSoDong = danhSachDong.Count,
            SoDongHopLe = danhSachDong.Count(item => item.HopLe),
            SoDongLoi = danhSachDong.Count(item => !item.HopLe),
            TongSoBom = danhSachBom.Count,
            SoBomCoTheImport = danhSachBom.Count(item => item.CoTheImport),
            SoBomLoi = danhSachBom.Count(item => !item.CoTheImport),
            DanhSachBom = danhSachBom,
            DanhSach = danhSachDong
        };
    }

    private static ImportRow DocDong(IXLWorksheet worksheet, int rowNumber)
        => new(rowNumber)
        {
            MaVatTuDauRa = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 1))),
            MaVatTuThanhPhan = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 2))),
            SoLuongRaw = ChuanHoaTuyChon(LaySo(worksheet.Cell(rowNumber, 3))),
            GhiChu = ChuanHoaTuyChon(LayChuoi(worksheet.Cell(rowNumber, 4)))
        };

    private static void KiemTraFile(Stream fileStream, string fileName, long fileSize)
    {
        if (fileStream is null || !fileStream.CanRead)
            throw new InvalidOperationException("Không thể đọc file import B.O.M vật tư.");
        if (fileSize <= 0)
            throw new InvalidOperationException("File import B.O.M vật tư đang trống.");
        if (fileSize > MaxFileSize)
            throw new InvalidOperationException("Dung lượng file import B.O.M vật tư không được vượt quá 20 MB.");
        if (!string.Equals(Path.GetExtension(fileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("File import B.O.M vật tư phải có định dạng .xlsx.");
    }

    private static XLWorkbook TaoWorkbook(Stream fileStream)
    {
        try
        {
            return new XLWorkbook(fileStream);
        }
        catch (Exception exception) when (exception is not InvalidOperationException)
        {
            throw new InvalidOperationException(
                "Không thể mở file Excel. Vui lòng kiểm tra file có đúng định dạng .xlsx và không bị hỏng.",
                exception);
        }
    }

    private static void KiemTraHeader(IXLWorksheet worksheet, IReadOnlyList<string> headers, string sheetName)
    {
        for (var index = 0; index < headers.Count; index++)
        {
            var actual = worksheet.Cell(HeaderRow, index + 1).GetString().Trim();
            if (!string.Equals(actual, headers[index], StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Cột {index + 1} trong sheet '{sheetName}' phải là '{headers[index]}'. Vui lòng dùng đúng file mẫu từ hệ thống.");
            }
        }
    }

    private static bool LaDongTrong(IXLWorksheet worksheet, int rowNumber, int columnCount)
    {
        for (var column = 1; column <= columnCount; column++)
        {
            if (!worksheet.Cell(rowNumber, column).IsEmpty()) return false;
        }
        return true;
    }

    private static string? LayChuoi(IXLCell cell)
        => cell.IsEmpty() ? null : cell.GetFormattedString().Trim();

    private static string? LaySo(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;
        if (cell.DataType == XLDataType.Number && cell.TryGetValue<decimal>(out var number))
            return number.ToString(CultureInfo.InvariantCulture);
        return cell.GetFormattedString().Trim();
    }

    private static bool ThuParseDecimal(string raw, out decimal value)
    {
        if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out value)) return true;
        return decimal.TryParse(raw, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out value);
    }

    private static void KiemTraBatBuoc(ImportRow row, string? value, string tenTruong)
    {
        if (string.IsNullOrWhiteSpace(value)) row.Loi.Add($"{tenTruong} là bắt buộc.");
    }

    private static void KiemTraDoDai(ImportRow row, string? value, string tenTruong, int maxLength)
    {
        if (value is { Length: > 0 } && value.Length > maxLength)
            row.Loi.Add($"{tenTruong} không được vượt quá {maxLength} ký tự.");
    }

    private static string? ChuanHoaMa(string? value)
    {
        var result = ChuanHoaTuyChon(value);
        return result?.ToUpperInvariant();
    }

    private static string? ChuanHoaTuyChon(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record ImportValidationResult(
        IReadOnlyList<ImportGroup> Groups,
        IReadOnlyList<ImportRow> Rows);

    private sealed class ImportGroup(string maVatTuDauRa, List<ImportRow> rows)
    {
        public string MaVatTuDauRa { get; } = maVatTuDauRa;
        public List<ImportRow> Rows { get; } = rows;
        public Guid? VatTuDauRaId { get; set; }
        public string? TenVatTuDauRa { get; set; }
        public string? MaDonViTinhDauRa { get; set; }
        public int SoPhienBanDuKien { get; set; }
        public List<string> Loi { get; } = [];
        public bool CoTheImport { get; private set; }

        public void CapNhatTrangThaiImport()
            => CoTheImport = Rows.Count > 0
                && VatTuDauRaId.HasValue
                && Loi.Count == 0
                && Rows.All(row => row.Loi.Count == 0 && row.VatTuThanhPhanId.HasValue && row.SoLuong.HasValue);

        public void DanhDauKhongTheImport() => CoTheImport = false;
    }

    private sealed class ImportRow(int rowNumber)
    {
        public int RowNumber { get; } = rowNumber;
        public string? MaVatTuDauRa { get; set; }
        public string? MaVatTuThanhPhan { get; set; }
        public string? SoLuongRaw { get; set; }
        public string? GhiChu { get; set; }
        public decimal? SoLuong { get; set; }
        public Guid? VatTuDauRaId { get; set; }
        public string? TenVatTuDauRa { get; set; }
        public string? MaDonViTinhDauRa { get; set; }
        public Guid? VatTuThanhPhanId { get; set; }
        public string? TenVatTuThanhPhan { get; set; }
        public string? MaDonViTinhThanhPhan { get; set; }
        public List<string> Loi { get; } = [];
    }
}
