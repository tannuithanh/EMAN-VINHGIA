using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;
using Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;
using Eman.Application.Modules.MasterData.Products.SanPham.Dtos;
using Eman.Application.Modules.MasterData.Products.SanPham.Interfaces;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;
using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Interfaces;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Interfaces;
using Eman.Domain.Common.Enums;
using SanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.SanPham;

namespace Eman.Application.Modules.MasterData.Products.SanPham.Services;

public sealed class SanPhamService(
    ISanPhamRepository repository,
    IDonViTinhRepository donViTinhRepository,
    INhomNangLucRepository nhomNangLucRepository,
    IKhoRepository khoRepository,
    IPhanXuongRepository phanXuongRepository,
    IThueSanPhamRepository thueSanPhamRepository,
    IUnitOfWork unitOfWork) : ISanPhamService
{
    public async Task<PagedResult<SanPhamDto>> LayDanhSachAsync(
        BoLocSanPhamRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword,
            request.DonViTinhId,
            request.NhomNangLucId,
            request.KhoMacDinhId,
            request.KhoTonId,
            request.XuongMacDinhId,
            request.ThueId,
            request.LaBanThanhPham,
            request.NoiGiaoHang,
            trangThai,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<SanPhamDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<SanPhamDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy sản phẩm.");

        return ChuyenDto(entity);
    }

    public async Task<SanPhamDto> TaoMoiAsync(
        TaoSanPhamRequest request,
        CancellationToken cancellationToken)
    {
        await KiemTraDanhMucAsync(
            request.DonViTinhId,
            request.NhomNangLucId,
            request.KhoMacDinhId,
            request.KhoTonId,
            request.XuongMacDinhId,
            request.ThueId,
            cancellationToken);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaSanPham);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã sản phẩm '{ma}' đã tồn tại.");
        }

        var id = request.Id.HasValue && request.Id.Value != Guid.Empty
            ? request.Id.Value
            : Guid.NewGuid();

        if (await repository.TonTaiIdAsync(id, cancellationToken))
        {
            throw new XungDotDuLieuException($"ID sản phẩm '{id}' đã tồn tại.");
        }

        var entity = new SanPhamEntity
        {
            Id = id,
            MaSanPham = ma,
            MoTaTiengViet = ChuoiHelper.ChuanHoaBatBuoc(request.MoTaTiengViet),
            MoTaTiengAnh = ChuoiHelper.ChuanHoaTuyChon(request.MoTaTiengAnh),
            DonViTinhId = request.DonViTinhId,
            NhomNangLucId = ChuanHoaGuidTuyChon(request.NhomNangLucId),
            ChieuDaiCm = request.ChieuDaiCm,
            ChieuRongCm = request.ChieuRongCm,
            ChieuCaoCm = request.ChieuCaoCm,
            TrongLuong = request.TrongLuong,
            DienTich = request.DienTich,
            DoKho = request.DoKho,
            HeSoTiTrong = request.HeSoTiTrong,
            CbmMacDinh = request.CbmMacDinh,
            KhoMacDinhId = ChuanHoaGuidTuyChon(request.KhoMacDinhId),
            KhoTonId = ChuanHoaGuidTuyChon(request.KhoTonId),
            XuongMacDinhId = ChuanHoaGuidTuyChon(request.XuongMacDinhId),
            ThueId = ChuanHoaGuidTuyChon(request.ThueId),
            LaBanThanhPham = request.LaBanThanhPham,
            NoiGiaoHang = ChuoiHelper.ChuanHoaTuyChon(request.NoiGiaoHang),
            GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu),
            TrangThai = TrangThaiHoatDong.HoatDong,
            CreatedAt = DateTime.UtcNow,
            CreatedByMsnv = ChuoiHelper.ChuanHoaTuyChon(request.CreatedByMsnv)
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<SanPhamDto> CapNhatAsync(
        Guid id,
        CapNhatSanPhamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy sản phẩm.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        await KiemTraDanhMucAsync(
            request.DonViTinhId,
            request.NhomNangLucId,
            request.KhoMacDinhId,
            request.KhoTonId,
            request.XuongMacDinhId,
            request.ThueId,
            cancellationToken);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaSanPham);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã sản phẩm '{ma}' đã tồn tại.");
        }

        entity.MaSanPham = ma;
        entity.MoTaTiengViet = ChuoiHelper.ChuanHoaBatBuoc(request.MoTaTiengViet);
        entity.MoTaTiengAnh = ChuoiHelper.ChuanHoaTuyChon(request.MoTaTiengAnh);
        entity.DonViTinhId = request.DonViTinhId;
        entity.NhomNangLucId = ChuanHoaGuidTuyChon(request.NhomNangLucId);
        entity.ChieuDaiCm = request.ChieuDaiCm;
        entity.ChieuRongCm = request.ChieuRongCm;
        entity.ChieuCaoCm = request.ChieuCaoCm;
        entity.TrongLuong = request.TrongLuong;
        entity.DienTich = request.DienTich;
        entity.DoKho = request.DoKho;
        entity.HeSoTiTrong = request.HeSoTiTrong;
        entity.CbmMacDinh = request.CbmMacDinh;
        entity.KhoMacDinhId = ChuanHoaGuidTuyChon(request.KhoMacDinhId);
        entity.KhoTonId = ChuanHoaGuidTuyChon(request.KhoTonId);
        entity.XuongMacDinhId = ChuanHoaGuidTuyChon(request.XuongMacDinhId);
        entity.ThueId = ChuanHoaGuidTuyChon(request.ThueId);
        entity.LaBanThanhPham = request.LaBanThanhPham;
        entity.NoiGiaoHang = ChuoiHelper.ChuanHoaTuyChon(request.NoiGiaoHang);
        entity.GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByMsnv = ChuoiHelper.ChuanHoaTuyChon(request.UpdatedByMsnv);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy sản phẩm.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task KiemTraDanhMucAsync(
        Guid donViTinhId,
        Guid? nhomNangLucId,
        Guid? khoMacDinhId,
        Guid? khoTonId,
        Guid? xuongMacDinhId,
        Guid? thueId,
        CancellationToken cancellationToken)
    {
        if (donViTinhId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException("Đơn vị tính là bắt buộc.");
        }

        var donViTinh = await donViTinhRepository.LayTheoIdAsync(
            donViTinhId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đơn vị tính.");
        KiemTraDangHoatDong(donViTinh.TrangThai, "Đơn vị tính");

        if (CoGiaTri(nhomNangLucId))
        {
            var nhomNangLuc = await nhomNangLucRepository.LayTheoIdAsync(
                nhomNangLucId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy nhóm năng lực.");
            KiemTraDangHoatDong(nhomNangLuc.TrangThai, "Nhóm năng lực");
        }

        if (CoGiaTri(khoMacDinhId))
        {
            var khoMacDinh = await khoRepository.LayTheoIdAsync(
                khoMacDinhId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy kho mặc định.");
            KiemTraDangHoatDong(khoMacDinh.TrangThai, "Kho mặc định");
        }

        if (CoGiaTri(khoTonId))
        {
            var khoTon = await khoRepository.LayTheoIdAsync(
                khoTonId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy kho tồn.");
            KiemTraDangHoatDong(khoTon.TrangThai, "Kho tồn");

            if (!khoTon.HangTon)
            {
                throw new QuyTacNghiepVuException(
                    "Kho tồn được chọn phải là kho có đánh dấu hàng tồn.");
            }
        }

        if (CoGiaTri(khoMacDinhId) &&
            CoGiaTri(khoTonId) &&
            khoMacDinhId == khoTonId)
        {
            throw new QuyTacNghiepVuException(
                "Kho mặc định và Kho tồn không được giống nhau.");
        }

        if (CoGiaTri(xuongMacDinhId))
        {
            var xuongMacDinh = await phanXuongRepository.LayTheoIdAsync(
                xuongMacDinhId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy xưởng mặc định.");
            KiemTraDangHoatDong(xuongMacDinh.TrangThai, "Xưởng mặc định");
        }

        if (CoGiaTri(thueId))
        {
            var thue = await thueSanPhamRepository.LayTheoIdAsync(
                thueId!.Value, false, cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy thuế sản phẩm.");
            KiemTraDangHoatDong(thue.TrangThai, "Thuế sản phẩm");
        }
    }

    private static void KiemTraDangHoatDong(
        TrangThaiHoatDong trangThai,
        string tenDanhMuc)
    {
        if (trangThai != TrangThaiHoatDong.HoatDong)
        {
            throw new QuyTacNghiepVuException($"{tenDanhMuc} đã ngừng hoạt động.");
        }
    }

    private static bool CoGiaTri(Guid? id)
        => id.HasValue && id.Value != Guid.Empty;

    private static Guid? ChuanHoaGuidTuyChon(Guid? id)
        => CoGiaTri(id) ? id : null;

    private static SanPhamDto ChuyenDto(SanPhamEntity entity)
        => new()
        {
            Id = entity.Id,
            MaSanPham = entity.MaSanPham,
            MoTaTiengViet = entity.MoTaTiengViet,
            MoTaTiengAnh = entity.MoTaTiengAnh,
            DonViTinhId = entity.DonViTinhId,
            MaDonViTinh = entity.DonViTinh.MaDonViTinh,
            TenDonViTinh = entity.DonViTinh.TenDonViTinh,
            KyHieuDonViTinh = entity.DonViTinh.KyHieu,
            NhomNangLucId = entity.NhomNangLucId,
            MaNhomNangLuc = entity.NhomNangLuc?.MaNhomNangLuc,
            TenNhomNangLuc = entity.NhomNangLuc?.TenNhomNangLuc,
            ChieuDaiCm = entity.ChieuDaiCm,
            ChieuRongCm = entity.ChieuRongCm,
            ChieuCaoCm = entity.ChieuCaoCm,
            TrongLuong = entity.TrongLuong,
            DienTich = entity.DienTich,
            DoKho = entity.DoKho,
            HeSoTiTrong = entity.HeSoTiTrong,
            CbmMacDinh = entity.CbmMacDinh,
            KhoMacDinhId = entity.KhoMacDinhId,
            MaKhoMacDinh = entity.KhoMacDinh?.MaKho,
            TenKhoMacDinh = entity.KhoMacDinh?.TenKho,
            KhoTonId = entity.KhoTonId,
            MaKhoTon = entity.KhoTon?.MaKho,
            TenKhoTon = entity.KhoTon?.TenKho,
            XuongMacDinhId = entity.XuongMacDinhId,
            MaXuongMacDinh = entity.XuongMacDinh?.MaPhanXuong,
            TenXuongMacDinh = entity.XuongMacDinh?.TenPhanXuong,
            ThueId = entity.ThueId,
            MaThue = entity.Thue?.MaThue,
            TenThue = entity.Thue?.TenThue,
            ThueSuat = entity.Thue?.ThueSuat,
            LaBanThanhPham = entity.LaBanThanhPham,
            NoiGiaoHang = entity.NoiGiaoHang,
            GhiChu = entity.GhiChu,
            TrangThai = (byte)entity.TrangThai,
            CreatedAt = entity.CreatedAt,
            CreatedByMsnv = entity.CreatedByMsnv,
            UpdatedAt = entity.UpdatedAt,
            UpdatedByMsnv = entity.UpdatedByMsnv,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
