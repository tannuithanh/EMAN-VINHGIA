using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Interfaces;
using Eman.Domain.Common.Enums;
using LoaiDoiTacEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.LoaiDoiTac;

namespace Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Services;

public sealed class LoaiDoiTacService(
    ILoaiDoiTacRepository repository,
    IUnitOfWork unitOfWork) : ILoaiDoiTacService
{
    public async Task<PagedResult<LoaiDoiTacDto>> LayDanhSachAsync(
        BoLocLoaiDoiTacRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword,
            trangThai,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<LoaiDoiTacDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<LoaiDoiTacDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy loại đối tác.");

        return ChuyenDto(entity);
    }

    public async Task<LoaiDoiTacDto> TaoMoiAsync(
        TaoLoaiDoiTacRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaLoaiDoiTac);

        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã loại đối tác '{ma}' đã tồn tại.");
        }

        var entity = new LoaiDoiTacEntity
        {
            MaLoaiDoiTac = ma,
            TenLoaiDoiTac = ChuoiHelper.ChuanHoaBatBuoc(request.TenLoaiDoiTac),
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ChuyenDto(entity);
    }

    public async Task<LoaiDoiTacDto> CapNhatAsync(
        Guid id,
        CapNhatLoaiDoiTacRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy loại đối tác.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaLoaiDoiTac);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã loại đối tác '{ma}' đã tồn tại.");
        }

        entity.MaLoaiDoiTac = ma;
        entity.TenLoaiDoiTac = ChuoiHelper.ChuanHoaBatBuoc(request.TenLoaiDoiTac);
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy loại đối tác.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (await repository.DangDuocSuDungAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể xóa loại đối tác vì đã có đối tác kinh doanh sử dụng.");
        }

        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static LoaiDoiTacDto ChuyenDto(LoaiDoiTacEntity entity)
        => new(
            entity.Id,
            entity.MaLoaiDoiTac,
            entity.TenLoaiDoiTac,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
