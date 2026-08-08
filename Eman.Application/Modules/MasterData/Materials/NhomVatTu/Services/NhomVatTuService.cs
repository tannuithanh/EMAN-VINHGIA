using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Dtos;
using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Interfaces;
using Eman.Domain.Common.Enums;
using NhomVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.NhomVatTu;

namespace Eman.Application.Modules.MasterData.Materials.NhomVatTu.Services;

public sealed class NhomVatTuService(INhomVatTuRepository repository, IUnitOfWork unitOfWork)
    : INhomVatTuService
{
    public async Task<PagedResult<NhomVatTuDto>> LayDanhSachAsync(
        BoLocNhomVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;
        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword, trangThai, request.Page, request.PageSize, cancellationToken);

        return new PagedResult<NhomVatTuDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<NhomVatTuDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm vật tư."));

    public async Task<NhomVatTuDto> TaoMoiAsync(
        TaoNhomVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaNhomVatTu);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã nhóm vật tư '{ma}' đã tồn tại.");
        }

        var entity = new NhomVatTuEntity
        {
            MaNhomVatTu = ma,
            TenNhomVatTu = ChuoiHelper.ChuanHoaBatBuoc(request.TenNhomVatTu),
            MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa),
            TrangThai = TrangThaiHoatDong.HoatDong,
            CreatedAt = DateTime.UtcNow,
            CreatedByMsnv = ChuoiHelper.ChuanHoaTuyChon(request.CreatedByMsnv)
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<NhomVatTuDto> CapNhatAsync(
        Guid id,
        CapNhatNhomVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm vật tư.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaNhomVatTu);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã nhóm vật tư '{ma}' đã tồn tại.");
        }

        entity.MaNhomVatTu = ma;
        entity.TenNhomVatTu = ChuoiHelper.ChuanHoaBatBuoc(request.TenNhomVatTu);
        entity.MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa);
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByMsnv = ChuoiHelper.ChuanHoaTuyChon(request.UpdatedByMsnv);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task XoaAsync(Guid id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy nhóm vật tư.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        if (await repository.DangDuocSuDungAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể xóa nhóm vật tư vì đang được sử dụng trong danh mục vật tư.");
        }
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static NhomVatTuDto ChuyenDto(NhomVatTuEntity entity) => new()
    {
        Id = entity.Id,
        MaNhomVatTu = entity.MaNhomVatTu,
        TenNhomVatTu = entity.TenNhomVatTu,
        MoTa = entity.MoTa,
        TrangThai = (byte)entity.TrangThai,
        CreatedAt = entity.CreatedAt,
        CreatedByMsnv = entity.CreatedByMsnv,
        UpdatedAt = entity.UpdatedAt,
        UpdatedByMsnv = entity.UpdatedByMsnv,
        RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
    };
}
