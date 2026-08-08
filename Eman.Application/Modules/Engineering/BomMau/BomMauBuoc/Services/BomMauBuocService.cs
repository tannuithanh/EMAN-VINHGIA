using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauBuoc;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Services;

public sealed class BomMauBuocService(IBomMauBuocRepository repository, IUnitOfWork unitOfWork) : IBomMauBuocService
{
    public async Task<PagedResult<BomMauBuocDto>> LayDanhSachAsync(BoLocBomMauBuocRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<BomMauBuocDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<BomMauBuocDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước B.O.M màu."));

    public async Task<BomMauBuocDto> TaoMoiAsync(TaoBomMauBuocRequest request, CancellationToken cancellationToken)
    {
        var maBuoc = ChuoiHelper.ChuanHoaMa(request.MaBuoc);
        var tenBuoc = ChuoiHelper.ChuanHoaBatBuoc(request.TenBuoc);
        if (await repository.TonTaiMaAsync(maBuoc, null, cancellationToken))
            throw new XungDotDuLieuException($"Mã bước B.O.M màu '{maBuoc}' đã tồn tại.");
        var entity = new Entity
        {
            MaBuoc = maBuoc,
            TenBuoc = tenBuoc,
            IsActive = true

        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<BomMauBuocDto> CapNhatAsync(long id, CapNhatBomMauBuocRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước B.O.M màu.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var maBuoc = ChuoiHelper.ChuanHoaMa(request.MaBuoc);
        var tenBuoc = ChuoiHelper.ChuanHoaBatBuoc(request.TenBuoc);
        if (await repository.TonTaiMaAsync(maBuoc, id, cancellationToken))
            throw new XungDotDuLieuException($"Mã bước B.O.M màu '{maBuoc}' đã tồn tại.");
        entity.MaBuoc = maBuoc;
        entity.TenBuoc = tenBuoc;
        entity.IsActive = request.IsActive;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước B.O.M màu.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static BomMauBuocDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            MaBuoc = entity.MaBuoc,
            TenBuoc = entity.TenBuoc,
            IsActive = entity.IsActive,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion),
        };
}
