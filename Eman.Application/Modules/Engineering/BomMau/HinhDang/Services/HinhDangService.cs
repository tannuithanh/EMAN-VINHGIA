using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HinhDang;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Services;

public sealed class HinhDangService(IHinhDangRepository repository, IUnitOfWork unitOfWork) : IHinhDangService
{
    public async Task<PagedResult<HinhDangDto>> LayDanhSachAsync(BoLocHinhDangRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<HinhDangDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<HinhDangDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hình dáng."));

    public async Task<HinhDangDto> TaoMoiAsync(TaoHinhDangRequest request, CancellationToken cancellationToken)
    {
        var maHinhDang = ChuoiHelper.ChuanHoaMa(request.MaHinhDang);
        var tenHinhDang = ChuoiHelper.ChuanHoaBatBuoc(request.TenHinhDang);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiMaAsync(maHinhDang, null, cancellationToken))
            throw new XungDotDuLieuException($"Mã hình dáng '{maHinhDang}' đã tồn tại.");
        var entity = new Entity
        {
            MaHinhDang = maHinhDang,
            TenHinhDang = tenHinhDang,
            MoTa = moTa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<HinhDangDto> CapNhatAsync(long id, CapNhatHinhDangRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hình dáng.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var maHinhDang = ChuoiHelper.ChuanHoaMa(request.MaHinhDang);
        var tenHinhDang = ChuoiHelper.ChuanHoaBatBuoc(request.TenHinhDang);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiMaAsync(maHinhDang, id, cancellationToken))
            throw new XungDotDuLieuException($"Mã hình dáng '{maHinhDang}' đã tồn tại.");
        entity.MaHinhDang = maHinhDang;
        entity.TenHinhDang = tenHinhDang;
        entity.MoTa = moTa;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hình dáng.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static HinhDangDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            MaHinhDang = entity.MaHinhDang,
            TenHinhDang = entity.TenHinhDang,
            MoTa = entity.MoTa,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion),
        };
}
