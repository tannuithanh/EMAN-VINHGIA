
using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Interfaces;
using Eman.Domain.Common.Enums;
using DieuKienThanhToanEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DieuKienThanhToan;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DieuKienThanhToan.Services;

public sealed class DieuKienThanhToanService(
    IDieuKienThanhToanRepository repository,
    IUnitOfWork unitOfWork) : IDieuKienThanhToanService
{
    public async Task<PagedResult<DieuKienThanhToanDto>> LayDanhSachAsync(
        BoLocDieuKienThanhToanRequest request,
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

        return new PagedResult<DieuKienThanhToanDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<DieuKienThanhToanDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy điều kiện thanh toán.");

        return ChuyenDto(entity);
    }

    public async Task<DieuKienThanhToanDto> TaoMoiAsync(
        TaoDieuKienThanhToanRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaDieuKienThanhToan);

        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Mã điều kiện thanh toán '{ma}' đã tồn tại.");
        }

        var entity = new DieuKienThanhToanEntity
        {
            MaDieuKienThanhToan = ma,
            TenDieuKienThanhToan = ChuoiHelper.ChuanHoaBatBuoc(
                request.TenDieuKienThanhToan),
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ChuyenDto(entity);
    }

    public async Task<DieuKienThanhToanDto> CapNhatAsync(
        Guid id,
        CapNhatDieuKienThanhToanRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy điều kiện thanh toán.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaDieuKienThanhToan);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Mã điều kiện thanh toán '{ma}' đã tồn tại.");
        }

        entity.MaDieuKienThanhToan = ma;
        entity.TenDieuKienThanhToan = ChuoiHelper.ChuanHoaBatBuoc(
            request.TenDieuKienThanhToan);
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
            ?? throw new KhongTimThayException("Không tìm thấy điều kiện thanh toán.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (await repository.DangDuocSuDungAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể xóa điều kiện thanh toán vì đã có đối tác kinh doanh sử dụng.");
        }

        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static DieuKienThanhToanDto ChuyenDto(DieuKienThanhToanEntity entity)
        => new(
            entity.Id,
            entity.MaDieuKienThanhToan,
            entity.TenDieuKienThanhToan,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
