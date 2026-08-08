using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.Engineering.Bom.Common;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauBuoc.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.DeTai.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.HeSanPham.Interfaces;
using Eman.Application.Modules.Engineering.Bom.DungChung.MauSac.Interfaces;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Dtos;
using Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Interfaces;
using Entity = Eman.Domain.Modules.Engineering.Bom.Mau.Entities.BomMauHeSoMau;

namespace Eman.Application.Modules.Engineering.Bom.Mau.BomMauHeSoMau.Services;

public sealed class BomMauHeSoMauService(
    IBomMauHeSoMauRepository repository,
    IHeSanPhamRepository heSanPhamRepository,
    IDeTaiRepository deTaiRepository,
    IMauSacRepository mauSacRepository,
    IBomMauBuocRepository buocRepository,
    IUnitOfWork unitOfWork) : IBomMauHeSoMauService
{
    public async Task<PagedResult<BomMauHeSoMauDto>> LayDanhSachAsync(BoLocBomMauHeSoMauRequest request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.LayDanhSachAsync(request, cancellationToken);
        return new PagedResult<BomMauHeSoMauDto> { Items = items.Select(ChuyenDto).ToList(), Page = request.Page, PageSize = request.PageSize, TotalCount = totalCount };
    }

    public async Task<BomMauHeSoMauDto> LayTheoIdAsync(long id, CancellationToken cancellationToken)
        => ChuyenDto(await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ số màu."));

    public async Task<BomMauHeSoMauDto> TaoMoiAsync(TaoBomMauHeSoMauRequest request, CancellationToken cancellationToken)
    {
        var heSanPham = await heSanPhamRepository.LayTheoIdAsync(request.HeSanPhamId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ sản phẩm.");
        var deTai = await deTaiRepository.LayTheoIdAsync(request.DeTaiId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đề tài.");
        var mauSac = await mauSacRepository.LayTheoIdAsync(request.MauSacId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy màu sắc.");
        var buoc = await buocRepository.LayTheoIdAsync(request.BuocId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước B.O.M màu.");
        if (deTai.HeSanPhamId != request.HeSanPhamId)
            throw new QuyTacNghiepVuException("Đề tài không thuộc hệ sản phẩm đã chọn.");
        if (mauSac.HeSanPhamId != request.HeSanPhamId || mauSac.DeTaiId != request.DeTaiId)
            throw new QuyTacNghiepVuException("Màu sắc không thuộc hệ sản phẩm và đề tài đã chọn.");
        BomValidationHelper.KiemTraDangHoatDong(heSanPham.IsActive, "Hệ sản phẩm");
        BomValidationHelper.KiemTraDangHoatDong(deTai.IsActive, "Đề tài");
        BomValidationHelper.KiemTraDangHoatDong(mauSac.IsActive, "Màu sắc");
        BomValidationHelper.KiemTraDangHoatDong(buoc.IsActive, "Bước B.O.M màu");
        var heSanPhamId = request.HeSanPhamId;
        var deTaiId = request.DeTaiId;
        var mauSacId = request.MauSacId;
        var buocId = request.BuocId;
        var heSo = request.HeSo;
        var ghiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        if (await repository.TonTaiTrungAsync(request.HeSanPhamId, request.DeTaiId, request.MauSacId, request.BuocId, null, cancellationToken))
            throw new XungDotDuLieuException("Hệ số màu đã tồn tại với cùng thông tin khóa.");
        var entity = new Entity
        {
            HeSanPhamId = heSanPhamId,
            DeTaiId = deTaiId,
            MauSacId = mauSacId,
            BuocId = buocId,
            HeSo = heSo,
            GhiChu = ghiChu,
            MaHe = heSanPham.MaHe,
            MaDeTai = deTai.MaDeTai,
            MaMau = mauSac.MaMau,
            TenMau = mauSac.TenMau,
            TenBuoc = buoc.TenBuoc,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<BomMauHeSoMauDto> CapNhatAsync(long id, CapNhatBomMauHeSoMauRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ số màu.");
        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        var heSanPham = await heSanPhamRepository.LayTheoIdAsync(request.HeSanPhamId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ sản phẩm.");
        var deTai = await deTaiRepository.LayTheoIdAsync(request.DeTaiId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đề tài.");
        var mauSac = await mauSacRepository.LayTheoIdAsync(request.MauSacId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy màu sắc.");
        var buoc = await buocRepository.LayTheoIdAsync(request.BuocId, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy bước B.O.M màu.");
        if (deTai.HeSanPhamId != request.HeSanPhamId)
            throw new QuyTacNghiepVuException("Đề tài không thuộc hệ sản phẩm đã chọn.");
        if (mauSac.HeSanPhamId != request.HeSanPhamId || mauSac.DeTaiId != request.DeTaiId)
            throw new QuyTacNghiepVuException("Màu sắc không thuộc hệ sản phẩm và đề tài đã chọn.");
        BomValidationHelper.KiemTraDangHoatDong(heSanPham.IsActive, "Hệ sản phẩm");
        BomValidationHelper.KiemTraDangHoatDong(deTai.IsActive, "Đề tài");
        BomValidationHelper.KiemTraDangHoatDong(mauSac.IsActive, "Màu sắc");
        BomValidationHelper.KiemTraDangHoatDong(buoc.IsActive, "Bước B.O.M màu");
        var heSanPhamId = request.HeSanPhamId;
        var deTaiId = request.DeTaiId;
        var mauSacId = request.MauSacId;
        var buocId = request.BuocId;
        var heSo = request.HeSo;
        var ghiChu = ChuoiHelper.ChuanHoaTuyChon(request.GhiChu);
        if (await repository.TonTaiTrungAsync(request.HeSanPhamId, request.DeTaiId, request.MauSacId, request.BuocId, id, cancellationToken))
            throw new XungDotDuLieuException("Hệ số màu đã tồn tại với cùng thông tin khóa.");
        entity.HeSanPhamId = heSanPhamId;
        entity.DeTaiId = deTaiId;
        entity.MauSacId = mauSacId;
        entity.BuocId = buocId;
        entity.HeSo = heSo;
        entity.GhiChu = ghiChu;
        entity.MaHe = heSanPham.MaHe;
        entity.MaDeTai = deTai.MaDeTai;
        entity.MaMau = mauSac.MaMau;
        entity.TenMau = mauSac.TenMau;
        entity.TenBuoc = buoc.TenBuoc;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(long id, string rowVersion, CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy hệ số màu.");
        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);
        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static BomMauHeSoMauDto ChuyenDto(Entity entity)
        => new()
        {
            Id = entity.Id,
            HeSanPhamId = entity.HeSanPhamId,
            DeTaiId = entity.DeTaiId,
            MauSacId = entity.MauSacId,
            BuocId = entity.BuocId,
            HeSo = entity.HeSo,
            GhiChu = entity.GhiChu,
            MaHe = entity.MaHe,
            MaDeTai = entity.MaDeTai,
            MaMau = entity.MaMau,
            TenMau = entity.TenMau,
            MaBuoc = entity.Buoc.MaBuoc,
            TenBuoc = entity.TenBuoc,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion)
        };
}
