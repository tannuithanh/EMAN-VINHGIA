
using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Interfaces;
using Eman.Domain.Common.Enums;
using DieuKienGiaoHangEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienGiaoHang;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienGiaoHang.Services;

public sealed class DieuKienGiaoHangService(
    IDieuKienGiaoHangRepository repository,
    IUnitOfWork unitOfWork) : IDieuKienGiaoHangService
{
    public async Task<PagedResult<DieuKienGiaoHangDto>> LayDanhSachAsync(
        BoLocDieuKienGiaoHangRequest request,
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

        return new PagedResult<DieuKienGiaoHangDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<DieuKienGiaoHangDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy điều kiện giao hàng.");

        return ChuyenDto(entity);
    }

    public async Task<DieuKienGiaoHangDto> TaoMoiAsync(
        TaoDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaDieuKienGiaoHang);

        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Mã điều kiện giao hàng '{ma}' đã tồn tại.");
        }

        var entity = new DieuKienGiaoHangEntity
        {
            MaDieuKienGiaoHang = ma,
            TenDieuKienGiaoHang = ChuoiHelper.ChuanHoaBatBuoc(
                request.TenDieuKienGiaoHang),
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ChuyenDto(entity);
    }

    public async Task<DieuKienGiaoHangDto> CapNhatAsync(
        Guid id,
        CapNhatDieuKienGiaoHangRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy điều kiện giao hàng.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaDieuKienGiaoHang);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Mã điều kiện giao hàng '{ma}' đã tồn tại.");
        }

        entity.MaDieuKienGiaoHang = ma;
        entity.TenDieuKienGiaoHang = ChuoiHelper.ChuanHoaBatBuoc(
            request.TenDieuKienGiaoHang);
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
            ?? throw new KhongTimThayException("Không tìm thấy điều kiện giao hàng.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (await repository.DangDuocSuDungAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể xóa điều kiện giao hàng vì đã có đối tác kinh doanh sử dụng.");
        }

        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static DieuKienGiaoHangDto ChuyenDto(DieuKienGiaoHangEntity entity)
        => new(
            entity.Id,
            entity.MaDieuKienGiaoHang,
            entity.TenDieuKienGiaoHang,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
