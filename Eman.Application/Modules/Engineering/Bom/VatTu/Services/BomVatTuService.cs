using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.VatTu.Dtos;
using Eman.Application.Modules.Engineering.Bom.VatTu.Interfaces;
using Eman.Application.Modules.MasterData.Materials.VatTu.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;
using Eman.Domain.Modules.Engineering.Bom.VatTu.Enums;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Services;

public sealed class BomVatTuService(
    IBomVatTuRepository repository,
    IVatTuRepository vatTuRepository,
    IUnitOfWork unitOfWork) : IBomVatTuService
{
    public async Task<PagedResult<BomVatTuPhienBanDto>> LayDanhSachAsync(
        BoLocBomVatTuPhienBanRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiBomVatTuPhienBan?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachPhienBanAsync(
            request.VatTuId,
            request.Keyword,
            trangThai,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<BomVatTuPhienBanDto>
        {
            Items = items.Select(ChuyenPhienBanDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BomVatTuPhienBanDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
        => ChuyenPhienBanDto(await repository.LayPhienBanTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản B.O.M vật tư."));

    public async Task<BomVatTuPhienBanDto> TaoPhienBanAsync(
        TaoBomVatTuPhienBanRequest request,
        CancellationToken cancellationToken)
    {
        var vatTu = await KiemTraVatTuHoatDongAsync(request.VatTuId, "vật tư đầu ra", cancellationToken);
        if (await repository.TonTaiSoPhienBanAsync(request.VatTuId, request.SoPhienBan, null, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Vật tư '{vatTu.MaVatTu}' đã có phiên bản B.O.M số {request.SoPhienBan}.");
        }

        var entity = new BomVatTuPhienBan
        {
            VatTuId = request.VatTuId,
            SoPhienBan = request.SoPhienBan,
            TrangThai = TrangThaiBomVatTuPhienBan.Nhap,
            GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu),
            CreatedAt = DateTime.UtcNow,
            CreatedByMsnv = ChuanHoaMsnv(request.CreatedByMsnv)
        };

        await repository.ThemPhienBanAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BomVatTuPhienBanDto> CapNhatPhienBanAsync(
        Guid id,
        CapNhatBomVatTuPhienBanRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayPhienBanTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản B.O.M vật tư.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        KiemTraPhienBanNhap(entity);
        if (await repository.TonTaiSoPhienBanAsync(entity.VatTuId, request.SoPhienBan, id, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Vật tư '{entity.VatTu.MaVatTu}' đã có phiên bản B.O.M số {request.SoPhienBan}.");
        }

        entity.SoPhienBan = request.SoPhienBan;
        entity.GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByMsnv = ChuanHoaMsnv(request.UpdatedByMsnv);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BomVatTuPhienBanDto> HieuLucAsync(
        Guid id,
        string rowVersion,
        string? updatedByMsnv,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayPhienBanTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản B.O.M vật tư.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        KiemTraPhienBanNhap(entity);

        if (entity.VatTu.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            throw new QuyTacNghiepVuException("Vật tư đầu ra đã ngừng hoạt động nên không thể hiệu lực B.O.M.");
        }

        if (entity.ChiTiets.Count == 0)
        {
            throw new QuyTacNghiepVuException("Phiên bản B.O.M phải có ít nhất một vật tư thành phần trước khi hiệu lực.");
        }

        var thanhPhanNgungHoatDong = entity.ChiTiets
            .FirstOrDefault(item => item.VatTuThanhPhan.TrangThai != TrangThaiHoatDong.HoatDong);
        if (thanhPhanNgungHoatDong is not null)
        {
            throw new QuyTacNghiepVuException(
                $"Vật tư thành phần '{thanhPhanNgungHoatDong.VatTuThanhPhan.MaVatTu}' đã ngừng hoạt động.");
        }

        if (await repository.CoPhienBanHieuLucAsync(entity.VatTuId, entity.Id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                $"Vật tư '{entity.VatTu.MaVatTu}' đang có một phiên bản B.O.M hiệu lực. Hãy ngừng phiên bản đó trước.");
        }

        await KiemTraVongLapAsync(entity, cancellationToken);

        entity.TrangThai = TrangThaiBomVatTuPhienBan.HieuLuc;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByMsnv = ChuanHoaMsnv(updatedByMsnv);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BomVatTuPhienBanDto> NgungHieuLucAsync(
        Guid id,
        string rowVersion,
        string? updatedByMsnv,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayPhienBanTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản B.O.M vật tư.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (entity.TrangThai != TrangThaiBomVatTuPhienBan.HieuLuc)
        {
            throw new QuyTacNghiepVuException("Chỉ phiên bản B.O.M đang hiệu lực mới được ngừng hiệu lực.");
        }

        entity.TrangThai = TrangThaiBomVatTuPhienBan.NgungHieuLuc;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByMsnv = ChuanHoaMsnv(updatedByMsnv);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaPhienBanAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayPhienBanTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản B.O.M vật tư.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        KiemTraPhienBanNhap(entity);

        repository.XoaPhienBan(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<BomVatTuChiTietDto> ThemChiTietAsync(
        Guid phienBanId,
        TaoBomVatTuChiTietRequest request,
        CancellationToken cancellationToken)
    {
        var phienBan = await repository.LayPhienBanTheoIdAsync(phienBanId, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản B.O.M vật tư.");

        KiemTraPhienBanNhap(phienBan);
        var vatTuThanhPhan = await KiemTraVatTuHoatDongAsync(request.VatTuThanhPhanId, "vật tư thành phần", cancellationToken);
        KiemTraKhongTuChuaChinhNo(phienBan.VatTuId, request.VatTuThanhPhanId);

        if (await repository.TonTaiThanhPhanAsync(phienBanId, request.VatTuThanhPhanId, null, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Vật tư '{vatTuThanhPhan.MaVatTu}' đã có trong phiên bản B.O.M này.");
        }

        var entity = new BomVatTuChiTiet
        {
            BomVatTuPhienBanId = phienBanId,
            VatTuThanhPhanId = request.VatTuThanhPhanId,
            SoLuong = request.SoLuong,
            ThuTu = request.ThuTu,
            GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu),
            CreatedAt = DateTime.UtcNow,
            CreatedByMsnv = ChuanHoaMsnv(request.CreatedByMsnv)
        };

        await repository.ThemChiTietAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var daTao = await repository.LayChiTietTheoIdAsync(entity.Id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không đọc lại được chi tiết B.O.M vật tư vừa tạo.");
        return ChuyenChiTietDto(daTao);
    }

    public async Task<BomVatTuChiTietDto> CapNhatChiTietAsync(
        Guid id,
        CapNhatBomVatTuChiTietRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayChiTietTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chi tiết B.O.M vật tư.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        KiemTraPhienBanNhap(entity.BomVatTuPhienBan);
        KiemTraKhongTuChuaChinhNo(entity.BomVatTuPhienBan.VatTuId, request.VatTuThanhPhanId);

        var vatTuThanhPhan = await KiemTraVatTuHoatDongAsync(request.VatTuThanhPhanId, "vật tư thành phần", cancellationToken);

        if (await repository.TonTaiThanhPhanAsync(
                entity.BomVatTuPhienBanId,
                request.VatTuThanhPhanId,
                id,
                cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Vật tư '{vatTuThanhPhan.MaVatTu}' đã có trong phiên bản B.O.M này.");
        }

        entity.VatTuThanhPhanId = request.VatTuThanhPhanId;
        entity.SoLuong = request.SoLuong;
        entity.ThuTu = request.ThuTu;
        entity.GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByMsnv = ChuanHoaMsnv(request.UpdatedByMsnv);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var daCapNhat = await repository.LayChiTietTheoIdAsync(entity.Id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không đọc lại được chi tiết B.O.M vật tư vừa cập nhật.");
        return ChuyenChiTietDto(daCapNhat);
    }

    public async Task XoaChiTietAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayChiTietTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chi tiết B.O.M vật tư.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        KiemTraPhienBanNhap(entity.BomVatTuPhienBan);
        repository.XoaChiTiet(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Eman.Domain.Modules.MasterData.Materials.Entities.VatTu> KiemTraVatTuHoatDongAsync(
        Guid vatTuId,
        string tenVaiTro,
        CancellationToken cancellationToken)
    {
        if (vatTuId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException($"{VietHoaDau(tenVaiTro)} là bắt buộc.");
        }

        var vatTu = await vatTuRepository.LayTheoIdAsync(vatTuId, false, cancellationToken)
            ?? throw new KhongTimThayException($"Không tìm thấy {tenVaiTro}.");

        if (vatTu.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            throw new QuyTacNghiepVuException($"{VietHoaDau(tenVaiTro)} '{vatTu.MaVatTu}' đã ngừng hoạt động.");
        }

        return vatTu;
    }

    private async Task KiemTraVongLapAsync(BomVatTuPhienBan phienBan, CancellationToken cancellationToken)
    {
        var quanHes = await repository.LayQuanHeBomHieuLucAsync(cancellationToken);
        var doThi = quanHes
            .Where(item => item.VatTuDauRaId != phienBan.VatTuId)
            .GroupBy(item => item.VatTuDauRaId)
            .ToDictionary(group => group.Key, group => group.Select(item => item.VatTuThanhPhanId).Distinct().ToList());

        foreach (var chiTiet in phienBan.ChiTiets)
        {
            if (CoDuongDi(chiTiet.VatTuThanhPhanId, phienBan.VatTuId, doThi))
            {
                throw new QuyTacNghiepVuException(
                    $"Không thể hiệu lực B.O.M vì vật tư thành phần '{chiTiet.VatTuThanhPhan.MaVatTu}' tạo thành vòng lặp quay lại '{phienBan.VatTu.MaVatTu}'.");
            }
        }
    }

    private static bool CoDuongDi(
        Guid batDau,
        Guid dich,
        IReadOnlyDictionary<Guid, List<Guid>> doThi)
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

    private static void KiemTraPhienBanNhap(BomVatTuPhienBan entity)
    {
        if (entity.TrangThai != TrangThaiBomVatTuPhienBan.Nhap)
        {
            throw new QuyTacNghiepVuException("Chỉ phiên bản B.O.M đang Nháp mới được thay đổi.");
        }
    }

    private static void KiemTraKhongTuChuaChinhNo(Guid vatTuDauRaId, Guid vatTuThanhPhanId)
    {
        if (vatTuDauRaId == vatTuThanhPhanId)
        {
            throw new QuyTacNghiepVuException("Vật tư đầu ra không thể đồng thời là vật tư thành phần của chính B.O.M đó.");
        }
    }

    private static string? ChuanHoaMsnv(string? value)
    {
        var result = ChuoiHelper.ChuanHoaTuyChon(value);
        if (result is { Length: > 50 })
        {
            throw new QuyTacNghiepVuException("Mã nhân viên không được vượt quá 50 ký tự.");
        }
        return result;
    }

    private static string VietHoaDau(string value)
        => string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value[1..];

    private static BomVatTuPhienBanDto ChuyenPhienBanDto(BomVatTuPhienBan entity)
        => new(
            entity.Id,
            entity.VatTuId,
            entity.VatTu.MaVatTu,
            entity.VatTu.TenVatTu,
            entity.VatTu.DonViTinhId,
            entity.VatTu.DonViTinh.MaDonViTinh,
            entity.VatTu.DonViTinh.TenDonViTinh,
            entity.SoPhienBan,
            (byte)entity.TrangThai,
            LayTenTrangThai(entity.TrangThai),
            entity.GhiChu,
            entity.ChiTiets.Count,
            entity.ChiTiets.OrderBy(item => item.ThuTu).ThenBy(item => item.VatTuThanhPhan.MaVatTu)
                .Select(ChuyenChiTietDto).ToList(),
            entity.CreatedAt,
            entity.CreatedByMsnv,
            entity.UpdatedAt,
            entity.UpdatedByMsnv,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));

    private static BomVatTuChiTietDto ChuyenChiTietDto(BomVatTuChiTiet entity)
        => new(
            entity.Id,
            entity.BomVatTuPhienBanId,
            entity.VatTuThanhPhanId,
            entity.VatTuThanhPhan.MaVatTu,
            entity.VatTuThanhPhan.TenVatTu,
            entity.VatTuThanhPhan.DonViTinhId,
            entity.VatTuThanhPhan.DonViTinh.MaDonViTinh,
            entity.VatTuThanhPhan.DonViTinh.TenDonViTinh,
            entity.SoLuong,
            entity.ThuTu,
            entity.GhiChu,
            entity.CreatedAt,
            entity.CreatedByMsnv,
            entity.UpdatedAt,
            entity.UpdatedByMsnv,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));

    private static string LayTenTrangThai(TrangThaiBomVatTuPhienBan trangThai)
        => trangThai switch
        {
            TrangThaiBomVatTuPhienBan.Nhap => "Nháp",
            TrangThaiBomVatTuPhienBan.HieuLuc => "Hiệu lực",
            TrangThaiBomVatTuPhienBan.NgungHieuLuc => "Ngừng hiệu lực",
            _ => "Không xác định"
        };
}
