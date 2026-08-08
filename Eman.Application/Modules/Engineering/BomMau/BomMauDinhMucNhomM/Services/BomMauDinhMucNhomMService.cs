using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BuocNhomTheoMau.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.NhomM.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauDinhMucNhomM;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauDinhMucNhomM.Services;

public sealed class BomMauDinhMucNhomMService(
    IBomMauDinhMucNhomMRepository repository,
    IBuocNhomTheoMauRepository buocNhomTheoMauRepository,
    INhomMRepository nhomMRepository,
    IUnitOfWork unitOfWork) : IBomMauDinhMucNhomMService
{
    public async Task<PagedResult<BomMauDinhMucNhomMDto>> LayDanhSachAsync(BoLocBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<BomMauDinhMucNhomMDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<BomMauDinhMucNhomMDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy định mức nhóm M."));

    public async Task<BomMauDinhMucNhomMDto> TaoMoiAsync(TaoBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken)
    {
        var buocNhom = await buocNhomTheoMauRepository.LayTheoIdAsync(request.BuocNhomMauId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước nhóm theo màu.");
        var nhomM = await nhomMRepository.LayTheoIdAsync(request.NhomMId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm M.");
        BomValidationHelper.KiemTraDangHoatDong(buocNhom.IsActive, "Bước nhóm theo màu");
        BomValidationHelper.KiemTraDangHoatDong(nhomM.IsActive, "Nhóm M");
        KiemTraNhomMBomMau(nhomM.PhamViBom);
        var buocNhomMauId = request.BuocNhomMauId;
        var nhomMId = request.NhomMId;
        var dinhMuc = request.DinhMuc;
        var ghiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        if (await repository.TonTaiTrungAsync(request.BuocNhomMauId, request.NhomMId, null, cancellationToken))
            throw new XungDotDuLieuException("Định mức nhóm m đã tồn tại với cùng thông tin khóa.");
        var entity = new Entity
        {
            BuocNhomMauId = buocNhomMauId,
            NhomMId = nhomMId,
            DinhMuc = dinhMuc,
            GhiChu = ghiChu,
            MaNhomM = nhomM.MaNhomM,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BomMauDinhMucNhomMDto> CapNhatAsync(long id, CapNhatBomMauDinhMucNhomMRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy định mức nhóm M.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var buocNhom = await buocNhomTheoMauRepository.LayTheoIdAsync(request.BuocNhomMauId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước nhóm theo màu.");
        var nhomM = await nhomMRepository.LayTheoIdAsync(request.NhomMId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm M.");
        BomValidationHelper.KiemTraDangHoatDong(buocNhom.IsActive, "Bước nhóm theo màu");
        BomValidationHelper.KiemTraDangHoatDong(nhomM.IsActive, "Nhóm M");
        KiemTraNhomMBomMau(nhomM.PhamViBom);
        var buocNhomMauId = request.BuocNhomMauId;
        var nhomMId = request.NhomMId;
        var dinhMuc = request.DinhMuc;
        var ghiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        if (await repository.TonTaiTrungAsync(request.BuocNhomMauId, request.NhomMId, id, cancellationToken))
            throw new XungDotDuLieuException("Định mức nhóm m đã tồn tại với cùng thông tin khóa.");
        entity.BuocNhomMauId = buocNhomMauId;
        entity.NhomMId = nhomMId;
        entity.DinhMuc = dinhMuc;
        entity.GhiChu = ghiChu;
        entity.MaNhomM = nhomM.MaNhomM;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy định mức nhóm M.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void KiemTraNhomMBomMau(string phamViBom)
    {
        if (!string.Equals(phamViBom, "BOM_MAU", StringComparison.Ordinal))
        {
            throw new QuyTacNghiepVuException(
                "Định mức B.O.M màu chỉ được sử dụng nhóm M thuộc phạm vi BOM_MAU.");
        }
    }

    private static BomMauDinhMucNhomMDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            BuocNhomMauId = entity.BuocNhomMauId,
            NhomMId = entity.NhomMId,
            DinhMuc = entity.DinhMuc,
            GhiChu = entity.GhiChu,
            MaNhomM = entity.MaNhomM,
            TenNhomM = entity.NhomM.TenNhomM,
            TenBuoc = entity.BuocNhomMau.TenBuoc,
            MaHonHop = entity.BuocNhomMau.MaHonHop,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
