using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Dtos;
using Eman.Application.Modules.MasterData.Production.PhanXuong.Interfaces;
using Eman.Domain.Common.Enums;
using PhanXuongEntity = Eman.Domain.Modules.MasterData.Production.Entities.PhanXuong;

namespace Eman.Application.Modules.MasterData.Production.PhanXuong.Services;

public sealed class PhanXuongService(
    IPhanXuongRepository repository,
    IUnitOfWork unitOfWork) : IPhanXuongService
{
    public async Task<PagedResult<PhanXuongDto>> LayDanhSachAsync(
        BoLocPhanXuongRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword, trangThai, request.Page, request.PageSize, cancellationToken);

        return new PagedResult<PhanXuongDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PhanXuongDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phân xưởng.");
        return ChuyenDto(entity);
    }

    public async Task<PhanXuongDto> TaoMoiAsync(
        TaoPhanXuongRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaPhanXuong);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã phân xưởng '{ma}' đã tồn tại.");
        }

        var entity = new PhanXuongEntity
        {
            MaPhanXuong = ma,
            TenPhanXuong = ChuoiHelper.ChuanHoaBatBuoc(request.TenPhanXuong),
            MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa),
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<PhanXuongDto> CapNhatAsync(
        Guid id,
        CapNhatPhanXuongRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phân xưởng.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var ma = ChuoiHelper.ChuanHoaMa(request.MaPhanXuong);

        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã phân xưởng '{ma}' đã tồn tại.");
        }

        entity.MaPhanXuong = ma;
        entity.TenPhanXuong = ChuoiHelper.ChuanHoaBatBuoc(request.TenPhanXuong);
        entity.MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phân xưởng.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static PhanXuongDto ChuyenDto(PhanXuongEntity entity)
        => new(
            entity.Id,
            entity.MaPhanXuong,
            entity.TenPhanXuong,
            entity.MoTa,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
