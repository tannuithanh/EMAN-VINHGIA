using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Inventory.Kho.Dtos;
using Eman.Application.Modules.MasterData.Inventory.Kho.Interfaces;
using Eman.Domain.Common.Enums;
using KhoEntity = Eman.Domain.Modules.MasterData.Inventory.Entities.Kho;

namespace Eman.Application.Modules.MasterData.Inventory.Kho.Services;

public sealed class KhoService(
    IKhoRepository repository,
    IUnitOfWork unitOfWork) : IKhoService
{
    public async Task<PagedResult<KhoDto>> LayDanhSachAsync(
        BoLocKhoRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword,
            request.HangTon,
            request.HangTru,
            trangThai,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<KhoDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<KhoDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy kho.");
        return ChuyenDto(entity);
    }

    public async Task<KhoDto> TaoMoiAsync(
        TaoKhoRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaKho);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã kho '{ma}' đã tồn tại.");
        }

        var entity = new KhoEntity
        {
            MaKho = ma,
            TenKho = ChuoiHelper.ChuanHoaBatBuoc(request.TenKho),
            HangTon = request.HangTon,
            HangTru = request.HangTru,
            MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa),
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<KhoDto> CapNhatAsync(
        Guid id,
        CapNhatKhoRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy kho.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var ma = ChuoiHelper.ChuanHoaMa(request.MaKho);

        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã kho '{ma}' đã tồn tại.");
        }

        entity.MaKho = ma;
        entity.TenKho = ChuoiHelper.ChuanHoaBatBuoc(request.TenKho);
        entity.HangTon = request.HangTon;
        entity.HangTru = request.HangTru;
        entity.MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy kho.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static KhoDto ChuyenDto(KhoEntity entity)
        => new(
            entity.Id,
            entity.MaKho,
            entity.TenKho,
            entity.HangTon,
            entity.HangTru,
            entity.MoTa,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
