using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Interfaces;
using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Domain.Modules.MasterData.BusinessPartners.Enums;
using PhienBanBangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.PhienBanBangGia;

namespace Eman.Application.Modules.MasterData.BusinessPartners.PhienBanBangGia.Services;

public sealed class PhienBanBangGiaService(
    IPhienBanBangGiaRepository repository,
    IBangGiaRepository bangGiaRepository,
    IUnitOfWork unitOfWork) : IPhienBanBangGiaService
{
    public async Task<PagedResult<PhienBanBangGiaDto>> LayDanhSachAsync(
        BoLocPhienBanBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiPhienBanBangGia?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.BangGiaId,
            trangThai,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<PhienBanBangGiaDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PhienBanBangGiaDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản bảng giá.");

        return ChuyenDto(entity);
    }

    public async Task<PhienBanBangGiaDto> TaoMoiAsync(
        TaoPhienBanBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        await KiemTraBangGiaAsync(request.BangGiaId, cancellationToken);
        KiemTraKhoangNgay(request.TuNgay, request.DenNgay);

        if (await repository.TonTaiSoPhienBanAsync(
                request.BangGiaId,
                request.SoPhienBan,
                null,
                cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Số phiên bản {request.SoPhienBan} đã tồn tại trong bảng giá.");
        }

        if (await repository.CoKhoangThoiGianChongLapAsync(
                request.BangGiaId,
                request.TuNgay,
                request.DenNgay,
                null,
                cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Khoảng thời gian áp dụng bị trùng với phiên bản bảng giá khác.");
        }

        var entity = new PhienBanBangGiaEntity
        {
            BangGiaId = request.BangGiaId,
            SoPhienBan = request.SoPhienBan,
            TuNgay = request.TuNgay,
            DenNgay = request.DenNgay,
            TrangThai = TrangThaiPhienBanBangGia.SoanThao
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<PhienBanBangGiaDto> CapNhatAsync(
        Guid id,
        CapNhatPhienBanBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản bảng giá.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        if (entity.TrangThai != TrangThaiPhienBanBangGia.SoanThao)
        {
            throw new QuyTacNghiepVuException(
                "Chỉ phiên bản đang soạn thảo mới được cập nhật.");
        }

        KiemTraKhoangNgay(request.TuNgay, request.DenNgay);

        if (await repository.TonTaiSoPhienBanAsync(
                entity.BangGiaId,
                request.SoPhienBan,
                id,
                cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Số phiên bản {request.SoPhienBan} đã tồn tại trong bảng giá.");
        }

        if (await repository.CoKhoangThoiGianChongLapAsync(
                entity.BangGiaId,
                request.TuNgay,
                request.DenNgay,
                id,
                cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Khoảng thời gian áp dụng bị trùng với phiên bản bảng giá khác.");
        }

        entity.SoPhienBan = request.SoPhienBan;
        entity.TuNgay = request.TuNgay;
        entity.DenNgay = request.DenNgay;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<PhienBanBangGiaDto> HieuLucAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản bảng giá.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (entity.TrangThai != TrangThaiPhienBanBangGia.SoanThao)
        {
            throw new QuyTacNghiepVuException(
                "Chỉ phiên bản đang soạn thảo mới được hiệu lực.");
        }

        await KiemTraBangGiaAsync(entity.BangGiaId, cancellationToken);

        if (await repository.CoPhienBanDangHieuLucAsync(
                entity.BangGiaId,
                entity.Id,
                cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Bảng giá đang có một phiên bản hiệu lực. Hãy kết thúc phiên bản đó trước.");
        }

        entity.TrangThai = TrangThaiPhienBanBangGia.HieuLuc;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<PhienBanBangGiaDto> HetHieuLucAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản bảng giá.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (entity.TrangThai != TrangThaiPhienBanBangGia.HieuLuc)
        {
            throw new QuyTacNghiepVuException(
                "Chỉ phiên bản đang hiệu lực mới được kết thúc hiệu lực.");
        }

        entity.TrangThai = TrangThaiPhienBanBangGia.HetHieuLuc;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<PhienBanBangGiaDto> HuyAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản bảng giá.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (entity.TrangThai != TrangThaiPhienBanBangGia.SoanThao)
        {
            throw new QuyTacNghiepVuException(
                "Chỉ phiên bản đang soạn thảo mới được hủy.");
        }

        entity.TrangThai = TrangThaiPhienBanBangGia.Huy;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phiên bản bảng giá.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (entity.TrangThai is not (
            TrangThaiPhienBanBangGia.SoanThao or TrangThaiPhienBanBangGia.Huy))
        {
            throw new QuyTacNghiepVuException(
                "Chỉ phiên bản soạn thảo hoặc đã hủy mới được xóa.");
        }

        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task KiemTraBangGiaAsync(
        Guid bangGiaId,
        CancellationToken cancellationToken)
    {
        if (bangGiaId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException("Bảng giá là bắt buộc.");
        }

        var bangGia = await bangGiaRepository.LayTheoIdAsync(
            bangGiaId,
            false,
            cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bảng giá.");

        if (bangGia.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            throw new QuyTacNghiepVuException("Bảng giá đã ngừng hoạt động.");
        }
    }

    private static void KiemTraKhoangNgay(DateOnly tuNgay, DateOnly? denNgay)
    {
        if (tuNgay == default)
        {
            throw new QuyTacNghiepVuException("Ngày bắt đầu áp dụng là bắt buộc.");
        }

        if (denNgay.HasValue && denNgay.Value < tuNgay)
        {
            throw new QuyTacNghiepVuException(
                "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
        }
    }

    private static PhienBanBangGiaDto ChuyenDto(PhienBanBangGiaEntity entity)
        => new(
            entity.Id,
            entity.BangGiaId,
            entity.BangGia.MaBangGia,
            entity.BangGia.TenBangGia,
            entity.SoPhienBan,
            entity.TuNgay,
            entity.DenNgay,
            (byte)entity.TrangThai,
            LayTenTrangThai(entity.TrangThai),
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));

    private static string LayTenTrangThai(TrangThaiPhienBanBangGia trangThai)
        => trangThai switch
        {
            TrangThaiPhienBanBangGia.SoanThao => "Soạn thảo",
            TrangThaiPhienBanBangGia.HieuLuc => "Hiệu lực",
            TrangThaiPhienBanBangGia.HetHieuLuc => "Hết hiệu lực",
            TrangThaiPhienBanBangGia.Huy => "Hủy",
            _ => "Không xác định"
        };
}
