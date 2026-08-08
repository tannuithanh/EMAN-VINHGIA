using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;
using Eman.Domain.Modules.Engineering.Bom.Mau.Entities;
using Eman.Domain.Modules.MasterData.BusinessPartners.Entities;
using Eman.Domain.Modules.MasterData.Common.Entities;
using Eman.Domain.Modules.MasterData.Inventory.Entities;
using Eman.Domain.Modules.MasterData.Materials.Entities;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using Eman.Domain.Modules.MasterData.Production.Entities;
using Eman.Domain.Modules.MasterData.Products.Entities;
using Eman.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Eman.Api.Tests.Infrastructure;

/// <summary>
/// Dữ liệu nền cố định phục vụ kiểm thử API trên database EmanMasterDataDb_Test.
/// </summary>
public static class DuLieuKiemThu
{
    public static readonly Guid DonViTinhHoatDongId = Guid.Parse("00000000-0000-0000-0000-000000000101");
    public static readonly Guid DonViTinhNgungId = Guid.Parse("00000000-0000-0000-0000-000000000102");
    public static readonly Guid NhomVatTuHoatDongId = Guid.Parse("00000000-0000-0000-0000-000000000201");
    public static readonly Guid NhomVatTuNgungId = Guid.Parse("00000000-0000-0000-0000-000000000202");
    public static readonly Guid CoSoMuaHoatDongId = Guid.Parse("00000000-0000-0000-0000-000000000301");
    public static readonly Guid CoSoMuaNgungId = Guid.Parse("00000000-0000-0000-0000-000000000302");
    public static readonly Guid KhoLuuTruId = Guid.Parse("00000000-0000-0000-0000-000000000401");
    public static readonly Guid KhoMacDinhId = Guid.Parse("00000000-0000-0000-0000-000000000402");
    public static readonly Guid KhoTonId = Guid.Parse("00000000-0000-0000-0000-000000000403");
    public static readonly Guid KhoKhongPhaiHangTonId = Guid.Parse("00000000-0000-0000-0000-000000000404");
    public static readonly Guid KhoNgungId = Guid.Parse("00000000-0000-0000-0000-000000000405");
    public static readonly Guid PhanXuong1Id = Guid.Parse("00000000-0000-0000-0000-000000000501");
    public static readonly Guid PhanXuong2Id = Guid.Parse("00000000-0000-0000-0000-000000000502");
    public static readonly Guid PhanXuongNgungId = Guid.Parse("00000000-0000-0000-0000-000000000503");
    public static readonly Guid NhomNangLucHoatDongId = Guid.Parse("00000000-0000-0000-0000-000000000601");
    public static readonly Guid NhomNangLucNgungId = Guid.Parse("00000000-0000-0000-0000-000000000602");
    public static readonly Guid ThueHoatDongId = Guid.Parse("00000000-0000-0000-0000-000000000701");
    public static readonly Guid ThueNgungId = Guid.Parse("00000000-0000-0000-0000-000000000702");
    public static readonly Guid LoaiDoiTacId = Guid.Parse("00000000-0000-0000-0000-000000000801");
    public static readonly Guid NhaCungCapId = Guid.Parse("00000000-0000-0000-0000-000000000802");
    public static readonly Guid DoiTacKhongPhaiNhaCungCapId = Guid.Parse("00000000-0000-0000-0000-000000000803");
    public static readonly Guid NhaCungCapNgungId = Guid.Parse("00000000-0000-0000-0000-000000000804");
    public static readonly Guid VatTuCoSanId = Guid.Parse("00000000-0000-0000-0000-000000000901");
    public static readonly Guid BomVatTuDauRaId = Guid.Parse("00000000-0000-0000-0000-000000000902");
    public static readonly Guid BomVatTuThanhPhan1Id = Guid.Parse("00000000-0000-0000-0000-000000000903");
    public static readonly Guid BomVatTuThanhPhan2Id = Guid.Parse("00000000-0000-0000-0000-000000000904");
    public static readonly Guid SanPhamCoSanId = Guid.Parse("00000000-0000-0000-0000-000000001001");

    public const string MaVatTuCoSan = "VT-TEST-CO-SAN";
    public const string MaBomVatTuDauRa = "BOM-VT-DAU-RA";
    public const string MaBomVatTuThanhPhan1 = "BOM-VT-TP-01";
    public const string MaBomVatTuThanhPhan2 = "BOM-VT-TP-02";
    public const string MaSanPhamCoSan = "SP-TEST-CO-SAN";
    public const string MaSanPhamTinhBomMau = "66-52220-01-02";
    public const string RowVersionHopLe = "AQIDBA==";

