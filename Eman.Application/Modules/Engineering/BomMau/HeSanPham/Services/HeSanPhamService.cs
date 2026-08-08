using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.HeSanPham;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Services;

public sealed class HeSanPhamService(IHeSanPhamRepository repository, IUnitOfWork unitOfWork) : IHeSanPhamService
{
    public async Task<PagedResult<HeSanPhamDto>> LayDanhSachAsync(BoLocHeSanPhamRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<HeSanPhamDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<HeSanPhamDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ sản phẩm."));

    public async Task<HeSanPhamDto> TaoMoiAsync(TaoHeSanPhamRequest request, CancellationToken cancellationToken)
    {
        var maHe = ChuoiHelper.ChuanHoaMa(request.MaHe);
        var tenHe = ChuoiHelper.ChuanHoaBatBuoc(request.TenHe);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiIdAsync(request.Id, cancellationToken))
            throw new XungDotDuLieuException($"ID hệ sản phẩm '{request.Id}' đã tồn tại.");

        if (await repository.TonTaiMaAsync(maHe, null, cancellationToken))
            throw new XungDotDuLieuException($"Mã hệ sản phẩm '{maHe}' đã tồn tại.");
        var entity = new Entity
        {
            Id = request.Id,
            MaHe = maHe,
            TenHe = tenHe,
            MoTa = moTa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<HeSanPhamDto> CapNhatAsync(long id, CapNhatHeSanPhamRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ sản phẩm.");
        var maHe = ChuoiHelper.ChuanHoaMa(request.MaHe);
        var tenHe = ChuoiHelper.ChuanHoaBatBuoc(request.TenHe);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiMaAsync(maHe, id, cancellationToken))
            throw new XungDotDuLieuException($"Mã hệ sản phẩm '{maHe}' đã tồn tại.");
        entity.MaHe = maHe;
        entity.TenHe = tenHe;
        entity.MoTa = moTa;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(long id, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ sản phẩm.");
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static HeSanPhamDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            MaHe = entity.MaHe,
            TenHe = entity.TenHe,
            MoTa = entity.MoTa,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
}
