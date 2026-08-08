using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Dtos;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;
using DeTaiEntity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.DeTai;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.MauSac;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Services;

public sealed class MauSacService(
    IMauSacRepository repository,
    IDeTaiRepository deTaiRepository,
    IUnitOfWork unitOfWork) : IMauSacService
{
    public async Task<PagedResult<MauSacDto>> LayDanhSachAsync(BoLocMauSacRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<MauSacDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<MauSacDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy màu sắc."));

    public async Task<MauSacDto> TaoMoiAsync(TaoMauSacRequest request, CancellationToken cancellationToken)
    {
        var deTai = await LayVaKiemTraDeTaiAsync(request.HeSanPhamId, request.DeTaiId, cancellationToken);
        var maMau = ChuoiHelper.ChuanHoaMa(request.MaMau);
        var tenMau = ChuoiHelper.ChuanHoaBatBuoc(request.TenMau);
        var maCotTho = ChuoiHelper.ChuanHoaMaTuyChon(request.MaCotTho);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);

        if (await repository.TonTaiTrungAsync(request.HeSanPhamId, request.DeTaiId, maMau, null, cancellationToken))
            throw new XungDotDuLieuException("Màu sắc đã tồn tại trong cùng hệ sản phẩm và đề tài.");

        var entity = new Entity
        {
            HeSanPhamId = request.HeSanPhamId,
            DeTaiId = deTai.Id,
            MaMau = maMau,
            TenMau = tenMau,
            MaCotTho = maCotTho,
            MoTa = moTa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<MauSacDto> CapNhatAsync(long id, CapNhatMauSacRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy màu sắc.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var deTai = await LayVaKiemTraDeTaiAsync(request.HeSanPhamId, request.DeTaiId, cancellationToken);
        var maMau = ChuoiHelper.ChuanHoaMa(request.MaMau);
        var tenMau = ChuoiHelper.ChuanHoaBatBuoc(request.TenMau);
        var maCotTho = ChuoiHelper.ChuanHoaMaTuyChon(request.MaCotTho);
        var moTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);

        if (await repository.TonTaiTrungAsync(request.HeSanPhamId, request.DeTaiId, maMau, id, cancellationToken))
            throw new XungDotDuLieuException("Màu sắc đã tồn tại trong cùng hệ sản phẩm và đề tài.");

        entity.HeSanPhamId = request.HeSanPhamId;
        entity.DeTaiId = deTai.Id;
        entity.MaMau = maMau;
        entity.TenMau = tenMau;
        entity.MaCotTho = maCotTho;
        entity.MoTa = moTa;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy màu sắc.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<DeTaiEntity> LayVaKiemTraDeTaiAsync(
        long heSanPhamId,
        long deTaiId,
        CancellationToken cancellationToken)
    {
        var deTai = await deTaiRepository.LayTheoIdAsync(deTaiId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đề tài.");

        if (deTai.HeSanPhamId != heSanPhamId)
            throw new QuyTacNghiepVuException("Đề tài không thuộc hệ sản phẩm đã chọn.");

        BomValidationHelper.KiemTraDangHoatDong(deTai.HeSanPham.IsActive, "Hệ sản phẩm");
        BomValidationHelper.KiemTraDangHoatDong(deTai.IsActive, "Đề tài");
        return deTai;
    }

    private static MauSacDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            HeSanPhamId = entity.HeSanPhamId,
            DeTaiId = entity.DeTaiId,
            MaMau = entity.MaMau,
            TenMau = entity.TenMau,
            MaCotTho = entity.MaCotTho,
            MoTa = entity.MoTa,
            MaHe = entity.HeSanPham.MaHe,
            MaDeTai = entity.DeTai.MaDeTai,
            TenDeTai = entity.DeTai.TenDeTai,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
