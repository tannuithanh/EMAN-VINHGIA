using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Dtos;
using Eman.Application.Modules.MasterData.Products.ThueSanPham.Interfaces;
using Eman.Domain.Common.Enums;
using ThueSanPhamEntity = Eman.Domain.Modules.MasterData.Products.Entities.ThueSanPham;

namespace Eman.Application.Modules.MasterData.Products.ThueSanPham.Services;

public sealed class ThueSanPhamService(
    IThueSanPhamRepository repository,
    IUnitOfWork unitOfWork) : IThueSanPhamService
{
    public async Task<PagedResult<ThueSanPhamDto>> LayDanhSachAsync(
        BoLocThueSanPhamRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword, trangThai, request.Page, request.PageSize, cancellationToken);

        return new PagedResult<ThueSanPhamDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ThueSanPhamDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy thuế sản phẩm.");
        return ChuyenDto(entity);
    }

    public async Task<ThueSanPhamDto> TaoMoiAsync(
        TaoThueSanPhamRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaThue);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã thuế '{ma}' đã tồn tại.");
        }

        var entity = new ThueSanPhamEntity
        {
            MaThue = ma,
            TenThue = ChuoiHelper.ChuanHoaBatBuoc(request.TenThue),
            ThueSuat = request.ThueSuat,
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<ThueSanPhamDto> CapNhatAsync(
        Guid id,
        CapNhatThueSanPhamRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy thuế sản phẩm.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var ma = ChuoiHelper.ChuanHoaMa(request.MaThue);

        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã thuế '{ma}' đã tồn tại.");
        }

        entity.MaThue = ma;
        entity.TenThue = ChuoiHelper.ChuanHoaBatBuoc(request.TenThue);
        entity.ThueSuat = request.ThueSuat;
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy thuế sản phẩm.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static ThueSanPhamDto ChuyenDto(ThueSanPhamEntity entity)
        => new(
            entity.Id, entity.MaThue, entity.TenThue, entity.ThueSuat,
            (byte)entity.TrangThai, entity.CreatedAt, entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
