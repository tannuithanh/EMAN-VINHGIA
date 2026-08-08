using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Dtos;
using Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Interfaces;
using Eman.Domain.Common.Enums;
using CoSoMuaVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.CoSoMuaVatTu;

namespace Eman.Application.Modules.MasterData.Materials.CoSoMuaVatTu.Services;

public sealed class CoSoMuaVatTuService(ICoSoMuaVatTuRepository repository, IUnitOfWork unitOfWork)
    : ICoSoMuaVatTuService
{
    public async Task<PagedResult<CoSoMuaVatTuDto>> LayDanhSachAsync(
        BoLocCoSoMuaVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;
        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword, trangThai, request.Page, request.PageSize, cancellationToken);
        return new PagedResult<CoSoMuaVatTuDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CoSoMuaVatTuDto> LayTheoIdAsync(Guid id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy cơ sở mua vật tư."));

    public async Task<CoSoMuaVatTuDto> TaoMoiAsync(
        TaoCoSoMuaVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var ma = ChuoiHelper.ChuanHoaMa(request.MaCoSoMuaVatTu);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã cơ sở mua vật tư '{ma}' đã tồn tại.");
        }
        var entity = new CoSoMuaVatTuEntity
        {
            MaCoSoMuaVatTu = ma,
            TenCoSoMuaVatTu = ChuoiHelper.ChuanHoaBatBuoc(request.TenCoSoMuaVatTu),
            MoTa = ChuoiHelper.ChuanHoaTuyChon(request.MoTa),
            TrangThai = TrangThaiHoatDong.HoatDong,
            CreatedAt = DateTime.UtcNow,
            CreatedByMsnv = ChuoiHelper.ChuanHoaTuyChon(request.CreatedByMsnv)
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ChuyenDto(entity);
    }

    public async Task<CoSoMuaVatTuDto> CapNhatAsync(
        Guid id,
        CapNhatCoSoMuaVatTuRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy cơ sở mua vật tư.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var ma = ChuoiHelper.ChuanHoaMa(request.MaCoSoMuaVatTu);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã cơ sở mua vật tư '{ma}' đã tồn tại.");
        }
        entity.MaCoSoMuaVatTu = ma;
        entity.TenCoSoMuaVatTu = ChuoiHelper.ChuanHoaBatBuoc(request.TenCoSoMuaVatTu);
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
            ?? throw new KhongTimThayException("Không tìm thấy cơ sở mua vật tư.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        if (await repository.DangDuocSuDungAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể xóa cơ sở mua vật tư vì đang được sử dụng trong danh mục vật tư.");
        }
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static CoSoMuaVatTuDto ChuyenDto(CoSoMuaVatTuEntity entity) => new()
    {
        Id = entity.Id,
        MaCoSoMuaVatTu = entity.MaCoSoMuaVatTu,
        TenCoSoMuaVatTu = entity.TenCoSoMuaVatTu,
        MoTa = entity.MoTa,
        TrangThai = (byte)entity.TrangThai,
        CreatedAt = entity.CreatedAt,
        CreatedByMsnv = entity.CreatedByMsnv,
        UpdatedAt = entity.UpdatedAt,
        UpdatedByMsnv = entity.UpdatedByMsnv,
        RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
    };
}
