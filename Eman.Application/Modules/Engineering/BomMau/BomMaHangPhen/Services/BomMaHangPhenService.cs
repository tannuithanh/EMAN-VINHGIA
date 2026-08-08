using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangPhen;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangPhen.Services;

public sealed class BomMaHangPhenService(
    IBomMaHangPhenRepository repository,
    IMaHangRepository maHangRepository,
    IUnitOfWork unitOfWork) : IBomMaHangPhenService
{
    public async Task<PagedResult<BomMaHangPhenDto>> LayDanhSachAsync(
        BoLocBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<BomMaHangPhenDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<MaHangCoPhenDto>> LayDanhSachMaHangCoPhenAsync(
        BoLocBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<MaHangCoPhenDto>
        {
            Items = items.Select(ChuyenMaHangCoPhenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BomMaHangPhenDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phên theo mã hàng."));

    public async Task<BomMaHangPhenDto> TaoMoiAsync(
        TaoBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
    {
        var maHang = await maHangRepository.LayTheoIdAsync(request.MaHangId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy mã hàng.");
        BomValidationHelper.KiemTraDangHoatDong(maHang.IsActive, "Mã hàng");

        var maHangPhen = ChuoiHelper.ChuanHoaMa(request.MaHangPhen);
        if (await repository.TonTaiTrungAsync(request.MaHangId, null, cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Mã hàng đã được khai báo phên.");
        }

        var entity = new Entity
        {
            MaHangId = request.MaHangId,
            MaHangPhen = maHangPhen,
            GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu),
            MaHang = maHang.MaHangCode,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BomMaHangPhenDto> CapNhatAsync(
        Guid id,
        CapNhatBomMaHangPhenRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phên theo mã hàng.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var maHang = await maHangRepository.LayTheoIdAsync(request.MaHangId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy mã hàng.");
        BomValidationHelper.KiemTraDangHoatDong(maHang.IsActive, "Mã hàng");

        if (await repository.TonTaiTrungAsync(request.MaHangId, id, cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Mã hàng đã được khai báo phên.");
        }

        entity.MaHangId = request.MaHangId;
        entity.MaHangPhen = ChuoiHelper.ChuanHoaMa(request.MaHangPhen);
        entity.GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        entity.MaHang = maHang.MaHangCode;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy phên theo mã hàng.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static BomMaHangPhenDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            MaHangId = entity.MaHangId,
            MaHangPhen = entity.MaHangPhen,
            GhiChu = entity.GhiChu,
            MaHang = entity.MaHang,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };

    private static MaHangCoPhenDto ChuyenMaHangCoPhenDto(Entity entity)
        => new()
        {
            CauHinhPhenId = entity.Id,
            MaHangId = entity.MaHangId,
            MaHang = entity.MaHang,
            MaHangPhen = entity.MaHangPhen,
            GhiChu = entity.GhiChu,
            IsActive = entity.IsActive,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
