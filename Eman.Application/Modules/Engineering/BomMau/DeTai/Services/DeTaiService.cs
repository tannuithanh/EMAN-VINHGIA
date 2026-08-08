using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.DeTai;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Services;

public sealed class DeTaiService(
    IDeTaiRepository repository,
    IHeSanPhamRepository heSanPhamRepository,
    IUnitOfWork unitOfWork) : IDeTaiService
{
    public async Task<PagedResult<DeTaiDto>> LayDanhSachAsync(BoLocDeTaiRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<DeTaiDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<DeTaiDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đề tài."));

    public async Task<DeTaiDto> TaoMoiAsync(TaoDeTaiRequest request, CancellationToken cancellationToken)
    {
        var heSanPham = await heSanPhamRepository.LayTheoIdAsync(request.HeSanPhamId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ sản phẩm.");
        BomValidationHelper.KiemTraDangHoatDong(heSanPham.IsActive, "Hệ sản phẩm");
        var heSanPhamId = request.HeSanPhamId;
        var maDeTai = ChuoiHelper.ChuanHoaMa(request.MaDeTai);
        var tenDeTai = ChuoiHelper.ChuanHoaBatBuoc(request.TenDeTai);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiTrungAsync(request.HeSanPhamId, maDeTai, null, cancellationToken))
            throw new XungDotDuLieuException("Đề tài đã tồn tại với cùng thông tin khóa.");
        var entity = new Entity
        {
            HeSanPhamId = heSanPhamId,
            MaDeTai = maDeTai,
            TenDeTai = tenDeTai,
            MoTa = moTa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<DeTaiDto> CapNhatAsync(long id, CapNhatDeTaiRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đề tài.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var heSanPham = await heSanPhamRepository.LayTheoIdAsync(request.HeSanPhamId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ sản phẩm.");
        BomValidationHelper.KiemTraDangHoatDong(heSanPham.IsActive, "Hệ sản phẩm");
        var heSanPhamId = request.HeSanPhamId;
        var maDeTai = ChuoiHelper.ChuanHoaMa(request.MaDeTai);
        var tenDeTai = ChuoiHelper.ChuanHoaBatBuoc(request.TenDeTai);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        if (await repository.TonTaiTrungAsync(request.HeSanPhamId, maDeTai, id, cancellationToken))
            throw new XungDotDuLieuException("Đề tài đã tồn tại với cùng thông tin khóa.");
        entity.HeSanPhamId = heSanPhamId;
        entity.MaDeTai = maDeTai;
        entity.TenDeTai = tenDeTai;
        entity.MoTa = moTa;

        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đề tài.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static DeTaiDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            HeSanPhamId = entity.HeSanPhamId,
            MaDeTai = entity.MaDeTai,
            TenDeTai = entity.TenDeTai,
            MoTa = entity.MoTa,
            MaHe = entity.HeSanPham.MaHe,
            TenHe = entity.HeSanPham.TenHe,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
