using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Dtos;
using Eman.Application.Modules.MasterData.Production.NhomNangLuc.Interfaces;
using Eman.Domain.Common.Enums;
using NhomNangLucEntity = Eman.Domain.Modules.MasterData.Production.Entities.NhomNangLuc;

namespace Eman.Application.Modules.MasterData.Production.NhomNangLuc.Services;

public sealed class NhomNangLucService(
    INhomNangLucRepository repository,
    IUnitOfWork unitOfWork) : INhomNangLucService
{
    public async Task<PagedResult<NhomNangLucDto>> LayDanhSachAsync(
        BoLocNhomNangLucRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword, trangThai, request.Page, request.PageSize, cancellationToken);

        return new PagedResult<NhomNangLucDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<NhomNangLucDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm năng lực.");
        return ChuyenDto(entity);
    }

    public async Task<NhomNangLucDto> TaoMoiAsync(
        TaoNhomNangLucRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaNhomNangLuc);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã nhóm năng lực '{ma}' đã tồn tại.");
        }

        var entity = new NhomNangLucEntity
        {
            MaNhomNangLuc = ma,
            TenNhomNangLuc = ChuoiHelper.ChuanHoaBatBuoc(request.TenNhomNangLuc),
            ThoiGianLamHang = request.ThoiGianLamHang,
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<NhomNangLucDto> CapNhatAsync(
        Guid id,
        CapNhatNhomNangLucRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm năng lực.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var ma = ChuoiHelper.ChuanHoaMa(request.MaNhomNangLuc);

        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã nhóm năng lực '{ma}' đã tồn tại.");
        }

        entity.MaNhomNangLuc = ma;
        entity.TenNhomNangLuc = ChuoiHelper.ChuanHoaBatBuoc(request.TenNhomNangLuc);
        entity.ThoiGianLamHang = request.ThoiGianLamHang;
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm năng lực.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static NhomNangLucDto ChuyenDto(NhomNangLucEntity entity)
        => new(
            entity.Id, entity.MaNhomNangLuc, entity.TenNhomNangLuc,
            entity.ThoiGianLamHang, (byte)entity.TrangThai,
            entity.CreatedAt, entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