    public const long HeSanPham66Id = 5;
    public const long HeSanPham68Id = 6;

    public static long DeTaiHe66Id { get; private set; }
    public static long DeTaiHe68Id { get; private set; }
    public static long MauSacHe66Id { get; private set; }
    public static long MauSacHe68Id { get; private set; }
    public static long HinhDangBomId { get; private set; }
    public static long MaHangBomId { get; private set; }
    public static long NhomMBomId { get; private set; }
    public static long NhomMThoBomId { get; private set; }
    public static long BomMauBuocId { get; private set; }
    public static long BuocNhomTheoMauId { get; private set; }
    public static Guid ChauInsertBomId { get; private set; }

    public static async Task KhoiTaoAsync(EmanDbContext dbContext)
    {
        if (!dbContext.Database.IsSqlServer())
        {
            throw new InvalidOperationException(
                "Bộ kiểm thử hiện tại bắt buộc phải chạy bằng SQL Server.");
        }

        BaoVeCoSoDuLieuKiemThu.KiemTraVaChuanHoa(
            dbContext.Database.GetConnectionString());

        dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(120));
        SqlConnection.ClearAllPools();

        // Chỉ database kiểm thử mới được phép bị xóa và tạo lại.
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var donViTinhHoatDong = new DonViTinh
        {
            Id = DonViTinhHoatDongId,
            MaDonViTinh = "DVT-TEST",
            TenDonViTinh = "Đơn vị kiểm thử",
            KyHieu = "ĐVT",
            TrangThai = TrangThaiHoatDong.HoatDong
        };
        var donViTinhNgung = new DonViTinh
        {
            Id = DonViTinhNgungId,
            MaDonViTinh = "DVT-NGUNG",
            TenDonViTinh = "Đơn vị ngừng hoạt động",
            TrangThai = TrangThaiHoatDong.NgungHoatDong
        };

        var nhomVatTuHoatDong = new NhomVatTu
        {
            Id = NhomVatTuHoatDongId,
            MaNhomVatTu = "NVT-TEST",
            TenNhomVatTu = "Nhóm vật tư kiểm thử",
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };
        var nhomVatTuNgung = new NhomVatTu
        {
            Id = NhomVatTuNgungId,
            MaNhomVatTu = "NVT-NGUNG",
            TenNhomVatTu = "Nhóm vật tư ngừng",
            TrangThai = TrangThaiHoatDong.NgungHoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };

        var coSoMuaHoatDong = new CoSoMuaVatTu
        {
            Id = CoSoMuaHoatDongId,
            MaCoSoMuaVatTu = "CSM-TEST",
            TenCoSoMuaVatTu = "Cơ sở mua kiểm thử",
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };
        var coSoMuaNgung = new CoSoMuaVatTu
        {
            Id = CoSoMuaNgungId,
            MaCoSoMuaVatTu = "CSM-NGUNG",
            TenCoSoMuaVatTu = "Cơ sở mua ngừng",
            TrangThai = TrangThaiHoatDong.NgungHoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };

        var khoLuuTru = TaoKho(KhoLuuTruId, "KHO-LUU", "Kho lưu trữ", false, TrangThaiHoatDong.HoatDong);
        var khoMacDinh = TaoKho(KhoMacDinhId, "KHO-MD", "Kho mặc định", false, TrangThaiHoatDong.HoatDong);
        var khoTon = TaoKho(KhoTonId, "KHO-TON", "Kho tồn", true, TrangThaiHoatDong.HoatDong);
        var khoKhongTon = TaoKho(KhoKhongPhaiHangTonId, "KHO-KHONG-TON", "Kho không phải hàng tồn", false, TrangThaiHoatDong.HoatDong);
        var khoNgung = TaoKho(KhoNgungId, "KHO-NGUNG", "Kho ngừng", true, TrangThaiHoatDong.NgungHoatDong);

        var phanXuong1 = TaoPhanXuong(PhanXuong1Id, "PX-01", "Phân xưởng 01", TrangThaiHoatDong.HoatDong);
        var phanXuong2 = TaoPhanXuong(PhanXuong2Id, "PX-02", "Phân xưởng 02", TrangThaiHoatDong.HoatDong);
        var phanXuongNgung = TaoPhanXuong(PhanXuongNgungId, "PX-NGUNG", "Phân xưởng ngừng", TrangThaiHoatDong.NgungHoatDong);

