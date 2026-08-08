using ClosedXML.Excel;
using Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;
using Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.Materials.Entities;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Infrastructure.Services.MasterData.Materials.VatTu.Imports;

/// <summary>
/// Đọc, kiểm tra và ghi dữ liệu vật tư từ file Excel vào md_vat_tu.
/// </summary>
internal sealed class VatTuImportService(
    EmanDbContext dbContext,
    VatTuImportTemplateBuilder templateBuilder) : IVatTuImportService
{
    private const string SheetName = "Import vật tư";
    private const string WorkshopSheetName = "Phân xưởng sử dụng";
    private const int HeaderRow = 1;
    private const int DataStartRow = 2;
    private const long MaxFileSize = 20 * 1024 * 1024;

    public Task<VatTuImportFileDto> TaoTemplateAsync(CancellationToken cancellationToken = default)
        => templateBuilder.BuildAsync(cancellationToken);

    public async Task<VatTuImportPreviewDto> XemTruocAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        CancellationToken cancellationToken = default)
    {
        var result = await DocVaKiemTraAsync(fileStream, fileName, fileSize, cancellationToken);
        return TaoPreview(result.Rows);
    }

    public async Task<VatTuImportResultDto> ImportAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        string? createdByMsnv,
        CancellationToken cancellationToken = default)
    {
        var result = await DocVaKiemTraAsync(fileStream, fileName, fileSize, cancellationToken);
        var preview = TaoPreview(result.Rows);
        if (!preview.CoTheImport)
        {
            return new VatTuImportResultDto
            {
                ThanhCong = false,
                Message = preview.TongSoDong == 0
                    ? "File import không có dòng dữ liệu vật tư."
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
            return new VatTuImportResultDto
            {
                ThanhCong = false,
                Message = "Mã nhân viên người import không được vượt quá 50 ký tự.",
                TongSoDong = preview.TongSoDong,
                SoDongDaImport = 0,
                SoDongBoQua = preview.SoDongLoi,
                XemTruoc = preview
            };
        }

        var thoiDiem = DateTime.UtcNow;
        var entities = result.Rows
            .Where(row => row.Loi.Count == 0)
            .Select(row => TaoEntity(row, nguoiTao, thoiDiem))
            .ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await dbContext.VatTus.AddRangeAsync(entities, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var message = preview.SoDongLoi > 0
            ? $"Import thành công {entities.Count} vật tư; đã bỏ qua {preview.SoDongLoi} dòng lỗi."
            : $"Import thành công {entities.Count} vật tư vào danh mục vật tư EMAN.";

        return new VatTuImportResultDto
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
        KiemTraHeader(worksheet, VatTuImportTemplateBuilder.Headers, SheetName);

        var workshopSheet = workbook.Worksheets.FirstOrDefault(item =>
            string.Equals(item.Name, WorkshopSheetName, StringComparison.OrdinalIgnoreCase));
        if (workshopSheet is not null)
        {
            KiemTraHeader(
                workshopSheet,
                VatTuImportTemplateBuilder.WorkshopHeaders,
                WorkshopSheetName);
        }

        var lastRow = worksheet.LastRowUsed(XLCellsUsedOptions.Contents)?.RowNumber() ?? HeaderRow;
        if (lastRow < DataStartRow)
        {
            return new ImportValidationResult(Array.Empty<ImportRow>());
        }

        var rows = new List<ImportRow>();
        for (var rowNumber = DataStartRow; rowNumber <= lastRow; rowNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (LaDongTrong(worksheet, rowNumber, VatTuImportTemplateBuilder.Headers.Length))
            {
                continue;
            }
            rows.Add(DocDong(worksheet, rowNumber));
        }

        var workshopMappings = workshopSheet is null
            ? new Dictionary<string, List<WorkshopMapping>>(StringComparer.OrdinalIgnoreCase)
            : DocPhanXuong(workshopSheet);

        var maTrongFile = rows
            .Where(row => !string.IsNullOrWhiteSpace(row.MaVatTu))
            .Select(row => row.MaVatTu!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var maKhongTonTai = workshopMappings.Keys.FirstOrDefault(code => !maTrongFile.Contains(code));
        if (maKhongTonTai is not null)
        {
            throw new InvalidOperationException(
                $"Sheet '{WorkshopSheetName}' có mã vật tư '{maKhongTonTai}' không tồn tại trong sheet '{SheetName}'.");
        }

        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row.MaVatTu)
                && workshopMappings.TryGetValue(row.MaVatTu, out var mappings))
            {
                row.WorkshopMappings.AddRange(mappings);
            }
        }

        await KiemTraNghiepVuAsync(rows, cancellationToken);
        return new ImportValidationResult(rows);
    }

    private async Task KiemTraNghiepVuAsync(
        List<ImportRow> rows,
        CancellationToken cancellationToken)
    {
        var donViTinhs = (await dbContext.DonViTinhs.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(item => item.MaDonViTinh, StringComparer.OrdinalIgnoreCase);
        var nhomVatTus = (await dbContext.NhomVatTus.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(item => item.MaNhomVatTu, StringComparer.OrdinalIgnoreCase);
        var coSoMuas = (await dbContext.CoSoMuaVatTus.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(item => item.MaCoSoMuaVatTu, StringComparer.OrdinalIgnoreCase);
        var nhaCungCaps = (await dbContext.DoiTacKinhDoanhs.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(item => item.MaDoiTac, StringComparer.OrdinalIgnoreCase);
        var thues = (await dbContext.ThueSanPhams.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(item => item.MaThue, StringComparer.OrdinalIgnoreCase);
        var khos = (await dbContext.Khos.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(item => item.MaKho, StringComparer.OrdinalIgnoreCase);
        var phanXuongs = (await dbContext.PhanXuongs.AsNoTracking().ToListAsync(cancellationToken))
            .ToDictionary(item => item.MaPhanXuong, StringComparer.OrdinalIgnoreCase);
        var maDaCo = (await dbContext.VatTus.AsNoTracking()
                .Select(item => item.MaVatTu).ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            KiemTraBatBuocVaDoDai(row);
            KiemTraSo(row);
            KiemTraDonViTinh(row, donViTinhs);
            KiemTraNhomVatTu(row, nhomVatTus);
            KiemTraKho(row, khos);
            KiemTraPhuongThucVaThongTinMua(row, coSoMuas, nhaCungCaps, thues);
            KiemTraPhamViVaPhanXuong(row, phanXuongs);

            if (!string.IsNullOrWhiteSpace(row.MaVatTu) && maDaCo.Contains(row.MaVatTu))
            {
                row.Loi.Add($"Mã vật tư '{row.MaVatTu}' đã tồn tại trong EMAN.");
            }
        }

        foreach (var group in rows
                     .Where(row => !string.IsNullOrWhiteSpace(row.MaVatTu))
                     .GroupBy(row => row.MaVatTu!, StringComparer.OrdinalIgnoreCase)
                     .Where(group => group.Count() > 1))
        {
            foreach (var row in group)
            {
                row.Loi.Add($"Mã vật tư '{group.Key}' bị trùng trong file import.");
            }
        }
    }

    private static void KiemTraBatBuocVaDoDai(ImportRow row)
    {
        KiemTraBatBuoc(row, row.MaVatTu, "Mã vật tư");
        KiemTraBatBuoc(row, row.TenVatTu, "Tên vật tư");
        KiemTraBatBuoc(row, row.MaDonViTinh, "ĐVT");
        KiemTraBatBuoc(row, row.MaNhomVatTu, "Nhóm vật tư");
        KiemTraBatBuoc(row, row.PhuongThucRaw, "Phương thức cung ứng");
        KiemTraBatBuoc(row, row.HanSuDungRaw, "Hạn sử dụng");

        KiemTraDoDai(row, row.MaVatTu, 100, "Mã vật tư");
        KiemTraDoDai(row, row.TenVatTu, 300, "Tên vật tư");
        KiemTraDoDai(row, row.TenTiengAnh, 300, "Tên tiếng Anh");
        KiemTraDoDai(row, row.MaDonViTinh, 50, "ĐVT");
        KiemTraDoDai(row, row.QuyCachDongGoi, 500, "Quy cách đóng gói");
        KiemTraDoDai(row, row.MaNhomVatTu, 50, "Nhóm vật tư");
        KiemTraDoDai(row, row.MucDichSuDung, 1000, "Mục đích sử dụng");
        KiemTraDoDai(row, row.MaCoSoMuaVatTu, 50, "Cơ sở mua");
        KiemTraDoDai(row, row.MaNhaCungCapMacDinh, 100, "NCC mặc định");
        KiemTraDoDai(row, row.MaThueVat, 50, "Thuế VAT");
        KiemTraDoDai(row, row.MaKhoLuuTru, 50, "Kho lưu trữ");
    }

    private static void KiemTraSo(ImportRow row)
    {
        row.PhamViSuDung = ParsePhamVi(row.PhamViRaw);
        if (!row.PhamViSuDung.HasValue && !string.IsNullOrWhiteSpace(row.PhamViRaw))
        {
            row.Loi.Add("Phạm vi sử dụng chỉ nhận 1 - Tất cả phân xưởng hoặc 2 - Phân xưởng cụ thể.");
        }

        row.PhuongThucCungUng = ParsePhuongThuc(row.PhuongThucRaw);
        if (!row.PhuongThucCungUng.HasValue && !string.IsNullOrWhiteSpace(row.PhuongThucRaw))
        {
            row.Loi.Add("Phương thức cung ứng chỉ nhận 1 - Chỉ mua ngoài, 2 - Mua hoặc tự sản xuất hoặc 3 - Chỉ tự sản xuất.");
        }

        if (!string.IsNullOrWhiteSpace(row.HanSuDungRaw))
        {
            if (!TryParseInt(row.HanSuDungRaw, out var hanSuDung) || hanSuDung < 0)
            {
                row.Loi.Add("Hạn sử dụng phải là số nguyên lớn hơn hoặc bằng 0 ngày.");
            }
            else
            {
                row.HanSuDungNgay = hanSuDung;
            }
        }

        if (!string.IsNullOrWhiteSpace(row.TonToiThieuRaw))
        {
            if (!TryParseDecimal(row.TonToiThieuRaw, out var tonToiThieu) || tonToiThieu < 0)
            {
                row.Loi.Add("Tồn tối thiểu phải là số lớn hơn hoặc bằng 0.");
            }
            else
            {
                row.TonToiThieu = tonToiThieu;
            }
        }

        if (!string.IsNullOrWhiteSpace(row.MoqRaw))
        {
            if (!TryParseDecimal(row.MoqRaw, out var moq) || moq <= 0)
            {
                row.Loi.Add("MOQ phải là số lớn hơn 0.");
            }
            else
            {
                row.Moq = moq;
            }
        }

        if (!string.IsNullOrWhiteSpace(row.NgayMuaHangRaw))
        {
            if (!TryParseInt(row.NgayMuaHangRaw, out var ngayMuaHang) || ngayMuaHang < 0)
            {
                row.Loi.Add("Thời gian mua hàng phải là số nguyên lớn hơn hoặc bằng 0 ngày.");
            }
            else
            {
                row.NgayMuaHang = ngayMuaHang;
            }
        }
    }

    private static void KiemTraDonViTinh(
        ImportRow row,
        IReadOnlyDictionary<string, Eman.Domain.Modules.MasterData.Common.Entities.DonViTinh> items)
    {
        if (string.IsNullOrWhiteSpace(row.MaDonViTinh)) return;
        if (!items.TryGetValue(row.MaDonViTinh, out var item))
        {
            row.Loi.Add($"Không tìm thấy đơn vị tính có mã '{row.MaDonViTinh}' trong EMAN.");
        }
        else if (item.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            row.Loi.Add($"Đơn vị tính mã '{row.MaDonViTinh}' đang ngừng hoạt động.");
        }
        else
        {
            row.DonViTinhId = item.Id;
        }
    }

    private static void KiemTraNhomVatTu(
        ImportRow row,
        IReadOnlyDictionary<string, NhomVatTu> items)
    {
        if (string.IsNullOrWhiteSpace(row.MaNhomVatTu)) return;
        if (!items.TryGetValue(row.MaNhomVatTu, out var item))
        {
            row.Loi.Add($"Không tìm thấy nhóm vật tư có mã '{row.MaNhomVatTu}' trong EMAN.");
        }
        else if (item.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            row.Loi.Add($"Nhóm vật tư mã '{row.MaNhomVatTu}' đang ngừng hoạt động.");
        }
        else
        {
            row.NhomVatTuId = item.Id;
        }
    }

    private static void KiemTraKho(
        ImportRow row,
        IReadOnlyDictionary<string, Eman.Domain.Modules.MasterData.Inventory.Entities.Kho> items)
    {
        if (string.IsNullOrWhiteSpace(row.MaKhoLuuTru)) return;
        if (!items.TryGetValue(row.MaKhoLuuTru, out var item))
        {
            row.Loi.Add($"Không tìm thấy kho lưu trữ có mã '{row.MaKhoLuuTru}' trong EMAN.");
        }
        else if (item.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            row.Loi.Add($"Kho lưu trữ mã '{row.MaKhoLuuTru}' đang ngừng hoạt động.");
        }
        else
        {
            row.KhoLuuTruId = item.Id;
        }
    }

    private static void KiemTraPhuongThucVaThongTinMua(
        ImportRow row,
        IReadOnlyDictionary<string, CoSoMuaVatTu> coSoMuas,
        IReadOnlyDictionary<string, Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DoiTacKinhDoanh> nhaCungCaps,
        IReadOnlyDictionary<string, Eman.Domain.Modules.MasterData.Products.Entities.ThueSanPham> thues)
    {
        if (row.PhuongThucCungUng == PhuongThucCungUngVatTu.ChiTuSanXuat)
        {
            row.MaCoSoMuaVatTu = null;
            row.MaNhaCungCapMacDinh = null;
            row.NgayMuaHang = null;
            row.Moq = null;
            row.MaThueVat = null;
            return;
        }

        if (row.PhuongThucCungUng is not (
            PhuongThucCungUngVatTu.ChiMuaNgoai or
            PhuongThucCungUngVatTu.MuaHoacTuSanXuat))
        {
            return;
        }

        KiemTraBatBuoc(row, row.MaCoSoMuaVatTu, "Cơ sở mua");
        KiemTraBatBuoc(row, row.NgayMuaHangRaw, "Thời gian mua hàng (ngày)");
        KiemTraBatBuoc(row, row.MaThueVat, "Thuế VAT");

        if (!string.IsNullOrWhiteSpace(row.MaCoSoMuaVatTu))
        {
            if (!coSoMuas.TryGetValue(row.MaCoSoMuaVatTu, out var item))
            {
                row.Loi.Add($"Không tìm thấy cơ sở mua có mã '{row.MaCoSoMuaVatTu}' trong EMAN.");
            }
            else if (item.TrangThai != TrangThaiHoatDong.HoatDong)
            {
                row.Loi.Add($"Cơ sở mua mã '{row.MaCoSoMuaVatTu}' đang ngừng hoạt động.");
            }
            else
            {
                row.CoSoMuaVatTuId = item.Id;
            }
        }

        if (!string.IsNullOrWhiteSpace(row.MaNhaCungCapMacDinh))
        {
            if (!nhaCungCaps.TryGetValue(row.MaNhaCungCapMacDinh, out var item))
            {
                row.Loi.Add($"Không tìm thấy nhà cung cấp mặc định có mã '{row.MaNhaCungCapMacDinh}' trong EMAN.");
            }
            else if (item.TrangThai != TrangThaiHoatDong.HoatDong)
            {
                row.Loi.Add($"Nhà cung cấp mặc định mã '{row.MaNhaCungCapMacDinh}' đang ngừng hoạt động.");
            }
            else if (!item.LaNhaCungCap)
            {
                row.Loi.Add($"Đối tác mã '{row.MaNhaCungCapMacDinh}' không phải là nhà cung cấp.");
            }
            else
            {
                row.NhaCungCapMacDinhId = item.Id;
            }
        }

        if (!string.IsNullOrWhiteSpace(row.MaThueVat))
        {
            if (!thues.TryGetValue(row.MaThueVat, out var item))
            {
                row.Loi.Add($"Không tìm thấy thuế VAT có mã '{row.MaThueVat}' trong EMAN.");
            }
            else if (item.TrangThai != TrangThaiHoatDong.HoatDong)
            {
                row.Loi.Add($"Thuế VAT mã '{row.MaThueVat}' đang ngừng hoạt động.");
            }
            else
            {
                row.ThueVatId = item.Id;
            }
        }
    }

    private static void KiemTraPhamViVaPhanXuong(
        ImportRow row,
        IReadOnlyDictionary<string, Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong> phanXuongs)
    {
        if (!row.PhamViSuDung.HasValue)
        {
            if (row.WorkshopMappings.Count > 0)
            {
                row.Loi.Add(
                    "Không được khai phân xưởng sử dụng khi chưa nhập Phạm vi sử dụng.");
            }
            return;
        }

        if (row.PhamViSuDung == PhamViSuDungVatTu.TatCaPhanXuong)
        {
            if (row.WorkshopMappings.Count > 0)
            {
                row.Loi.Add(
                    "Vật tư dùng cho tất cả phân xưởng không được khai phân xưởng cụ thể trong sheet Phân xưởng sử dụng.");
            }
            return;
        }

        if (row.PhamViSuDung != PhamViSuDungVatTu.PhanXuongCuThe)
        {
            return;
        }
        if (row.WorkshopMappings.Count == 0)
        {
            row.Loi.Add("Phạm vi sử dụng là Phân xưởng cụ thể nhưng chưa khai phân xưởng sử dụng.");
            return;
        }

        foreach (var duplicate in row.WorkshopMappings
                     .GroupBy(item => item.MaPhanXuong, StringComparer.OrdinalIgnoreCase)
                     .Where(group => group.Count() > 1))
        {
            row.Loi.Add($"Mã phân xưởng '{duplicate.Key}' bị khai trùng cho vật tư.");
        }

        foreach (var mapping in row.WorkshopMappings
                     .GroupBy(item => item.MaPhanXuong, StringComparer.OrdinalIgnoreCase)
                     .Select(group => group.First()))
        {
            if (!phanXuongs.TryGetValue(mapping.MaPhanXuong, out var item))
            {
                row.Loi.Add($"Không tìm thấy phân xưởng có mã '{mapping.MaPhanXuong}' trong EMAN.");
            }
            else if (item.TrangThai != TrangThaiHoatDong.HoatDong)
            {
                row.Loi.Add($"Phân xưởng mã '{mapping.MaPhanXuong}' đang ngừng hoạt động.");
            }
            else
            {
                row.PhanXuongIds.Add(item.Id);
            }
        }
    }

    private static VatTuEntity TaoEntity(ImportRow row, string? nguoiTao, DateTime thoiDiem)
    {
        var entity = new VatTuEntity
        {
            Id = Guid.NewGuid(),
            MaVatTu = row.MaVatTu!,
            TenVatTu = row.TenVatTu!,
            TenTiengAnh = row.TenTiengAnh,
            DonViTinhId = row.DonViTinhId!.Value,
            QuyCachDongGoi = row.QuyCachDongGoi,
            PhamViSuDung = row.PhamViSuDung,
            NhomVatTuId = row.NhomVatTuId!.Value,
            MucDichSuDung = row.MucDichSuDung,
            PhuongThucCungUng = row.PhuongThucCungUng!.Value,
            CoSoMuaVatTuId = row.CoSoMuaVatTuId,
            NhaCungCapMacDinhId = row.NhaCungCapMacDinhId,
            NgayMuaHang = row.NgayMuaHang,
            HanSuDungNgay = row.HanSuDungNgay!.Value,
            Moq = row.Moq,
            ThueVatId = row.ThueVatId,
            TonToiThieu = row.TonToiThieu,
            KhoLuuTruId = row.KhoLuuTruId,
            TrangThai = TrangThaiHoatDong.HoatDong,
            CreatedAt = thoiDiem,
            CreatedByMsnv = nguoiTao
        };
        foreach (var phanXuongId in row.PhanXuongIds.Distinct())
        {
            entity.PhanXuongs.Add(new VatTuPhanXuong
            {
                Id = Guid.NewGuid(),
                PhanXuongId = phanXuongId,
                CreatedAt = thoiDiem,
                CreatedByMsnv = nguoiTao
            });
        }
        return entity;
    }

    private static VatTuImportPreviewDto TaoPreview(IReadOnlyList<ImportRow> rows)
    {
        var danhSach = rows.Select(row => new VatTuImportRowPreviewDto
        {
            Dong = row.RowNumber,
            MaVatTu = row.MaVatTu,
            TenVatTu = row.TenVatTu,
            MaDonViTinh = row.MaDonViTinh,
            MaNhomVatTu = row.MaNhomVatTu,
            PhamViSuDung = row.PhamViSuDung.HasValue ? (byte)row.PhamViSuDung.Value : null,
            PhuongThucCungUng = row.PhuongThucCungUng.HasValue ? (byte)row.PhuongThucCungUng.Value : null,
            MaCoSoMuaVatTu = row.MaCoSoMuaVatTu,
            MaNhaCungCapMacDinh = row.MaNhaCungCapMacDinh,
            MaThueVat = row.MaThueVat,
            MaKhoLuuTru = row.MaKhoLuuTru,
            MaPhanXuongs = row.WorkshopMappings
                .Select(item => item.MaPhanXuong)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList(),
            Loi = row.Loi.ToList()
        }).ToList();
        return new VatTuImportPreviewDto
        {
            TongSoDong = danhSach.Count,
            SoDongHopLe = danhSach.Count(item => item.HopLe),
            SoDongLoi = danhSach.Count(item => !item.HopLe),
            DanhSach = danhSach
        };
    }

    private static ImportRow DocDong(IXLWorksheet worksheet, int rowNumber)
        => new(rowNumber)
        {
            MaVatTu = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 1))),
            TenVatTu = ChuanHoaTuyChon(LayChuoi(worksheet.Cell(rowNumber, 2))),
            TenTiengAnh = ChuanHoaTuyChon(LayChuoi(worksheet.Cell(rowNumber, 3))),
            MaDonViTinh = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 4))),
            QuyCachDongGoi = ChuanHoaTuyChon(LayChuoi(worksheet.Cell(rowNumber, 5))),
            PhamViRaw = ChuanHoaTuyChon(LayChuoi(worksheet.Cell(rowNumber, 6))),
            MaNhomVatTu = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 7))),
            MucDichSuDung = ChuanHoaTuyChon(LayChuoi(worksheet.Cell(rowNumber, 8))),
            PhuongThucRaw = ChuanHoaTuyChon(LayChuoi(worksheet.Cell(rowNumber, 9))),
            MaCoSoMuaVatTu = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 10))),
            MaNhaCungCapMacDinh = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 11))),
            NgayMuaHangRaw = ChuanHoaTuyChon(LaySo(worksheet.Cell(rowNumber, 12))),
            HanSuDungRaw = ChuanHoaTuyChon(LaySo(worksheet.Cell(rowNumber, 13))),
            MoqRaw = ChuanHoaTuyChon(LaySo(worksheet.Cell(rowNumber, 14))),
            MaThueVat = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 15))),
            TonToiThieuRaw = ChuanHoaTuyChon(LaySo(worksheet.Cell(rowNumber, 16))),
            MaKhoLuuTru = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 17)))
        };

    private static Dictionary<string, List<WorkshopMapping>> DocPhanXuong(IXLWorksheet worksheet)
    {
        var result = new Dictionary<string, List<WorkshopMapping>>(StringComparer.OrdinalIgnoreCase);
        var lastRow = worksheet.LastRowUsed(XLCellsUsedOptions.Contents)?.RowNumber() ?? HeaderRow;
        for (var rowNumber = DataStartRow; rowNumber <= lastRow; rowNumber++)
        {
            if (LaDongTrong(worksheet, rowNumber, 2)) continue;
            var maVatTu = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 1)));
            var maPhanXuong = ChuanHoaMa(LayChuoi(worksheet.Cell(rowNumber, 2)));
            if (string.IsNullOrWhiteSpace(maVatTu) || string.IsNullOrWhiteSpace(maPhanXuong))
            {
                throw new InvalidOperationException(
                    $"Dòng {rowNumber} trong sheet '{WorkshopSheetName}' phải có đủ Mã vật tư và Mã phân xưởng.");
            }
            if (!result.TryGetValue(maVatTu, out var list))
            {
                list = [];
                result.Add(maVatTu, list);
            }
            list.Add(new WorkshopMapping(rowNumber, maPhanXuong));
        }
        return result;
    }

    private static void KiemTraFile(Stream fileStream, string fileName, long fileSize)
    {
        if (fileStream is null || !fileStream.CanRead)
        {
            throw new InvalidOperationException("Không thể đọc file import vật tư.");
        }
        if (fileSize <= 0)
        {
            throw new InvalidOperationException("File import vật tư đang trống.");
        }
        if (fileSize > MaxFileSize)
        {
            throw new InvalidOperationException("Dung lượng file import không được vượt quá 20 MB.");
        }
        if (!string.Equals(Path.GetExtension(fileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("File import phải có định dạng .xlsx.");
        }
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

        if (cell.DataType == XLDataType.Number
            && cell.TryGetValue<decimal>(out var number))
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        return cell.GetFormattedString().Trim();
    }

    private static PhamViSuDungVatTu? ParsePhamVi(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var value = ChuanHoaGiaTriLuaChon(raw);
        var ma = LayMaLuaChon(value);

        if (ma == "1"
            || value.Equals("Tất cả phân xưởng", StringComparison.OrdinalIgnoreCase))
        {
            return PhamViSuDungVatTu.TatCaPhanXuong;
        }

        if (ma == "2"
            || value.Equals("Phân xưởng cụ thể", StringComparison.OrdinalIgnoreCase))
        {
            return PhamViSuDungVatTu.PhanXuongCuThe;
        }

        return null;
    }

    private static PhuongThucCungUngVatTu? ParsePhuongThuc(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        var value = ChuanHoaGiaTriLuaChon(raw);
        var ma = LayMaLuaChon(value);

        if (ma == "1"
            || value.Equals("Chỉ mua ngoài", StringComparison.OrdinalIgnoreCase))
        {
            return PhuongThucCungUngVatTu.ChiMuaNgoai;
        }

        if (ma == "2"
            || value.Equals("Mua hoặc tự sản xuất", StringComparison.OrdinalIgnoreCase))
        {
            return PhuongThucCungUngVatTu.MuaHoacTuSanXuat;
        }

        if (ma == "3"
            || value.Equals("Chỉ tự sản xuất", StringComparison.OrdinalIgnoreCase))
        {
            return PhuongThucCungUngVatTu.ChiTuSanXuat;
        }

        return null;
    }

    private static string ChuanHoaGiaTriLuaChon(string raw)
        => raw.Trim()
            .Replace('–', '-')
            .Replace('—', '-')
            .Replace(' ', ' ');

    private static string? LayMaLuaChon(string value)
    {
        var separators = new[] { ' ', '-', '.', ':', ')', '(' };
        return value.Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();
    }

    private static bool TryParseInt(string? raw, out int value)
        => int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
            || int.TryParse(raw, NumberStyles.Integer, CultureInfo.GetCultureInfo("vi-VN"), out value);

    private static bool TryParseDecimal(string? raw, out decimal value)
    {
        if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out value)) return true;
        if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.GetCultureInfo("vi-VN"), out value)) return true;
        var normalized = raw?.Trim().Replace(" ", string.Empty).Replace(",", ".");
        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }


    private static void KiemTraBatBuoc(ImportRow row, string? value, string tenCot)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            row.Loi.Add($"Dòng {row.RowNumber} không có dữ liệu {tenCot}.");
        }
    }

    private static void KiemTraDoDai(ImportRow row, string? value, int maxLength, string tenCot)
    {
        if (value?.Length > maxLength)
        {
            row.Loi.Add($"{tenCot} không được vượt quá {maxLength} ký tự.");
        }
    }

    private static string? ChuanHoaMa(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    private static string? ChuanHoaTuyChon(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record ImportValidationResult(IReadOnlyList<ImportRow> Rows);
    private sealed record WorkshopMapping(int RowNumber, string MaPhanXuong);

    private sealed class ImportRow(int rowNumber)
    {
        public int RowNumber { get; } = rowNumber;
        public string? MaVatTu { get; set; }
        public string? TenVatTu { get; set; }
        public string? TenTiengAnh { get; set; }
        public string? MaDonViTinh { get; set; }
        public string? QuyCachDongGoi { get; set; }
        public string? PhamViRaw { get; set; }
        public string? MaNhomVatTu { get; set; }
        public string? MucDichSuDung { get; set; }
        public string? PhuongThucRaw { get; set; }
        public string? MaCoSoMuaVatTu { get; set; }
        public string? MaNhaCungCapMacDinh { get; set; }
        public string? NgayMuaHangRaw { get; set; }
        public string? HanSuDungRaw { get; set; }
        public string? MoqRaw { get; set; }
        public string? MaThueVat { get; set; }
        public string? TonToiThieuRaw { get; set; }
        public string? MaKhoLuuTru { get; set; }

        public PhamViSuDungVatTu? PhamViSuDung { get; set; }
        public PhuongThucCungUngVatTu? PhuongThucCungUng { get; set; }
        public Guid? DonViTinhId { get; set; }
        public Guid? NhomVatTuId { get; set; }
        public Guid? CoSoMuaVatTuId { get; set; }
        public Guid? NhaCungCapMacDinhId { get; set; }
        public int? NgayMuaHang { get; set; }
        public int? HanSuDungNgay { get; set; }
        public decimal? Moq { get; set; }
        public Guid? ThueVatId { get; set; }
        public decimal? TonToiThieu { get; set; }
        public Guid? KhoLuuTruId { get; set; }
        public List<Guid> PhanXuongIds { get; } = [];
        public List<WorkshopMapping> WorkshopMappings { get; } = [];
        public List<string> Loi { get; } = [];
    }
}
