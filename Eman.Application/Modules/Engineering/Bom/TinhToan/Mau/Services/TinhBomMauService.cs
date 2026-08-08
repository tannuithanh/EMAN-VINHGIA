using System.Globalization;
using Eman.Application.Common.Helpers;
using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Dtos;
using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Interfaces;
using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Models;

namespace Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Services;

/// <summary>
/// Điều phối toàn bộ nghiệp vụ tính B.O.M màu.
/// Repository chỉ chịu trách nhiệm đọc dữ liệu; mọi quy tắc phân tích mã,
/// chọn nhóm M và tính lượng tiêu hao nằm tại Service.
/// </summary>
public sealed class TinhBomMauService(
    ITraCuuTinhBomMauRepository repository) : ITinhBomMauService
{
    private const int SoChuSoLamTron = 6;

    public async Task<KetQuaKiemThuBomMauDto> KiemThuAsync(
        string maSanPham,
        CancellationToken cancellationToken)
    {
        var maDaChuanHoa = ChuoiHelper.ChuanHoaMa(maSanPham);
        var ketQua = new KetQuaKiemThuBomMauDto
        {
            MaSanPham = maDaChuanHoa
        };

        if (!ThuPhanTichMaSanPham(maDaChuanHoa, out var maDaPhanTich, out var loiPhanTich))
        {
            ketQua.TrangThai = "MA_SAN_PHAM_KHONG_HOP_LE";
            ketQua.LoiCauHinh.Add(loiPhanTich);
            return HoanTat(ketQua);
        }

        ketQua.PhanTichMaSanPham = new ThongTinPhanTichMaSanPhamBomMauDto
        {
            MaHe = maDaPhanTich.MaHe,
            PhanDoanChuaDeTai = maDaPhanTich.PhanDoanChuaDeTai,
            MaMau = maDaPhanTich.MaMau,
            MaHangNen = maDaPhanTich.MaHangNen
        };

        var heVaDeTai = await repository.LayHeVaDeTaiAsync(
            maDaPhanTich.MaHe,
            cancellationToken);

        if (heVaDeTai is null)
        {
            ketQua.LoiCauHinh.Add(
                $"Không tìm thấy hệ sản phẩm '{maDaPhanTich.MaHe}' trong bảng md_he_san_pham.");
            return HoanTat(ketQua);
        }

        ketQua.PhanTichMaSanPham.HeSanPhamId = heVaDeTai.HeSanPhamId;
        ketQua.PhanTichMaSanPham.TenHe = heVaDeTai.TenHe;

        if (!heVaDeTai.IsActive)
        {
            ketQua.LoiCauHinh.Add(
                $"Hệ sản phẩm '{heVaDeTai.MaHe}' đang ngừng hoạt động.");
        }

        var deTai = TimDeTaiTheoPhanDoan(
            maDaPhanTich.PhanDoanChuaDeTai,
            heVaDeTai.DeTais,
            ketQua.LoiCauHinh);

        if (deTai is null)
        {
            return HoanTat(ketQua);
        }

        ketQua.PhanTichMaSanPham.DeTaiId = deTai.Id;
        ketQua.PhanTichMaSanPham.MaDeTai = deTai.MaDeTai;
        ketQua.PhanTichMaSanPham.TenDeTai = deTai.TenDeTai;

        if (!deTai.IsActive)
        {
            ketQua.LoiCauHinh.Add(
                $"Đề tài '{deTai.MaDeTai}' đang ngừng hoạt động.");
        }

        var mauSac = await repository.LayMauSacAsync(
            heVaDeTai.HeSanPhamId,
            deTai.Id,
            maDaPhanTich.MaMau,
            cancellationToken);

        if (mauSac is null)
        {
            ketQua.LoiCauHinh.Add(
                $"Không tìm thấy màu '{maDaPhanTich.MaMau}' thuộc hệ " +
                $"'{heVaDeTai.MaHe}', đề tài '{deTai.MaDeTai}'.");
        }
        else
        {
            ketQua.MauSac = new ThongTinMauSacTinhBomMauDto
            {
                Id = mauSac.Id,
                MaMau = mauSac.MaMau,
                TenMau = mauSac.TenMau,
                MaCotTho = mauSac.MaCotTho,
                IsActive = mauSac.IsActive
            };

            if (!mauSac.IsActive)
            {
                ketQua.LoiCauHinh.Add(
                    $"Màu '{mauSac.MaMau}' đang ngừng hoạt động.");
            }
        }

        var maHang = await repository.LayMaHangAsync(
            maDaPhanTich.MaHangNen,
            cancellationToken);

        if (maHang is null)
        {
            ketQua.LoiCauHinh.Add(
                $"Không tìm thấy mã hàng nền '{maDaPhanTich.MaHangNen}' trong bảng md_ma_hang.");
            return HoanTat(ketQua);
        }

        ketQua.MaHang = ChuyenMaHangDto(maHang);

        if (!maHang.IsActive)
        {
            ketQua.LoiCauHinh.Add(
                $"Mã hàng nền '{maHang.MaHang}' đang ngừng hoạt động.");
        }

        if (!maHang.DienTich.HasValue)
        {
            ketQua.LoiCauHinh.Add(
                $"Mã hàng '{maHang.MaHang}' chưa khai báo diện tích.");
        }

        if (!maHang.HinhDangBomMauId.HasValue)
        {
            ketQua.LoiCauHinh.Add(
                $"Mã hàng '{maHang.MaHang}' chưa khai báo hình dáng B.O.M màu.");
        }

        QuyTacNhomMTraCuuBomMau? quyTacNhomM = null;
        if (maHang.DienTich.HasValue && maHang.HinhDangBomMauId.HasValue)
        {
            var cacQuyTac = await repository.LayCacQuyTacNhomMAsync(
                maHang.HinhDangBomMauId.Value,
                cancellationToken);

            var cacQuyTacPhuHop = cacQuyTac
                .Where(item => NamTrongKhoangDienTich(item, maHang.DienTich.Value))
                .OrderBy(item => item.DienTichTu)
                .ThenBy(item => item.DienTichDen)
                .ToList();

            if (cacQuyTacPhuHop.Count == 0)
            {
                ketQua.LoiCauHinh.Add(
                    $"Không tìm thấy quy tắc nhóm M B.O.M màu phù hợp với hình dáng " +
                    $"'{maHang.MaHinhDangBomMau ?? maHang.HinhDangBomMauId.Value.ToString(CultureInfo.InvariantCulture)}' " +
                    $"và diện tích {DinhDangSo(maHang.DienTich.Value)}.");
            }
            else if (cacQuyTacPhuHop.Count > 1)
            {
                ketQua.LoiCauHinh.Add(
                    $"Có {cacQuyTacPhuHop.Count} quy tắc nhóm M cùng khớp với diện tích " +
                    $"{DinhDangSo(maHang.DienTich.Value)}. Cần xử lý vùng diện tích bị chồng lấn.");
            }
            else
            {
                quyTacNhomM = cacQuyTacPhuHop[0];
                ketQua.NhomM = new ThongTinNhomMTinhBomMauDto
                {
                    Id = quyTacNhomM.NhomMId,
                    MaNhomM = quyTacNhomM.MaNhomM,
                    TenNhomM = quyTacNhomM.TenNhomM,
                    QuyTacNhomMId = quyTacNhomM.Id,
                    DienTichTu = quyTacNhomM.DienTichTu,
                    DienTichDen = quyTacNhomM.DienTichDen,
                    BaoGomTu = quyTacNhomM.BaoGomTu,
                    BaoGomDen = quyTacNhomM.BaoGomDen
                };
            }
        }

        if (mauSac is not null && quyTacNhomM is not null && maHang.DienTich.HasValue)
        {
            var goiDuLieuBuoc = await repository.LayGoiDuLieuBuocAsync(
                heVaDeTai.HeSanPhamId,
                deTai.Id,
                mauSac.Id,
                quyTacNhomM.NhomMId,
                cancellationToken);

            TinhCacBuoc(
                ketQua,
                goiDuLieuBuoc,
                maHang.DienTich.Value);
        }

        await BoSungThongTinPhuTroAsync(
            ketQua,
            maHang,
            mauSac,
            maDaPhanTich.MaHangNen,
            cancellationToken);

        ketQua.DaTinhThanhCong =
            ketQua.CacBuoc.Count > 0 &&
            ketQua.CacBuoc.All(item => item.DaTinhDuoc) &&
            !ketQua.LoiCauHinh.Any();

        ketQua.DayDuThongTinPhuTro =
            mauSac is not null &&
            !string.IsNullOrWhiteSpace(mauSac.MaCotTho) &&
            ketQua.CotTho.TonTaiTrongDanhMuc &&
            ketQua.CotTho.IsActive == true;

        ketQua.TrangThai = ketQua.DaTinhThanhCong
            ? ketQua.CanhBao.Count == 0 && ketQua.DayDuThongTinPhuTro
                ? "THANH_CONG"
                : "THANH_CONG_CO_CANH_BAO"
            : "THIEU_CAU_HINH";

        return HoanTat(ketQua);
    }

    private static void TinhCacBuoc(
        KetQuaKiemThuBomMauDto ketQua,
        GoiDuLieuBuocTraCuuBomMau goiDuLieu,
        decimal dienTich)
    {
        if (goiDuLieu.BuocNhoms.Count == 0)
        {
            ketQua.LoiCauHinh.Add(
                "Không tìm thấy bước/hỗn hợp hoạt động cho màu sản phẩm trong bảng md_buoc_nhom_theo_mau.");
            return;
        }

        var buocTheoMa = goiDuLieu.Buocs
            .GroupBy(item => item.MaBuoc, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var dinhMucTheoBuocNhom = goiDuLieu.DinhMucs
            .GroupBy(item => item.BuocNhomMauId)
            .ToDictionary(group => group.Key, group => group.First());
        var heSoDeTaiTheoBuoc = goiDuLieu.HeSoDeTais
            .GroupBy(item => item.BuocId)
            .ToDictionary(group => group.Key, group => group.First());
        var heSoMauTheoBuoc = goiDuLieu.HeSoMaus
            .GroupBy(item => item.BuocId)
            .ToDictionary(group => group.Key, group => group.First());

        var thuTu = 0;
        foreach (var buocNhom in goiDuLieu.BuocNhoms
                     .OrderBy(item => item.Id))
        {
            thuTu++;
            var chiTiet = new ChiTietBuocTinhBomMauDto
            {
                ThuTu = thuTu,
                BuocNhomMauId = buocNhom.Id,
                MaBuoc = buocNhom.MaBuoc,
                TenBuoc = buocNhom.TenBuoc,
                MaHonHopId = buocNhom.MaHonHopId,
                MaHonHop = buocNhom.MaHonHop,
                DienTich = dienTich,
                SoChuSoLamTron = SoChuSoLamTron
            };

            if (!buocTheoMa.TryGetValue(buocNhom.MaBuoc, out var buoc))
            {
                chiTiet.LoiCauHinh.Add(
                    $"Mã bước '{buocNhom.MaBuoc}' chưa được khai báo hoạt động trong bảng md_bom_buoc.");
            }
            else
            {
                chiTiet.BuocId = buoc.Id;
            }

            if (!dinhMucTheoBuocNhom.TryGetValue(buocNhom.Id, out var dinhMuc))
            {
                chiTiet.LoiCauHinh.Add(
                    "Thiếu định mức của bước/hỗn hợp theo nhóm M đang xác định.");
            }
            else
            {
                chiTiet.DinhMucNhomMId = dinhMuc.Id;
                chiTiet.DinhMucNhomM = dinhMuc.DinhMuc;
            }

            if (buoc is not null)
            {
                if (!heSoDeTaiTheoBuoc.TryGetValue(buoc.Id, out var heSoDeTai))
                {
                    chiTiet.LoiCauHinh.Add(
                        "Thiếu hệ số đề tài theo hệ + đề tài + bước.");
                }
                else
                {
                    chiTiet.HeSoDeTaiId = heSoDeTai.Id;
                    chiTiet.HeSoDeTai = heSoDeTai.HeSo;
                }

                if (!heSoMauTheoBuoc.TryGetValue(buoc.Id, out var heSoMau))
                {
                    chiTiet.LoiCauHinh.Add(
                        "Thiếu hệ số màu theo hệ + đề tài + màu + bước.");
                }
                else
                {
                    chiTiet.HeSoMauId = heSoMau.Id;
                    chiTiet.HeSoMau = heSoMau.HeSo;
                }
            }

            if (chiTiet.LoiCauHinh.Count == 0 &&
                chiTiet.DinhMucNhomM.HasValue &&
                chiTiet.HeSoDeTai.HasValue &&
                chiTiet.HeSoMau.HasValue)
            {
                var chuaLamTron =
                    dienTich *
                    chiTiet.DinhMucNhomM.Value *
                    chiTiet.HeSoDeTai.Value *
                    chiTiet.HeSoMau.Value;
                var daLamTron = decimal.Round(
                    chuaLamTron,
                    SoChuSoLamTron,
                    MidpointRounding.AwayFromZero);

                chiTiet.LuongTieuHaoChuaLamTron = chuaLamTron;
                chiTiet.LuongTieuHao = daLamTron;
                chiTiet.CongThucThaySo =
                    $"{DinhDangSo(dienTich)} × " +
                    $"{DinhDangSo(chiTiet.DinhMucNhomM.Value)} × " +
                    $"{DinhDangSo(chiTiet.HeSoDeTai.Value)} × " +
                    $"{DinhDangSo(chiTiet.HeSoMau.Value)} = " +
                    DinhDangSo(daLamTron);
                chiTiet.DaTinhDuoc = true;
            }

            foreach (var loi in chiTiet.LoiCauHinh)
            {
                ketQua.LoiCauHinh.Add(
                    $"Bước '{chiTiet.MaBuoc}' - hỗn hợp '{chiTiet.MaHonHop}': {loi}");
            }

            ketQua.CacBuoc.Add(chiTiet);
        }
    }

    private async Task BoSungThongTinPhuTroAsync(
        KetQuaKiemThuBomMauDto ketQua,
        MaHangTraCuuBomMau maHang,
        MauSacTraCuuBomMau? mauSac,
        string maHangNen,
        CancellationToken cancellationToken)
    {
        var chauInserts = await repository.LayChauInsertsAsync(
            maHang.Id,
            cancellationToken);

        ketQua.ChauInserts = chauInserts
            .Select(item => new ChauInsertTinhBomMauDto
            {
                ChauInsertId = item.ChauInsertId,
                MaChauInsert = item.MaChauInsert,
                TenChauInsert = item.TenChauInsert,
                SoLuong = item.SoLuong,
                GhiChu = item.GhiChu,
                IsActive = item.IsActive
            })
            .ToList();

        foreach (var insert in ketQua.ChauInserts.Where(item => !item.IsActive))
        {
            ketQua.CanhBao.Add(
                $"Chậu insert '{insert.MaChauInsert}' được gán cho mã hàng nhưng đang ngừng hoạt động.");
        }

        var phen = await repository.LayPhenAsync(
            maHang.Id,
            cancellationToken);
        ketQua.Phen = phen is null
            ? new ThongTinPhenTinhBomMauDto { CoPhen = false }
            : new ThongTinPhenTinhBomMauDto
            {
                CoPhen = true,
                MaHangPhen = phen.MaHangPhen,
                GhiChu = phen.GhiChu
            };

        if (mauSac is null || string.IsNullOrWhiteSpace(mauSac.MaCotTho))
        {
            ketQua.CotTho = new ThongTinCotThoTinhBomMauDto();
            ketQua.CanhBao.Add(
                "Màu sản phẩm chưa khai báo mã cốt thô trong cột ma_cot_tho.");
            return;
        }

        var maCotTho = ChuoiHelper.ChuanHoaMa(mauSac.MaCotTho);
        var maHangCotTho = maCotTho.StartsWith(maHangNen + "-", StringComparison.OrdinalIgnoreCase)
            ? maCotTho
            : $"{maHangNen}-{maCotTho}";
        var cotTho = await repository.LayMaHangAsync(
            maHangCotTho,
            cancellationToken);

        ketQua.CotTho = new ThongTinCotThoTinhBomMauDto
        {
            MaCotTho = maCotTho,
            MaHangCotThoDuKien = maHangCotTho,
            TonTaiTrongDanhMuc = cotTho is not null,
            MaHangCotThoId = cotTho?.Id,
            IsActive = cotTho?.IsActive
        };

        if (cotTho is null)
        {
            ketQua.CanhBao.Add(
                $"Đã xác định mã cốt thô dự kiến '{maHangCotTho}' nhưng chưa tìm thấy trong md_ma_hang.");
        }
        else if (!cotTho.IsActive)
        {
            ketQua.CanhBao.Add(
                $"Mã cốt thô '{maHangCotTho}' đang ngừng hoạt động.");
        }
    }

    private static DeTaiTraCuuBomMau? TimDeTaiTheoPhanDoan(
        string phanDoanChuaDeTai,
        IReadOnlyList<DeTaiTraCuuBomMau> deTais,
        ICollection<string> loiCauHinh)
    {
        var ungViens = deTais
            .Where(item =>
                phanDoanChuaDeTai.EndsWith(
                    item.MaDeTai,
                    StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.MaDeTai.Length)
            .ThenBy(item => item.MaDeTai, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (ungViens.Count == 0)
        {
            var danhSachDeTai = deTais.Count == 0
                ? "Hệ chưa có đề tài nào."
                : $"Các mã đề tài hiện có: {string.Join(", ", deTais.Select(item => item.MaDeTai))}.";
            loiCauHinh.Add(
                $"Không xác định được đề tài từ phân đoạn '{phanDoanChuaDeTai}'. {danhSachDeTai}");
            return null;
        }

        var doDaiLonNhat = ungViens[0].MaDeTai.Length;
        var ungVienCungDoDai = ungViens
            .Where(item => item.MaDeTai.Length == doDaiLonNhat)
            .ToList();

        if (ungVienCungDoDai.Count > 1)
        {
            loiCauHinh.Add(
                $"Không thể xác định duy nhất đề tài từ phân đoạn '{phanDoanChuaDeTai}'. " +
                $"Các đề tài cùng khớp: {string.Join(", ", ungVienCungDoDai.Select(item => item.MaDeTai))}.");
            return null;
        }

        return ungViens[0];
    }

    private static bool NamTrongKhoangDienTich(
        QuyTacNhomMTraCuuBomMau quyTac,
        decimal dienTich)
    {
        var datCanDuoi = quyTac.BaoGomTu
            ? dienTich >= quyTac.DienTichTu
            : dienTich > quyTac.DienTichTu;
        var datCanTren = !quyTac.DienTichDen.HasValue ||
                         (quyTac.BaoGomDen
                             ? dienTich <= quyTac.DienTichDen.Value
                             : dienTich < quyTac.DienTichDen.Value);
        return datCanDuoi && datCanTren;
    }

    private static ThongTinMaHangTinhBomMauDto ChuyenMaHangDto(
        MaHangTraCuuBomMau maHang)
        => new()
        {
            Id = maHang.Id,
            MaHang = maHang.MaHang,
            LoaiMaHang = maHang.LoaiMaHang,
            DienTich = maHang.DienTich,
            HinhDangBomMauId = maHang.HinhDangBomMauId,
            MaHinhDangBomMau = maHang.MaHinhDangBomMau,
            TenHinhDangBomMau = maHang.TenHinhDangBomMau,
            IsActive = maHang.IsActive
        };

    private static bool ThuPhanTichMaSanPham(
        string maSanPham,
        out MaSanPhamBomMauDaPhanTich ketQua,
        out string loi)
    {
        ketQua = new MaSanPhamBomMauDaPhanTich();
        loi = string.Empty;

        if (string.IsNullOrWhiteSpace(maSanPham))
        {
            loi = "Mã sản phẩm là bắt buộc.";
            return false;
        }

        var phanDoans = maSanPham.Split(
            '-',
            StringSplitOptions.TrimEntries);

        if (phanDoans.Length < 4 || phanDoans.Any(string.IsNullOrWhiteSpace))
        {
            loi =
                "Mã sản phẩm không đúng cấu trúc B.O.M màu. " +
                "Ví dụ hợp lệ: 66-52220-01-02.";
            return false;
        }

        ketQua = new MaSanPhamBomMauDaPhanTich
        {
            MaHe = phanDoans[0],
            PhanDoanChuaDeTai = phanDoans[1],
            MaMau = phanDoans[^1],
            MaHangNen = string.Join("-", phanDoans[..^1])
        };
        return true;
    }

    private static KetQuaKiemThuBomMauDto HoanTat(
        KetQuaKiemThuBomMauDto ketQua)
    {
        ketQua.LoiCauHinh = ketQua.LoiCauHinh
            .Distinct(StringComparer.Ordinal)
            .ToList();
        ketQua.CanhBao = ketQua.CanhBao
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (!ketQua.DaTinhThanhCong &&
            string.Equals(ketQua.TrangThai, "CHUA_TINH", StringComparison.Ordinal))
        {
            ketQua.TrangThai = "THIEU_CAU_HINH";
        }

        return ketQua;
    }

    private static string DinhDangSo(decimal value)
        => value.ToString("0.######", CultureInfo.InvariantCulture);

    private sealed class MaSanPhamBomMauDaPhanTich
    {
        public string MaHe { get; init; } = string.Empty;
        public string PhanDoanChuaDeTai { get; init; } = string.Empty;
        public string MaMau { get; init; } = string.Empty;
        public string MaHangNen { get; init; } = string.Empty;
    }
}
