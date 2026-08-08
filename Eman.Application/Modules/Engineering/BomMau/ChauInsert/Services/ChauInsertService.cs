using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.ChauInsert;

namespace Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Services;

public sealed class ChauInsertService(IChauInsertRepository repository, IUnitOfWork unitOfWork) : IChauInsertService
{
    public async Task<PagedResult<ChauInsertDto>> LayDanhSachAsync(BoLocChauInsertRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<ChauInsertDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<ChauInsertDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert."));

    public async Task<ChauInsertDto> TaoMoiAsync(TaoChauInsertRequest request, CancellationToken cancellationToken)
    {
        var maChauInsert = ChuoiHelper.ChuanHoaMa(request.MaChauInsert);
        var tenChauInsert = ChuoiHelper.ChuanHoaTuyChon(request.TenChauInsert);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiMaAsync(maChauInsert, null, cancellationToken))
            throw new XungDotDuLieuException($"Mã chậu insert '{maChauInsert}' đã tồn tại.");
        var entity = new Entity
        {
            MaChauInsert = maChauInsert,
            TenChauInsert = tenChauInsert,
            MoTa = moTa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<ChauInsertDto> CapNhatAsync(Guid id, CapNhatChauInsertRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var maChauInsert = ChuoiHelper.ChuanHoaMa(request.MaChauInsert);
        var tenChauInsert = ChuoiHelper.ChuanHoaTuyChon(request.TenChauInsert);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiMaAsync(maChauInsert, id, cancellationToken))
            throw new XungDotDuLieuException($"Mã chậu insert '{maChauInsert}' đã tồn tại.");
        entity.MaChauInsert = maChauInsert;
        entity.TenChauInsert = tenChauInsert;
        entity.MoTa = moTa;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static ChauInsertDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            MaChauInsert = entity.MaChauInsert,
            TenChauInsert = entity.TenChauInsert,
            MoTa = entity.MoTa,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion),
        };
}