        var nhomNangLucHoatDong = new NhomNangLuc
        {
            Id = NhomNangLucHoatDongId,
            MaNhomNangLuc = "NNL-TEST",
            TenNhomNangLuc = "Nhóm năng lực kiểm thử",
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };
        var nhomNangLucNgung = new NhomNangLuc
        {
            Id = NhomNangLucNgungId,
            MaNhomNangLuc = "NNL-NGUNG",
            TenNhomNangLuc = "Nhóm năng lực ngừng",
            TrangThai = TrangThaiHoatDong.NgungHoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };

        var thueHoatDong = TaoThue(ThueHoatDongId, "VAT10", "VAT 10%", 10, TrangThaiHoatDong.HoatDong);
        var thueNgung = TaoThue(ThueNgungId, "VAT-NGUNG", "Thuế ngừng", 8, TrangThaiHoatDong.NgungHoatDong);

        var loaiDoiTac = new LoaiDoiTac
        {
            Id = LoaiDoiTacId,
            MaLoaiDoiTac = "LDT-TEST",
            TenLoaiDoiTac = "Loại đối tác kiểm thử",
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };
        var nhaCungCap = TaoDoiTac(NhaCungCapId, "NCC-TEST", "Nhà cung cấp kiểm thử", true, TrangThaiHoatDong.HoatDong);
        var doiTacKhongPhaiNcc = TaoDoiTac(DoiTacKhongPhaiNhaCungCapId, "DT-TEST", "Đối tác không phải nhà cung cấp", false, TrangThaiHoatDong.HoatDong);
        var nhaCungCapNgung = TaoDoiTac(NhaCungCapNgungId, "NCC-NGUNG", "Nhà cung cấp ngừng", true, TrangThaiHoatDong.NgungHoatDong);

        dbContext.AddRange(
            donViTinhHoatDong, donViTinhNgung,
            nhomVatTuHoatDong, nhomVatTuNgung,
            coSoMuaHoatDong, coSoMuaNgung,
            khoLuuTru, khoMacDinh, khoTon, khoKhongTon, khoNgung,
            phanXuong1, phanXuong2, phanXuongNgung,
            nhomNangLucHoatDong, nhomNangLucNgung,
            thueHoatDong, thueNgung,
            loaiDoiTac);

        nhaCungCap.LoaiDoiTacId = LoaiDoiTacId;
        doiTacKhongPhaiNcc.LoaiDoiTacId = LoaiDoiTacId;
        nhaCungCapNgung.LoaiDoiTacId = LoaiDoiTacId;
        dbContext.AddRange(nhaCungCap, doiTacKhongPhaiNcc, nhaCungCapNgung);

        dbContext.VatTus.Add(new VatTu
        {
            Id = VatTuCoSanId,
            MaVatTu = MaVatTuCoSan,
            TenVatTu = "Vật tư có sẵn",
            DonViTinhId = DonViTinhHoatDongId,
            PhamViSuDung = PhamViSuDungVatTu.TatCaPhanXuong,
            NhomVatTuId = NhomVatTuHoatDongId,
            PhuongThucCungUng = PhuongThucCungUngVatTu.ChiTuSanXuat,
            HanSuDungNgay = 30,
            TonToiThieu = 0,
            KhoLuuTruId = KhoLuuTruId,
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        });

        dbContext.VatTus.AddRange(
            TaoVatTuBom(BomVatTuDauRaId, MaBomVatTuDauRa, "Vật tư đầu ra B.O.M kiểm thử"),
            TaoVatTuBom(BomVatTuThanhPhan1Id, MaBomVatTuThanhPhan1, "Vật tư thành phần B.O.M 01"),
            TaoVatTuBom(BomVatTuThanhPhan2Id, MaBomVatTuThanhPhan2, "Vật tư thành phần B.O.M 02"));

        dbContext.SanPhams.Add(new SanPham
        {
            Id = SanPhamCoSanId,
            MaSanPham = MaSanPhamCoSan,
            MoTaTiengViet = "Sản phẩm có sẵn",
            DonViTinhId = DonViTinhHoatDongId,
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        });

