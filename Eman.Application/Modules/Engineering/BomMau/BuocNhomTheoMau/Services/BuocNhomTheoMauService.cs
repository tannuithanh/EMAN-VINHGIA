using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BuocNhomTheoMau;
using MauSacEntity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MauSac;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Services;

public sealed class BuocNhomTheoMauService(
    IBuocNhomTheoMauRepository repository,
    IMauSacRepository mauSacRepository,
    IUnitOfWork unitOfWork) : IBuocNhomTheoMauService
{
    public async Task<PagedResult<BuocNhomTheoMauDto>> LayDanhSachAsync(
        BoLocBuocNhomTheoMauRequest request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);

        return new PagedResult<BuocNhomTheoMauDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BuocNhomTheoMauDto> LayTheoIdAsync(
        long id,
        CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước nhóm theo màu."));

    public async Task<BuocNhomTheoMauDto> TaoMoiAsync(
        TaoBuocNhomTheoMauRequest request,
        CancellationToken cancellationToken)
    {
        var mauSac = await LayVaKiemTraMauSacAsync(
            request.MauSacId,
            request.HeSanPhamId,
            request.DeTaiId,
            cancellationToken);

        var maBuoc = LayMaBuoc(request.MaBuoc, request.TenBuoc);
        var tenBuoc = ChuoiHelper.ChuanHoaBatBuoc(request.TenBuoc);
        var maHonHop = ChuoiHelper.ChuanHoaMa(request.MaHonHop);
        var ghiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);

        if (await repository.TonTaiTrungAsync(
                mauSac.HeSanPhamId,
                mauSac.Id,
                maBuoc,
                request.MaHonHopId,
                null,
                cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Bước nhóm theo màu đã tồn tại với cùng hệ, màu, mã bước và mã hỗn hợp.");
        }

        var entity = new Entity
        {
            HeSanPhamId = mauSac.HeSanPhamId,
            MauSacId = mauSac.Id,
            MaBuoc = maBuoc,
            TenBuoc = tenBuoc,
            MaHonHopId = request.MaHonHopId,
            MaHonHop = maHonHop,
            GhiChu = ghiChu,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BuocNhomTheoMauDto> CapNhatAsync(
        long id,
        CapNhatBuocNhomTheoMauRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước nhóm theo màu.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var mauSac = await LayVaKiemTraMauSacAsync(
            request.MauSacId,
            request.HeSanPhamId,
            request.DeTaiId,
            cancellationToken);

        var maBuoc = LayMaBuoc(request.MaBuoc, request.TenBuoc);
        var tenBuoc = ChuoiHelper.ChuanHoaBatBuoc(request.TenBuoc);
        var maHonHop = ChuoiHelper.ChuanHoaMa(request.MaHonHop);
        var ghiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);

        if (await repository.TonTaiTrungAsync(
                mauSac.HeSanPhamId,
                mauSac.Id,
                maBuoc,
                request.MaHonHopId,
                id,
                cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Bước nhóm theo màu đã tồn tại với cùng hệ, màu, mã bước và mã hỗn hợp.");
        }

        entity.HeSanPhamId = mauSac.HeSanPhamId;
        entity.MauSacId = mauSac.Id;
        entity.MaBuoc = maBuoc;
        entity.TenBuoc = tenBuoc;
        entity.MaHonHopId = request.MaHonHopId;
        entity.MaHonHop = maHonHop;
        entity.GhiChu = ghiChu;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(
        long id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước nhóm theo màu.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<MauSacEntity> LayVaKiemTraMauSacAsync(
        long mauSacId,
        long? heSanPhamId,
        long? deTaiId,
        CancellationToken cancellationToken)
    {
        var mauSac = await mauSacRepository.LayTheoIdAsync(
            mauSacId,
            false,
            cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy màu sắc.");

        if (heSanPhamId.HasValue && mauSac.HeSanPhamId != heSanPhamId.Value)
        {
            throw new QuyTacNghiepVuException(
                "Màu sắc không thuộc hệ sản phẩm đã chọn.");
        }

        if (deTaiId.HasValue && mauSac.DeTaiId != deTaiId.Value)
        {
            throw new QuyTacNghiepVuException(
                "Màu sắc không thuộc đề tài đã chọn.");
        }

        BomValidationHelper.KiemTraDangHoatDong(
            mauSac.HeSanPham.IsActive,
            "Hệ sản phẩm");
        BomValidationHelper.KiemTraDangHoatDong(
            mauSac.DeTai.IsActive,
            "Đề tài");
        BomValidationHelper.KiemTraDangHoatDong(
            mauSac.IsActive,
            "Màu sắc");

        return mauSac;
    }

    private static string LayMaBuoc(string? maBuoc, string tenBuoc)
        => ChuoiHelper.ChuanHoaMaTuyChon(maBuoc)
           ?? ChuoiHelper.ChuanHoaMa(tenBuoc);

    private static BuocNhomTheoMauDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            HeSanPhamId = entity.HeSanPhamId,
            MaHe = entity.HeSanPham.MaHe,
            TenHe = entity.HeSanPham.TenHe,
            DeTaiId = entity.MauSac.DeTaiId,
            MaDeTai = entity.MauSac.DeTai.MaDeTai,
            TenDeTai = entity.MauSac.DeTai.TenDeTai,
            MauSacId = entity.MauSacId,
            MaMau = entity.MauSac.MaMau,
            TenMau = entity.MauSac.TenMau,
            MaBuoc = entity.MaBuoc,
            TenBuoc = entity.TenBuoc,
            MaHonHopId = entity.MaHonHopId,
            MaHonHop = entity.MaHonHop,
            GhiChu = entity.GhiChu,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
