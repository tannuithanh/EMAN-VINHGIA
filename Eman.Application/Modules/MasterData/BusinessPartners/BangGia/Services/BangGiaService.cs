using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Interfaces;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;
using Eman.Domain.Common.Enums;
using BangGiaEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.BangGia;

namespace Eman.Application.Modules.MasterData.BusinessPartners.BangGia.Services;

public sealed class BangGiaService(
    IBangGiaRepository repository,
    IDoiTacKinhDoanhRepository doiTacRepository,
    IUnitOfWork unitOfWork) : IBangGiaService
{
    public async Task<PagedResult<BangGiaDto>> LayDanhSachAsync(
        BoLocBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword,
            request.DoiTacKinhDoanhId,
            trangThai,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<BangGiaDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BangGiaDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bảng giá.");

        return ChuyenDto(entity);
    }

    public async Task<BangGiaDto> TaoMoiAsync(
        TaoBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        await KiemTraNhaCungCapAsync(request.DoiTacKinhDoanhId, cancellationToken);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaBangGia);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã bảng giá '{ma}' đã tồn tại.");
        }

        var entity = new BangGiaEntity
        {
            MaBangGia = ma,
            TenBangGia = ChuoiHelper.ChuanHoaBatBuoc(request.TenBangGia),
            DoiTacKinhDoanhId = request.DoiTacKinhDoanhId,
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BangGiaDto> CapNhatAsync(
        Guid id,
        CapNhatBangGiaRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bảng giá.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        await KiemTraNhaCungCapAsync(request.DoiTacKinhDoanhId, cancellationToken);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaBangGia);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã bảng giá '{ma}' đã tồn tại.");
        }

        entity.MaBangGia = ma;
        entity.TenBangGia = ChuoiHelper.ChuanHoaBatBuoc(request.TenBangGia);
        entity.DoiTacKinhDoanhId = request.DoiTacKinhDoanhId;
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
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
            ?? throw new KhongTimThayException("Không tìm thấy bảng giá.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (await repository.CoPhienBanAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể xóa bảng giá vì đã có phiên bản bảng giá.");
        }

        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task KiemTraNhaCungCapAsync(
        Guid doiTacId,
        CancellationToken cancellationToken)
    {
        if (doiTacId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException("Nhà cung cấp là bắt buộc.");
        }

        var doiTac = await doiTacRepository.LayTheoIdAsync(doiTacId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đối tác kinh doanh.");

        if (!doiTac.LaNhaCungCap)
        {
            throw new QuyTacNghiepVuException(
                "Chỉ đối tác được đánh dấu là nhà cung cấp mới được tạo bảng giá.");
        }

        if (doiTac.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            throw new QuyTacNghiepVuException("Nhà cung cấp đã ngừng hoạt động.");
        }
    }

    private static BangGiaDto ChuyenDto(BangGiaEntity entity)
        => new(
            entity.Id,
            entity.MaBangGia,
            entity.TenBangGia,
            entity.DoiTacKinhDoanhId,
            entity.DoiTacKinhDoanh.MaDoiTac,
            entity.DoiTacKinhDoanh.TenDoiTac,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