        await dbContext.SaveChangesAsync();
        await KhoiTaoDuLieuBomAsync(dbContext);
    }

    private static async Task KhoiTaoDuLieuBomAsync(EmanDbContext dbContext)
    {
        var heSanPhams = new[]
        {
            new HeSanPham { Id = 1, MaHe = "11", TenHe = "Hệ 11" },
            new HeSanPham { Id = 2, MaHe = "26", TenHe = "Hệ 26" },
            new HeSanPham { Id = 3, MaHe = "60", TenHe = "Hệ 60" },
            new HeSanPham { Id = 4, MaHe = "61", TenHe = "Hệ 61" },
            new HeSanPham { Id = HeSanPham66Id, MaHe = "66", TenHe = "Hệ 66" },
            new HeSanPham { Id = HeSanPham68Id, MaHe = "68", TenHe = "Hệ 68" },
            new HeSanPham { Id = 7, MaHe = "77", TenHe = "Hệ 77" },
            new HeSanPham { Id = 8, MaHe = "79", TenHe = "Hệ 79" },
            new HeSanPham { Id = 9, MaHe = "90", TenHe = "Hệ 90" },
            new HeSanPham { Id = 10, MaHe = "91", TenHe = "Hệ 91" },
            new HeSanPham { Id = 11, MaHe = "92", TenHe = "Hệ 92" },
            new HeSanPham { Id = 12, MaHe = "94", TenHe = "Hệ 94" },
            new HeSanPham { Id = 13, MaHe = "96", TenHe = "Hệ 96" },
            new HeSanPham { Id = 14, MaHe = "99", TenHe = "Hệ 99" }
        };

        dbContext.HeSanPhams.AddRange(heSanPhams);
        await dbContext.SaveChangesAsync();

        var deTai66 = new DeTai
        {
            HeSanPhamId = HeSanPham66Id,
            MaDeTai = "DT-TEST-66",
            TenDeTai = "Đề tài kiểm thử hệ 66"
        };
        var deTai68 = new DeTai
        {
            HeSanPhamId = HeSanPham68Id,
            MaDeTai = "DT-TEST-68",
            TenDeTai = "Đề tài kiểm thử hệ 68"
        };
        var hinhDang = new HinhDang
        {
            MaHinhDang = "HD-TEST-BOM",
            TenHinhDang = "Hình dáng kiểm thử B.O.M"
        };
        var nhomM = new NhomM
        {
            PhamViBom = "BOM_MAU",
            MaNhomM = "M-TEST",
            TenNhomM = "Nhóm M màu kiểm thử",
            ThuTu = 999
        };
        var nhomMTho = new NhomM
        {
            PhamViBom = "BOM_THO",
            MaNhomM = "M-TEST",
            TenNhomM = "Nhóm M thô kiểm thử",
            ThuTu = 999
        };
        var buocBomMau = new BomMauBuoc
        {
            MaBuoc = "BUOC-TEST",
            TenBuoc = "Bước B.O.M màu kiểm thử"
        };
        var chauInsert = new ChauInsert
        {
            MaChauInsert = "INSERT-TEST",
            TenChauInsert = "Chậu insert kiểm thử"
        };

        dbContext.AddRange(deTai66, deTai68, hinhDang, nhomM, nhomMTho, buocBomMau, chauInsert);
        await dbContext.SaveChangesAsync();

        DeTaiHe66Id = deTai66.Id;
        DeTaiHe68Id = deTai68.Id;
        HinhDangBomId = hinhDang.Id;
        NhomMBomId = nhomM.Id;
        NhomMThoBomId = nhomMTho.Id;
        BomMauBuocId = buocBomMau.Id;
        ChauInsertBomId = chauInsert.Id;

        var mauSac66 = new MauSac
        {
            HeSanPhamId = HeSanPham66Id,
            DeTaiId = deTai66.Id,
            MaMau = "MAU-TEST-66",
            TenMau = "Màu kiểm thử hệ 66"
        };
        var mauSac68 = new MauSac
        {
            HeSanPhamId = HeSanPham68Id,
            DeTaiId = deTai68.Id,
            MaMau = "MAU-TEST-68",
            TenMau = "Màu kiểm thử hệ 68"
        };
        var maHang = new MaHang
        {
            MaHangCode = "66-TEST-BOM",
            DienTich = 1.25m,
            HinhDangBomThoId = hinhDang.Id,
            HinhDangBomMauId = hinhDang.Id
        };

        dbContext.AddRange(mauSac66, mauSac68, maHang);
        await dbContext.SaveChangesAsync();

        MauSacHe66Id = mauSac66.Id;
        MauSacHe68Id = mauSac68.Id;
        MaHangBomId = maHang.Id;

        var buocNhomTheoMau = new BuocNhomTheoMau
        {
            HeSanPhamId = HeSanPham66Id,
            MauSacId = mauSac66.Id,
            MaBuoc = "BUOC-NHOM-TEST",
            TenBuoc = "Bước nhóm kiểm thử",
            MaHonHopId = 7001,
            MaHonHop = "HH-TEST-001"
        };

        dbContext.BuocNhomTheoMaus.Add(buocNhomTheoMau);
        await dbContext.SaveChangesAsync();
        BuocNhomTheoMauId = buocNhomTheoMau.Id;

        await KhoiTaoDuLieuTinhBomMauAsync(dbContext);
    }

    private static async Task KhoiTaoDuLieuTinhBomMauAsync(EmanDbContext dbContext)
    {
        var deTai = new DeTai
        {
            HeSanPhamId = HeSanPham66Id,
            MaDeTai = "20",
            TenDeTai = "Đề tài 20 phục vụ tính B.O.M màu"
        };
        var hinhDang = new HinhDang
        {
            MaHinhDang = "HD-TINH-MAU",
            TenHinhDang = "Hình dáng tính B.O.M màu"
        };
        var nhomM = new NhomM
        {
            PhamViBom = "BOM_MAU",
            MaNhomM = "M-TINH-MAU",
            TenNhomM = "Nhóm M tính B.O.M màu",
            ThuTu = 1000
        };
        var buocLot = new BomMauBuoc
        {
            MaBuoc = "SON-LOT",
            TenBuoc = "Sơn lót"
        };
        var buocMau = new BomMauBuoc
        {
            MaBuoc = "SON-MAU",
            TenBuoc = "Sơn màu"
        };
        var chauInsert = new ChauInsert
        {
            MaChauInsert = "INSERT-TINH-MAU",
            TenChauInsert = "Chậu insert tính B.O.M màu"
        };

        dbContext.AddRange(deTai, hinhDang, nhomM, buocLot, buocMau, chauInsert);
        await dbContext.SaveChangesAsync();

        var mauSac = new MauSac
        {
            HeSanPhamId = HeSanPham66Id,
            DeTaiId = deTai.Id,
            MaMau = "02",
            TenMau = "Màu 02",
            MaCotTho = "B"
        };
        var maHang = new MaHang
        {
            MaHangCode = "66-52220-01",
            DienTich = 1.25m,
            HinhDangBomMauId = hinhDang.Id,
            HinhDangBomThoId = hinhDang.Id,
            LoaiMaHang = "SAN_PHAM"
        };
        var maHangCotTho = new MaHang
        {
            MaHangCode = "66-52220-01-B",
            DienTich = 1.25m,
            HinhDangBomThoId = hinhDang.Id,
            LoaiMaHang = "COT_THO"
        };

        dbContext.AddRange(mauSac, maHang, maHangCotTho);
        await dbContext.SaveChangesAsync();

        var quyTacNhomM = new QuyTacNhomM
        {
            HinhDangId = hinhDang.Id,
            DienTichTu = 0m,
            DienTichDen = 10m,
            BaoGomTu = true,
            BaoGomDen = true,
            NhomMId = nhomM.Id
        };
        var buocNhomLot = new BuocNhomTheoMau
        {
            HeSanPhamId = HeSanPham66Id,
            MauSacId = mauSac.Id,
            MaBuoc = buocLot.MaBuoc,
            TenBuoc = buocLot.TenBuoc,
            MaHonHopId = 8101,
            MaHonHop = "HH-LOT-01"
        };
        var buocNhomMau = new BuocNhomTheoMau
        {
            HeSanPhamId = HeSanPham66Id,
            MauSacId = mauSac.Id,
            MaBuoc = buocMau.MaBuoc,
            TenBuoc = buocMau.TenBuoc,
            MaHonHopId = 8102,
            MaHonHop = "HH-MAU-02"
        };

        dbContext.AddRange(quyTacNhomM, buocNhomLot, buocNhomMau);
        await dbContext.SaveChangesAsync();

        dbContext.AddRange(
            new BomMauDinhMucNhomM
            {
                BuocNhomMauId = buocNhomLot.Id,
                NhomMId = nhomM.Id,
                MaNhomM = nhomM.MaNhomM,
                DinhMuc = 0.8m
            },
            new BomMauDinhMucNhomM
            {
                BuocNhomMauId = buocNhomMau.Id,
                NhomMId = nhomM.Id,
                MaNhomM = nhomM.MaNhomM,
                DinhMuc = 0.5m
            },
            new BomMauHeSoDeTai
            {
                HeSanPhamId = HeSanPham66Id,
                MaHe = "66",
                DeTaiId = deTai.Id,
                MaDeTai = deTai.MaDeTai,
                TenDeTai = deTai.TenDeTai,
                BuocId = buocLot.Id,
                TenBuoc = buocLot.TenBuoc,
                HeSo = 1.1m
            },
            new BomMauHeSoDeTai
            {
                HeSanPhamId = HeSanPham66Id,
                MaHe = "66",
                DeTaiId = deTai.Id,
                MaDeTai = deTai.MaDeTai,
                TenDeTai = deTai.TenDeTai,
                BuocId = buocMau.Id,
                TenBuoc = buocMau.TenBuoc,
                HeSo = 1.0m
            },
            new BomMauHeSoMau
            {
                HeSanPhamId = HeSanPham66Id,
                MaHe = "66",
                DeTaiId = deTai.Id,
                MaDeTai = deTai.MaDeTai,
                MauSacId = mauSac.Id,
                MaMau = mauSac.MaMau,
                TenMau = mauSac.TenMau,
                BuocId = buocLot.Id,
                TenBuoc = buocLot.TenBuoc,
                HeSo = 1.2m
            },
            new BomMauHeSoMau
            {
                HeSanPhamId = HeSanPham66Id,
                MaHe = "66",
                DeTaiId = deTai.Id,
                MaDeTai = deTai.MaDeTai,
                MauSacId = mauSac.Id,
                MaMau = mauSac.MaMau,
                TenMau = mauSac.TenMau,
                BuocId = buocMau.Id,
                TenBuoc = buocMau.TenBuoc,
                HeSo = 0.9m
            },
            new BomMaHangChauInsert
            {
                MaHangId = maHang.Id,
                MaHang = maHang.MaHangCode,
                ChauInsertId = chauInsert.Id,
                MaChauInsert = chauInsert.MaChauInsert,
                SoLuong = 2
            },
            new BomMaHangPhen
            {
                MaHangId = maHang.Id,
                MaHang = maHang.MaHangCode,
                MaHangPhen = "66-52220-01-PHEN"
            });

        await dbContext.SaveChangesAsync();
    }


    private static VatTu TaoVatTuBom(Guid id, string ma, string ten)
        => new()
        {
            Id = id,
            MaVatTu = ma,
            TenVatTu = ten,
            DonViTinhId = DonViTinhHoatDongId,
            NhomVatTuId = NhomVatTuHoatDongId,
            PhuongThucCungUng = PhuongThucCungUngVatTu.ChiTuSanXuat,
            HanSuDungNgay = 0,
            TrangThai = TrangThaiHoatDong.HoatDong,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };

    private static Kho TaoKho(Guid id, string ma, string ten, bool hangTon, TrangThaiHoatDong trangThai)
        => new()
        {
            Id = id,
            MaKho = ma,
            TenKho = ten,
            HangTon = hangTon,
            HangTru = false,
            TrangThai = trangThai,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };

    private static PhanXuong TaoPhanXuong(Guid id, string ma, string ten, TrangThaiHoatDong trangThai)
        => new()
        {
            Id = id,
            MaPhanXuong = ma,
            TenPhanXuong = ten,
            TrangThai = trangThai,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };

    private static ThueSanPham TaoThue(Guid id, string ma, string ten, decimal thueSuat, TrangThaiHoatDong trangThai)
        => new()
        {
            Id = id,
            MaThue = ma,
            TenThue = ten,
            ThueSuat = thueSuat,
            TrangThai = trangThai,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };

    private static DoiTacKinhDoanh TaoDoiTac(
        Guid id, string ma, string ten, bool laNhaCungCap, TrangThaiHoatDong trangThai)
        => new()
        {
            Id = id,
            MaDoiTac = ma,
            TenDoiTac = ten,
            LoaiDoiTacId = LoaiDoiTacId,
            LaNhaCungCap = laNhaCungCap,
            TrangThai = trangThai,
            RowVersion = Convert.FromBase64String(RowVersionHopLe)
        };
}
