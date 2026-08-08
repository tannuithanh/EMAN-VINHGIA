using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.MaHang.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.ChauInsert.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMaHangChauInsert;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMaHangChauInsert.Services;

public sealed class BomMaHangChauInsertService(
    IBomMaHangChauInsertRepository repository,
    IMaHangRepository maHangRepository,
    IChauInsertRepository chauInsertRepository,
    IUnitOfWork unitOfWork) : IBomMaHangChauInsertService
{
    public async Task<PagedResult<BomMaHangChauInsertDto>> LayDanhSachAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<BomMaHangChauInsertDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<MaHangCoChauInsertDto>> LayDanhSachMaHangCoChauInsertAsync(
        BoLocBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachMaHangCoChauInsertAsync(
            request,
            cancellationToken);

        var duLieu = items
            .GroupBy(x => new { x.MaHangId, x.MaHang })
            .OrderBy(nhom => nhom.Key.MaHang)
            .ThenBy(nhom => nhom.Key.MaHangId)
            .Select(nhom => new MaHangCoChauInsertDto
            {
                MaHangId = nhom.Key.MaHangId,
                MaHang = nhom.Key.MaHang,
                SoLoaiChauInsert = nhom.Select(x => x.ChauInsertId).Distinct().Count(),
                TongSoLuongChauInsert = nhom.Sum(x => x.SoLuong),
                DanhSachChauInsert = nhom
                    .OrderBy(x => x.MaChauInsert)
                    .ThenBy(x => x.Id)
                    .Select(ChuyenChiTietTheoMaHangDto)
                    .ToList()
            })
            .ToList();

        return new PagedResult<MaHangCoChauInsertDto>
        {
            Items = duLieu,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BomMaHangChauInsertDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert theo mã hàng."));

    public async Task<BomMaHangChauInsertDto> TaoMoiAsync(
        TaoBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
    {
        KiemTraChauInsertId(request.ChauInsertId);

        var maHang = await maHangRepository.LayTheoIdAsync(request.MaHangId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy mã hàng.");
        var chauInsert = await chauInsertRepository.LayTheoIdAsync(request.ChauInsertId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert.");

        BomValidationHelper.KiemTraDangHoatDong(maHang.IsActive, "Mã hàng");
        BomValidationHelper.KiemTraDangHoatDong(chauInsert.IsActive, "Chậu insert");

        if (await repository.TonTaiTrungAsync(
                request.MaHangId,
                request.ChauInsertId,
                null,
                cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Mã hàng đã được khai báo cùng loại chậu insert này.");
        }

        var entity = new Entity
        {
            MaHangId = request.MaHangId,
            ChauInsertId = request.ChauInsertId,
            SoLuong = request.SoLuong,
            GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu),
            MaHang = maHang.MaHangCode,
            MaChauInsert = chauInsert.MaChauInsert,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BomMaHangChauInsertDto> CapNhatAsync(
        Guid id,
        CapNhatBomMaHangChauInsertRequest request,
        CancellationToken cancellationToken)
    {
        KiemTraChauInsertId(request.ChauInsertId);

        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert theo mã hàng.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var maHang = await maHangRepository.LayTheoIdAsync(request.MaHangId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy mã hàng.");
        var chauInsert = await chauInsertRepository.LayTheoIdAsync(request.ChauInsertId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert.");

        BomValidationHelper.KiemTraDangHoatDong(maHang.IsActive, "Mã hàng");
        BomValidationHelper.KiemTraDangHoatDong(chauInsert.IsActive, "Chậu insert");

        if (await repository.TonTaiTrungAsync(
                request.MaHangId,
                request.ChauInsertId,
                id,
                cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Mã hàng đã được khai báo cùng loại chậu insert này.");
        }

        entity.MaHangId = request.MaHangId;
        entity.ChauInsertId = request.ChauInsertId;
        entity.SoLuong = request.SoLuong;
        entity.GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        entity.MaHang = maHang.MaHangCode;
        entity.MaChauInsert = chauInsert.MaChauInsert;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy chậu insert theo mã hàng.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static BomMaHangChauInsertDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            MaHangId = entity.MaHangId,
            ChauInsertId = entity.ChauInsertId,
            SoLuong = entity.SoLuong,
            GhiChu = entity.GhiChu,
            MaHang = entity.MaHang,
            MaChauInsert = entity.MaChauInsert,
            TenChauInsert = entity.ChauInsert.TenChauInsert,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };

    private static ChiTietChauInsertTheoMaHangDto ChuyenChiTietTheoMaHangDto(Entity entity)
        => new()
        {
            CauHinhChauInsertId = entity.Id,
            ChauInsertId = entity.ChauInsertId,
            MaChauInsert = entity.MaChauInsert,
            TenChauInsert = entity.ChauInsert.TenChauInsert,
            SoLuong = entity.SoLuong,
            GhiChu = entity.GhiChu,
            IsActive = entity.IsActive,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };

    private static void KiemTraChauInsertId(Guid chauInsertId)
    {
        if (chauInsertId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException("Chậu insert không hợp lệ.");
        }
    }
}
