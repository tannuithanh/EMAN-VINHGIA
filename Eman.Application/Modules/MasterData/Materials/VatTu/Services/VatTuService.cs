using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;
using Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;
using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Interfaces;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;
using Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.Materials.Entities;
using Eman.Domain.Modules.MasterData.Materials.Enums;
using PhanXuongEntity = Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong;
using VatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.VatTu;

namespace Eman.Application.Modules.MasterData.Materials.VatTu.Services;

public sealed class VatTuService(
    IVatTuRepository repository,
    IDonViTinhRepository donViTinhRepository,
    INhomVatTuRepository nhomVatTuRepository,
    ICoSoMuaVatTuRepository coSoMuaVatTuRepository,
    IDoiTacKinhDoanhRepository doiTacKinhDoanhRepository,
    IThueSanPhamRepository thueSanPhamRepository,
    IKhoRepository khoRepository,
    IUnitOfWork unitOfWork) : IVatTuService
{
    public async Task<PagedResult<VatTuDto>> LayDanhSachAsync(
        BoLocVatTuRequest request,
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

        var (items, totalCount) = await repository.LayDanhSachAsync(
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
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<VatTuDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<VatTuDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy vật tư."));

    public async Task<VatTuDto> TaoMoiAsync(
        TaoVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaVatTu);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã vật tư '{ma}' đã tồn tại.");
        }

        var hanSuDungNgay = KiemTraHanSuDung(request.HanSuDungNgay);
        var tonToiThieu = KiemTraTonToiThieu(request.TonToiThieu);
        var phamVi = request.PhamViSuDung.HasValue
            ? (PhamViSuDungVatTu?)request.PhamViSuDung.Value
            : null;
        var phuongThuc = (PhuongThucCungUngVatTu)request.PhuongThucCungUng;
        var khoLuuTruId = ChuanHoaGuidTuyChon(request.KhoLuuTruId);
        var phanXuongs = await KiemTraDanhMucAsync(
            request.DonViTinhId,
            request.NhomVatTuId,
            phamVi,
            request.PhanXuongIds ?? Array.Empty<Guid>(),
            phuongThuc,
            request.CoSoMuaVatTuId,
            request.NhaCungCapMacDinhId,
            request.NgayMuaHang,
            request.Moq,
            request.ThueVatId,
            khoLuuTruId,
            cancellationToken);

        var thongTinMua = ChuanHoaThongTinMua(
            phuongThuc,
            request.CoSoMuaVatTuId,
            request.NhaCungCapMacDinhId,
            request.NgayMuaHang,
            request.Moq,
            request.ThueVatId);
        var nguoiTao = ChuoiHelper.ChuanHoaTuyChon(request.CreatedByMsnv);
        var thoiDiem = DateTime.UtcNow;

        var entity = new VatTuEntity
        {
            MaVatTu = ma,
            TenVatTu = ChuoiHelper.ChuanHoaBatBuoc(request.TenVatTu),
            TenTiengAnh = ChuoiHelper.ChuanHoaTuyChon(request.TenTiengAnh),
            DonViTinhId = request.DonViTinhId,
            QuyCachDongGoi = ChuoiHelper.ChuanHoaTuyChon(request.QuyCachDongGoi),
            PhamViSuDung = phamVi,
            NhomVatTuId = request.NhomVatTuId,
            MucDichSuDung = ChuoiHelper.ChuanHoaTuyChon(request.MucDichSuDung),
            PhuongThucCungUng = phuongThuc,
            CoSoMuaVatTuId = thongTinMua.CoSoMuaVatTuId,
            NhaCungCapMacDinhId = thongTinMua.NhaCungCapMacDinhId,
            NgayMuaHang = thongTinMua.NgayMuaHang,
            HanSuDungNgay = hanSuDungNgay,
            Moq = thongTinMua.Moq,
            ThueVatId = thongTinMua.ThueVatId,
            TonToiThieu = tonToiThieu,
            KhoLuuTruId = khoLuuTruId,
            TrangThai = TrangThaiHoatDong.HoatDong,
            CreatedAt = thoiDiem,
            CreatedByMsnv = nguoiTao
        };

        foreach (var phanXuong in phanXuongs)
        {
            entity.PhanXuongs.Add(new VatTuPhanXuong
            {
                PhanXuongId = phanXuong.Id,
                CreatedAt = thoiDiem,
                CreatedByMsnv = nguoiTao
            });
        }

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<VatTuDto> CapNhatAsync(
        Guid id,
        CapNhatVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy vật tư.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaVatTu);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã vật tư '{ma}' đã tồn tại.");
        }

        var hanSuDungNgay = KiemTraHanSuDung(request.HanSuDungNgay);
        var tonToiThieu = KiemTraTonToiThieu(request.TonToiThieu);
        var phamVi = request.PhamViSuDung.HasValue
            ? (PhamViSuDungVatTu?)request.PhamViSuDung.Value
            : null;
        var phuongThuc = (PhuongThucCungUngVatTu)request.PhuongThucCungUng;
        var khoLuuTruId = ChuanHoaGuidTuyChon(request.KhoLuuTruId);
        var trangThai = (TrangThaiHoatDong)request.TrangThai;
        if (!Enum.IsDefined(trangThai))
        {
            throw new QuyTacNghiepVuException("Trạng thái vật tư không hợp lệ.");
        }

        var phanXuongs = await KiemTraDanhMucAsync(
            request.DonViTinhId,
            request.NhomVatTuId,
            phamVi,
            request.PhanXuongIds ?? Array.Empty<Guid>(),
            phuongThuc,
            request.CoSoMuaVatTuId,
            request.NhaCungCapMacDinhId,
            request.NgayMuaHang,
            request.Moq,
            request.ThueVatId,
            khoLuuTruId,
            cancellationToken);
        var thongTinMua = ChuanHoaThongTinMua(
            phuongThuc,
            request.CoSoMuaVatTuId,
            request.NhaCungCapMacDinhId,
            request.NgayMuaHang,
            request.Moq,
            request.ThueVatId);
        var nguoiCapNhat = ChuoiHelper.ChuanHoaTuyChon(request.UpdatedByMsnv);
        var thoiDiem = DateTime.UtcNow;

        entity.MaVatTu = ma;
        entity.TenVatTu = ChuoiHelper.ChuanHoaBatBuoc(request.TenVatTu);
        entity.TenTiengAnh = ChuoiHelper.ChuanHoaTuyChon(request.TenTiengAnh);
        entity.DonViTinhId = request.DonViTinhId;
        entity.QuyCachDongGoi = ChuoiHelper.ChuanHoaTuyChon(request.QuyCachDongGoi);
        entity.PhamViSuDung = phamVi;
        entity.NhomVatTuId = request.NhomVatTuId;
        entity.MucDichSuDung = ChuoiHelper.ChuanHoaTuyChon(request.MucDichSuDung);
        entity.PhuongThucCungUng = phuongThuc;
        entity.CoSoMuaVatTuId = thongTinMua.CoSoMuaVatTuId;
        entity.NhaCungCapMacDinhId = thongTinMua.NhaCungCapMacDinhId;
        entity.NgayMuaHang = thongTinMua.NgayMuaHang;
        entity.HanSuDungNgay = hanSuDungNgay;
        entity.Moq = thongTinMua.Moq;
        entity.ThueVatId = thongTinMua.ThueVatId;
        entity.TonToiThieu = tonToiThieu;
        entity.KhoLuuTruId = khoLuuTruId;
        entity.TrangThai = trangThai;
        entity.UpdatedAt = thoiDiem;
        entity.UpdatedByMsnv = nguoiCapNhat;

        var phanXuongIdsMoi = phanXuongs.Select(item => item.Id).ToHashSet();
        var lienKetCanXoa = entity.PhanXuongs
            .Where(item => !phanXuongIdsMoi.Contains(item.PhanXuongId))
            .ToList();
        foreach (var lienKet in lienKetCanXoa)
        {
            entity.PhanXuongs.Remove(lienKet);
        }

        var phanXuongIdsHienTai = entity.PhanXuongs
            .Select(item => item.PhanXuongId)
            .ToHashSet();
        foreach (var phanXuong in phanXuongs.Where(item => !phanXuongIdsHienTai.Contains(item.Id)))
        {
            entity.PhanXuongs.Add(new VatTuPhanXuong
            {
                VatTuId = entity.Id,
                PhanXuongId = phanXuong.Id,
                CreatedAt = thoiDiem,
                CreatedByMsnv = nguoiCapNhat
            });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy vật tư.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<PhanXuongEntity>> KiemTraDanhMucAsync(
        Guid donViTinhId,
        Guid nhomVatTuId,
        PhamViSuDungVatTu? phamVi,
        IReadOnlyCollection<Guid> phanXuongIds,
        PhuongThucCungUngVatTu phuongThuc,
        Guid? coSoMuaVatTuId,
        Guid? nhaCungCapMacDinhId,
        int? ngayMuaHang,
        decimal? moq,
        Guid? thueVatId,
        Guid? khoLuuTruId,
        CancellationToken cancellationToken)
    {
        if (phamVi.HasValue && !Enum.IsDefined(phamVi.Value))
        {
            throw new QuyTacNghiepVuException("Phạm vi sử dụng vật tư không hợp lệ.");
        }
        if (!Enum.IsDefined(phuongThuc))
        {
            throw new QuyTacNghiepVuException("Phương thức cung ứng vật tư không hợp lệ.");
        }
        if (donViTinhId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException("Đơn vị tính là bắt buộc.");
        }
        if (nhomVatTuId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException("Nhóm vật tư là bắt buộc.");
        }
        var donViTinh = await donViTinhRepository.LayTheoIdAsync(donViTinhId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đơn vị tính.");
        KiemTraHoatDong(donViTinh.TrangThai, "Đơn vị tính");

        var nhomVatTu = await nhomVatTuRepository.LayTheoIdAsync(nhomVatTuId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm vật tư.");
        KiemTraHoatDong(nhomVatTu.TrangThai, "Nhóm vật tư");

        if (CoGiaTri(khoLuuTruId))
        {
            var kho = await khoRepository.LayTheoIdAsync(khoLuuTruId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy kho lưu trữ.");
            KiemTraHoatDong(kho.TrangThai, "Kho lưu trữ");
        }

        if (phuongThuc is PhuongThucCungUngVatTu.ChiMuaNgoai
            or PhuongThucCungUngVatTu.MuaHoacTuSanXuat)
        {
            if (!CoGiaTri(coSoMuaVatTuId))
            {
                throw new QuyTacNghiepVuException(
                    "Cơ sở mua vật tư là bắt buộc khi vật tư có phương thức mua ngoài.");
            }
            if (!ngayMuaHang.HasValue || ngayMuaHang.Value < 0)
            {
                throw new QuyTacNghiepVuException(
                    "Thời gian mua hàng là bắt buộc và phải là số nguyên lớn hơn hoặc bằng 0 ngày khi vật tư có phương thức mua ngoài.");
            }
            if (moq.HasValue && moq.Value <= 0)
            {
                throw new QuyTacNghiepVuException(
                    "MOQ phải lớn hơn 0 khi có nhập giá trị.");
            }
            if (!CoGiaTri(thueVatId))
            {
                throw new QuyTacNghiepVuException(
                    "Thuế VAT là bắt buộc khi vật tư có phương thức mua ngoài.");
            }

            var coSoMua = await coSoMuaVatTuRepository.LayTheoIdAsync(
                coSoMuaVatTuId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy cơ sở mua vật tư.");
            KiemTraHoatDong(coSoMua.TrangThai, "Cơ sở mua vật tư");

            var thue = await thueSanPhamRepository.LayTheoIdAsync(
                thueVatId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy thuế VAT.");
            KiemTraHoatDong(thue.TrangThai, "Thuế VAT");

            if (CoGiaTri(nhaCungCapMacDinhId))
            {
                var nhaCungCap = await doiTacKinhDoanhRepository.LayTheoIdAsync(
                    nhaCungCapMacDinhId!.Value, false, cancellationToken)
                    ?? throw new KhongTimThayException("Không tìm thấy nhà cung cấp mặc định.");
                KiemTraHoatDong(nhaCungCap.TrangThai, "Nhà cung cấp mặc định");
                if (!nhaCungCap.LaNhaCungCap)
                {
                    throw new QuyTacNghiepVuException(
                        "Đối tác được chọn không phải là nhà cung cấp.");
                }
            }
        }

        if (!phamVi.HasValue)
        {
            if (phanXuongIds.Count > 0)
            {
                throw new QuyTacNghiepVuException(
                    "Không được chọn phân xưởng khi chưa khai báo phạm vi sử dụng vật tư.");
            }

            return Array.Empty<PhanXuongEntity>();
        }

        if (phamVi == PhamViSuDungVatTu.TatCaPhanXuong)
        {
            if (phanXuongIds.Count > 0)
            {
                throw new QuyTacNghiepVuException(
                    "Vật tư dùng cho tất cả phân xưởng không được chọn phân xưởng cụ thể.");
            }

            return Array.Empty<PhanXuongEntity>();
        }

        var ids = phanXuongIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();
        if (ids.Length == 0)
        {
            throw new QuyTacNghiepVuException(
                "Vui lòng chọn ít nhất một phân xưởng sử dụng vật tư.");
        }
        if (ids.Length != phanXuongIds.Count)
        {
            throw new QuyTacNghiepVuException(
                "Danh sách phân xưởng sử dụng không được để trống hoặc trùng lặp.");
        }

        var phanXuongs = await repository.LayPhanXuongsHoatDongTheoIdsAsync(ids, cancellationToken);
        if (phanXuongs.Count != ids.Length)
        {
            throw new QuyTacNghiepVuException(
                "Có phân xưởng không tồn tại hoặc đã ngừng hoạt động.");
        }
        return phanXuongs;
    }

    private static ThongTinMuaVatTu ChuanHoaThongTinMua(
        PhuongThucCungUngVatTu phuongThuc,
        Guid? coSoMuaVatTuId,
        Guid? nhaCungCapMacDinhId,
        int? ngayMuaHang,
        decimal? moq,
        Guid? thueVatId)
    {
        if (phuongThuc == PhuongThucCungUngVatTu.ChiTuSanXuat)
        {
            return new ThongTinMuaVatTu(null, null, null, null, null);
        }
        return new ThongTinMuaVatTu(
            ChuanHoaGuidTuyChon(coSoMuaVatTuId),
            ChuanHoaGuidTuyChon(nhaCungCapMacDinhId),
            ngayMuaHang,
            moq,
            ChuanHoaGuidTuyChon(thueVatId));
    }

    private static void KiemTraHoatDong(TrangThaiHoatDong trangThai, string tenDanhMuc)
    {
        if (trangThai != TrangThaiHoatDong.HoatDong)
        {
            throw new QuyTacNghiepVuException($"{tenDanhMuc} đã ngừng hoạt động.");
        }
    }

    private static bool CoGiaTri(Guid? id) => id.HasValue && id.Value != Guid.Empty;
    private static Guid? ChuanHoaGuidTuyChon(Guid? id) => CoGiaTri(id) ? id : null;

    private static int KiemTraHanSuDung(int? hanSuDungNgay)
    {
        if (!hanSuDungNgay.HasValue)
        {
            throw new QuyTacNghiepVuException("Hạn sử dụng là bắt buộc.");
        }

        if (hanSuDungNgay.Value < 0)
        {
            throw new QuyTacNghiepVuException("Hạn sử dụng phải lớn hơn hoặc bằng 0 ngày.");
        }

        return hanSuDungNgay.Value;
    }

    private static decimal? KiemTraTonToiThieu(decimal? tonToiThieu)
    {
        if (tonToiThieu.HasValue && tonToiThieu.Value < 0)
        {
            throw new QuyTacNghiepVuException("Tồn tối thiểu phải lớn hơn hoặc bằng 0.");
        }

        return tonToiThieu;
    }

    private static VatTuDto ChuyenDto(VatTuEntity entity) => new()
    {
        Id = entity.Id,
        MaVatTu = entity.MaVatTu,
        TenVatTu = entity.TenVatTu,
        TenTiengAnh = entity.TenTiengAnh,
        DonViTinhId = entity.DonViTinhId,
        MaDonViTinh = entity.DonViTinh.MaDonViTinh,
        TenDonViTinh = entity.DonViTinh.TenDonViTinh,
        KyHieuDonViTinh = entity.DonViTinh.KyHieu,
        QuyCachDongGoi = entity.QuyCachDongGoi,
        PhamViSuDung = entity.PhamViSuDung.HasValue
            ? (byte?)entity.PhamViSuDung.Value
            : null,
        TenPhamViSuDung = LayTenPhamVi(entity.PhamViSuDung),
        PhanXuongs = entity.PhanXuongs
            .OrderBy(item => item.PhanXuong.MaPhanXuong)
            .Select(item => new VatTuPhanXuongDto
            {
                PhanXuongId = item.PhanXuongId,
                MaPhanXuong = item.PhanXuong.MaPhanXuong,
                TenPhanXuong = item.PhanXuong.TenPhanXuong
            })
            .ToList(),
        NhomVatTuId = entity.NhomVatTuId,
        MaNhomVatTu = entity.NhomVatTu.MaNhomVatTu,
        TenNhomVatTu = entity.NhomVatTu.TenNhomVatTu,
        MucDichSuDung = entity.MucDichSuDung,
        PhuongThucCungUng = (byte)entity.PhuongThucCungUng,
        TenPhuongThucCungUng = LayTenPhuongThuc(entity.PhuongThucCungUng),
        CoSoMuaVatTuId = entity.CoSoMuaVatTuId,
        MaCoSoMuaVatTu = entity.CoSoMuaVatTu?.MaCoSoMuaVatTu,
        TenCoSoMuaVatTu = entity.CoSoMuaVatTu?.TenCoSoMuaVatTu,
        NhaCungCapMacDinhId = entity.NhaCungCapMacDinhId,
        MaNhaCungCapMacDinh = entity.NhaCungCapMacDinh?.MaDoiTac,
        TenNhaCungCapMacDinh = entity.NhaCungCapMacDinh?.TenDoiTac,
        NgayMuaHang = entity.NgayMuaHang,
        HanSuDungNgay = entity.HanSuDungNgay,
        Moq = entity.Moq,
        ThueVatId = entity.ThueVatId,
        MaThueVat = entity.ThueVat?.MaThue,
        TenThueVat = entity.ThueVat?.TenThue,
        ThueSuat = entity.ThueVat?.ThueSuat,
        TonToiThieu = entity.TonToiThieu,
        KhoLuuTruId = entity.KhoLuuTruId,
        MaKhoLuuTru = entity.KhoLuuTru?.MaKho,
        TenKhoLuuTru = entity.KhoLuuTru?.TenKho,
        TrangThai = (byte)entity.TrangThai,
        CreatedAt = entity.CreatedAt,
        CreatedByMsnv = entity.CreatedByMsnv,
        UpdatedAt = entity.UpdatedAt,
        UpdatedByMsnv = entity.UpdatedByMsnv,
        RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
    };

    private static string? LayTenPhamVi(PhamViSuDungVatTu? value) => value switch
    {
        PhamViSuDungVatTu.TatCaPhanXuong => "Tất cả phân xưởng",
        PhamViSuDungVatTu.PhanXuongCuThe => "Phân xưởng cụ thể",
        null => null,
        _ => "Không xác định"
    };

    private static string LayTenPhuongThuc(PhuongThucCungUngVatTu value) => value switch
    {
        PhuongThucCungUngVatTu.ChiMuaNgoai => "Chỉ mua ngoài",
        PhuongThucCungUngVatTu.MuaHoacTuSanXuat => "Mua hoặc tự sản xuất",
        PhuongThucCungUngVatTu.ChiTuSanXuat => "Chỉ tự sản xuất",
        _ => "Không xác định"
    };

    private sealed record ThongTinMuaVatTu(
        Guid? CoSoMuaVatTuId,
        Guid? NhaCungCapMacDinhId,
        int? NgayMuaHang,
        decimal? Moq,
        Guid? ThueVatId);
}
