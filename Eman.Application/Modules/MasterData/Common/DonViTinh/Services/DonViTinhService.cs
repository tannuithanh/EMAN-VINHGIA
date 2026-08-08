using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Dtos;
using Eman.Application.Modules.MasterData.Common.DonViTinh.Interfaces;
using Eman.Domain.Common.Enums;
using DonViTinhEntity = Eman.Domain.Modules.MasterData.Common.Entities.DonViTinh;

namespace Eman.Application.Modules.MasterData.Common.DonViTinh.Services;

public sealed class DonViTinhService(
    IDonViTinhRepository repository,
    IUnitOfWork unitOfWork) : IDonViTinhService
{
    public async Task<PagedResult<DonViTinhDto>> LayDanhSachAsync(
        BoLocDonViTinhRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword, trangThai, request.Page, request.PageSize, cancellationToken);

        return new PagedResult<DonViTinhDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<DonViTinhDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đơn vị tính.");
        return ChuyenDto(entity);
    }

    public async Task<DonViTinhDto> TaoMoiAsync(
        TaoDonViTinhRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaDonViTinh);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã đơn vị tính '{ma}' đã tồn tại.");
        }

        var entity = new DonViTinhEntity
        {
            MaDonViTinh = ma,
            TenDonViTinh = ChuoiHelper.ChuanHoaBatBuoc(request.TenDonViTinh),
            KyHieu = ChuoiHelper.ChuanHoaTuyChon(request.KyHieu),
            MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa),
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<DonViTinhDto> CapNhatAsync(
        Guid id,
        CapNhatDonViTinhRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đơn vị tính.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var ma = ChuoiHelper.ChuanHoaMa(request.MaDonViTinh);

        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã đơn vị tính '{ma}' đã tồn tại.");
        }

        entity.MaDonViTinh = ma;
        entity.TenDonViTinh = ChuoiHelper.ChuanHoaBatBuoc(request.TenDonViTinh);
        entity.KyHieu = ChuoiHelper.ChuanHoaTuyChon(request.KyHieu);
        entity.MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đơn vị tính.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static DonViTinhDto ChuyenDto(DonViTinhEntity entity)
        => new(
            entity.Id,
            entity.MaDonViTinh,
            entity.TenDonViTinh,
            entity.KyHieu,
            entity.MoTa,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
