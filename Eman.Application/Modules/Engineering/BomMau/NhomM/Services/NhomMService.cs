using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.NhomM;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Services;

public sealed class NhomMService(INhomMRepository repository, IUnitOfWork unitOfWork) : INhomMService
{
    public async Task<PagedResult<NhomMDto>> LayDanhSachAsync(BoLocNhomMRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<NhomMDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<NhomMDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm M."));

    public async Task<NhomMDto> TaoMoiAsync(TaoNhomMRequest request, CancellationToken cancellationToken)
    {
        var phamViBom = BomValidationHelper.ChuanHoaPhamViBom(request.PhamViBom);
        var maNhomM = ChuoiHelper.ChuanHoaMa(request.MaNhomM);
        var tenNhomM = ChuoiHelper.ChuanHoaBatBuoc(request.TenNhomM);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);

        if (await repository.TonTaiMaAsync(phamViBom, maNhomM, null, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Mã nhóm M '{maNhomM}' đã tồn tại trong phạm vi {phamViBom}.");
        }

        var entity = new Entity
        {
            PhamViBom = phamViBom,
            MaNhomM = maNhomM,
            TenNhomM = tenNhomM,
            ThuTu = request.ThuTu,
            MoTa = moTa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<NhomMDto> CapNhatAsync(long id, CapNhatNhomMRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm M.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var phamViBom = BomValidationHelper.ChuanHoaPhamViBom(request.PhamViBom);
        var maNhomM = ChuoiHelper.ChuanHoaMa(request.MaNhomM);
        var tenNhomM = ChuoiHelper.ChuanHoaBatBuoc(request.TenNhomM);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);

        if (await repository.TonTaiMaAsync(phamViBom, maNhomM, id, cancellationToken))
        {
            throw new XungDotDuLieuException(
                $"Mã nhóm M '{maNhomM}' đã tồn tại trong phạm vi {phamViBom}.");
        }

        entity.PhamViBom = phamViBom;
        entity.MaNhomM = maNhomM;
        entity.TenNhomM = tenNhomM;
        entity.ThuTu = request.ThuTu;
        entity.MoTa = moTa;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm M.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static NhomMDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            PhamViBom = entity.PhamViBom,
            MaNhomM = entity.MaNhomM,
            TenNhomM = entity.TenNhomM,
            ThuTu = entity.ThuTu,
            MoTa = entity.MoTa,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
