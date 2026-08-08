using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.HinhDang.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.QuyTacNhomM;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Services;

public sealed class QuyTacNhomMService(
    IQuyTacNhomMRepository repository,
    IHinhDangRepository hinhDangRepository,
    INhomMRepository nhomMRepository,
    IUnitOfWork unitOfWork) : IQuyTacNhomMService
{
    public async Task<PagedResult<QuyTacNhomMDto>> LayDanhSachAsync(BoLocQuyTacNhomMRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<QuyTacNhomMDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<QuyTacNhomMDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy quy tắc nhóm M."));

    public async Task<QuyTacNhomMDto> TaoMoiAsync(TaoQuyTacNhomMRequest request, CancellationToken cancellationToken)
    {
        var hinhDang = await hinhDangRepository.LayTheoIdAsync(request.HinhDangId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hình dáng.");
        var nhomM = await nhomMRepository.LayTheoIdAsync(request.NhomMId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm M.");

        BomValidationHelper.KiemTraDangHoatDong(hinhDang.IsActive, "Hình dáng");
        BomValidationHelper.KiemTraDangHoatDong(nhomM.IsActive, "Nhóm M");
        BomValidationHelper.KiemTraKhoangDienTich(request.DienTichTu, request.DienTichDen);

        if (await repository.TonTaiTrungAsync(
            request.HinhDangId,
            request.NhomMId,
            null,
            cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Quy tắc đã tồn tại cho cùng hình dáng và nhóm M.");
        }

        if (await repository.TonTaiKhoangChongLanAsync(
            nhomM.PhamViBom,
            request.HinhDangId,
            request.DienTichTu!.Value,
            request.DienTichDen,
            request.BaoGomTu,
            request.BaoGomDen,
            null,
            cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Khoảng diện tích bị chồng lấn với quy tắc khác trong cùng phạm vi B.O.M và hình dáng.");
        }

        var entity = new Entity
        {
            HinhDangId = request.HinhDangId,
            DienTichTu = request.DienTichTu!.Value,
            DienTichDen = request.DienTichDen,
            BaoGomTu = request.BaoGomTu,
            BaoGomDen = request.BaoGomDen,
            NhomMId = request.NhomMId,
            GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<QuyTacNhomMDto> CapNhatAsync(long id, CapNhatQuyTacNhomMRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy quy tắc nhóm M.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var hinhDang = await hinhDangRepository.LayTheoIdAsync(request.HinhDangId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hình dáng.");
        var nhomM = await nhomMRepository.LayTheoIdAsync(request.NhomMId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm M.");

        BomValidationHelper.KiemTraDangHoatDong(hinhDang.IsActive, "Hình dáng");
        BomValidationHelper.KiemTraDangHoatDong(nhomM.IsActive, "Nhóm M");
        BomValidationHelper.KiemTraKhoangDienTich(request.DienTichTu, request.DienTichDen);

        if (await repository.TonTaiTrungAsync(
            request.HinhDangId,
            request.NhomMId,
            id,
            cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Quy tắc đã tồn tại cho cùng hình dáng và nhóm M.");
        }

        if (await repository.TonTaiKhoangChongLanAsync(
            nhomM.PhamViBom,
            request.HinhDangId,
            request.DienTichTu!.Value,
            request.DienTichDen,
            request.BaoGomTu,
            request.BaoGomDen,
            id,
            cancellationToken))
        {
            throw new XungDotDuLieuException(
                "Khoảng diện tích bị chồng lấn với quy tắc khác trong cùng phạm vi B.O.M và hình dáng.");
        }

        entity.HinhDangId = request.HinhDangId;
        entity.DienTichTu = request.DienTichTu!.Value;
        entity.DienTichDen = request.DienTichDen;
        entity.BaoGomTu = request.BaoGomTu;
        entity.BaoGomDen = request.BaoGomDen;
        entity.NhomMId = request.NhomMId;
        entity.GhiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy quy tắc nhóm M.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static QuyTacNhomMDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            HinhDangId = entity.HinhDangId,
            DienTichTu = entity.DienTichTu,
            DienTichDen = entity.DienTichDen,
            BaoGomTu = entity.BaoGomTu,
            BaoGomDen = entity.BaoGomDen,
            NhomMId = entity.NhomMId,
            GhiChu = entity.GhiChu,
            MaHinhDang = entity.HinhDang.MaHinhDang,
            TenHinhDang = entity.HinhDang.TenHinhDang,
            PhamViBom = entity.NhomM.PhamViBom,
            MaNhomM = entity.NhomM.MaNhomM,
            TenNhomM = entity.NhomM.TenNhomM,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
