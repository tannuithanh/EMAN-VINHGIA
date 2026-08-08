using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MaHang;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Services;

public sealed class MaHangService(
    IMaHangRepository repository,
    IHinhDangRepository hinhDangRepository,
    IUnitOfWork unitOfWork) : IMaHangService
{
    public async Task<PagedResult<MaHangDto>> LayDanhSachAsync(BoLocMaHangRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<MaHangDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<MaHangDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy mã hàng."));

    public async Task<MaHangDto> TaoMoiAsync(TaoMaHangRequest request, CancellationToken cancellationToken)
    {
        await KiemTraHinhDangAsync(request.HinhDangBomThoId, request.HinhDangBomMauId, cancellationToken);

        var maHang = ChuoiHelper.ChuanHoaMa(request.MaHang);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        var loaiMaHang = ChuoiHelper.ChuanHoaMaTuyChon(request.LoaiMaHang) ?? "SAN_PHAM";

        if (await repository.TonTaiTrungAsync(maHang, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã hàng '{maHang}' đã tồn tại.");
        }

        var entity = new Entity
        {
            MaHangCode = maHang,
            DienTich = request.DienTich!.Value,
            HinhDangBomThoId = request.HinhDangBomThoId,
            HinhDangBomMauId = request.HinhDangBomMauId,
            MoTa = moTa,
            LoaiMaHang = loaiMaHang,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<MaHangDto> CapNhatAsync(long id, CapNhatMaHangRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy mã hàng.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        await KiemTraHinhDangAsync(request.HinhDangBomThoId, request.HinhDangBomMauId, cancellationToken);

        var maHang = ChuoiHelper.ChuanHoaMa(request.MaHang);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        var loaiMaHang = ChuoiHelper.ChuanHoaMaTuyChon(request.LoaiMaHang) ?? entity.LoaiMaHang;

        if (await repository.TonTaiTrungAsync(maHang, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã hàng '{maHang}' đã tồn tại.");
        }

        entity.MaHangCode = maHang;
        entity.DienTich = request.DienTich!.Value;
        entity.HinhDangBomThoId = request.HinhDangBomThoId;
        entity.HinhDangBomMauId = request.HinhDangBomMauId;
        entity.MoTa = moTa;
        entity.LoaiMaHang = loaiMaHang;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy mã hàng.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task KiemTraHinhDangAsync(
        long? hinhDangBomThoId,
        long? hinhDangBomMauId,
        CancellationToken cancellationToken)
    {
        if (!hinhDangBomThoId.HasValue && !hinhDangBomMauId.HasValue)
        {
            throw new QuyTacNghiepVuException(
                "Mã hàng phải có ít nhất một hình dáng cho B.O.M thô hoặc B.O.M màu.");
        }

        if (hinhDangBomThoId.HasValue)
        {
            var hinhDangBomTho = await hinhDangRepository.LayTheoIdAsync(
                hinhDangBomThoId.Value,
                false,
                cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy hình dáng B.O.M thô.");

            BomValidationHelper.KiemTraDangHoatDong(hinhDangBomTho.IsActive, "Hình dáng B.O.M thô");
        }

        if (hinhDangBomMauId.HasValue && hinhDangBomMauId != hinhDangBomThoId)
        {
            var hinhDangBomMau = await hinhDangRepository.LayTheoIdAsync(
                hinhDangBomMauId.Value,
                false,
                cancellationToken)
                ?? throw new KhongTimThayException("Không tìm thấy hình dáng B.O.M màu.");

            BomValidationHelper.KiemTraDangHoatDong(hinhDangBomMau.IsActive, "Hình dáng B.O.M màu");
        }
    }

    private static MaHangDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            MaHang = entity.MaHangCode,
            DienTich = entity.DienTich,
            HinhDangBomThoId = entity.HinhDangBomThoId,
            MaHinhDangBomTho = entity.HinhDangBomTho?.MaHinhDang,
            TenHinhDangBomTho = entity.HinhDangBomTho?.TenHinhDang,
            HinhDangBomMauId = entity.HinhDangBomMauId,
            MaHinhDangBomMau = entity.HinhDangBomMau?.MaHinhDang,
            TenHinhDangBomMau = entity.HinhDangBomMau?.TenHinhDang,
            MoTa = entity.MoTa,
            LoaiMaHang = entity.LoaiMaHang,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
