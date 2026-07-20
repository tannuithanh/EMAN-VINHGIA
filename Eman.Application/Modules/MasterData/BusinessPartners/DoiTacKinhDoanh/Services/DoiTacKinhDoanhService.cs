using Eman.Application.Common;
using Eman.Application.Common.Exceptions;
using Eman.Application.Common.Helpers;
using Eman.Application.Common.Persistence;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Dtos;
using Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Interfaces;
using Eman.Application.Modules.MasterData.BusinessPartners.LoaiDoiTac.Interfaces;
using Eman.Domain.Common.Enums;
using DoiTacKinhDoanhEntity = Eman.Domain.Modules.MasterData.BusinessPartners.Entities.DoiTacKinhDoanh;

namespace Eman.Application.Modules.MasterData.BusinessPartners.DoiTacKinhDoanh.Services;

public sealed class DoiTacKinhDoanhService(
    IDoiTacKinhDoanhRepository repository,
    ILoaiDoiTacRepository loaiDoiTacRepository,
    IUnitOfWork unitOfWork) : IDoiTacKinhDoanhService
{
    public async Task<PagedResult<DoiTacKinhDoanhDto>> LayDanhSachAsync(
        BoLocDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = request.TrangThai.HasValue
            ? (TrangThaiHoatDong?)request.TrangThai.Value
            : null;

        var (items, totalCount) = await repository.LayDanhSachAsync(
            request.Keyword,
            request.LoaiDoiTacId,
            request.LaNhaCungCap,
            trangThai,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<DoiTacKinhDoanhDto>
        {
            Items = items.Select(ChuyenDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<DoiTacKinhDoanhDto> LayTheoIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, false, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đối tác kinh doanh.");

        return ChuyenDto(entity);
    }

    public async Task<DoiTacKinhDoanhDto> TaoMoiAsync(
        TaoDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken)
    {
        await KiemTraLoaiDoiTacAsync(request.LoaiDoiTacId, cancellationToken);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaDoiTac);
        if (await repository.TonTaiMaAsync(ma, null, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã đối tác '{ma}' đã tồn tại.");
        }

        var entity = new DoiTacKinhDoanhEntity
        {
            MaDoiTac = ma,
            TenDoiTac = ChuoiHelper.ChuanHoaBatBuoc(request.TenDoiTac),
            LoaiDoiTacId = request.LoaiDoiTacId,
            LaNhaCungCap = request.LaNhaCungCap,
            MaSoThue = ChuoiHelper.ChuanHoaTuyChon(request.MaSoThue),
            DiaChi = ChuoiHelper.ChuanHoaTuyChon(request.DiaChi),
            NguoiLienHe = ChuoiHelper.ChuanHoaTuyChon(request.NguoiLienHe),
            DienThoai = ChuoiHelper.ChuanHoaTuyChon(request.DienThoai),
            Email = ChuoiHelper.ChuanHoaTuyChon(request.Email),
            SoTaiKhoan = ChuoiHelper.ChuanHoaTuyChon(request.SoTaiKhoan),
            TenNganHang = ChuoiHelper.ChuanHoaTuyChon(request.TenNganHang),
            TrangThai = TrangThaiHoatDong.HoatDong
        };

        await repository.ThemAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task<DoiTacKinhDoanhDto> CapNhatAsync(
        Guid id,
        CapNhatDoiTacKinhDoanhRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đối tác kinh doanh.");

        RowVersionHelper.KiemTra(request.RowVersion, entity.RowVersion);
        await KiemTraLoaiDoiTacAsync(request.LoaiDoiTacId, cancellationToken);

        var ma = ChuoiHelper.ChuanHoaMa(request.MaDoiTac);
        if (await repository.TonTaiMaAsync(ma, id, cancellationToken))
        {
            throw new XungDotDuLieuException($"Mã đối tác '{ma}' đã tồn tại.");
        }

        if (entity.LaNhaCungCap && !request.LaNhaCungCap &&
            await repository.CoBangGiaAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể bỏ đánh dấu nhà cung cấp vì đối tác đã có bảng giá.");
        }

        entity.MaDoiTac = ma;
        entity.TenDoiTac = ChuoiHelper.ChuanHoaBatBuoc(request.TenDoiTac);
        entity.LoaiDoiTacId = request.LoaiDoiTacId;
        entity.LaNhaCungCap = request.LaNhaCungCap;
        entity.MaSoThue = ChuoiHelper.ChuanHoaTuyChon(request.MaSoThue);
        entity.DiaChi = ChuoiHelper.ChuanHoaTuyChon(request.DiaChi);
        entity.NguoiLienHe = ChuoiHelper.ChuanHoaTuyChon(request.NguoiLienHe);
        entity.DienThoai = ChuoiHelper.ChuanHoaTuyChon(request.DienThoai);
        entity.Email = ChuoiHelper.ChuanHoaTuyChon(request.Email);
        entity.SoTaiKhoan = ChuoiHelper.ChuanHoaTuyChon(request.SoTaiKhoan);
        entity.TenNganHang = ChuoiHelper.ChuanHoaTuyChon(request.TenNganHang);
        entity.TrangThai = (TrangThaiHoatDong)request.TrangThai;
        entity.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await LayTheoIdAsync(entity.Id, cancellationToken);
    }

    public async Task XoaAsync(
        Guid id,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var entity = await repository.LayTheoIdAsync(id, true, cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy đối tác kinh doanh.");

        RowVersionHelper.KiemTra(rowVersion, entity.RowVersion);

        if (await repository.CoBangGiaAsync(id, cancellationToken))
        {
            throw new QuyTacNghiepVuException(
                "Không thể xóa đối tác kinh doanh vì đã có bảng giá.");
        }

        repository.Xoa(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task KiemTraLoaiDoiTacAsync(
        Guid loaiDoiTacId,
        CancellationToken cancellationToken)
    {
        if (loaiDoiTacId == Guid.Empty)
        {
            throw new QuyTacNghiepVuException("Loại đối tác là bắt buộc.");
        }

        var loaiDoiTac = await loaiDoiTacRepository.LayTheoIdAsync(
            loaiDoiTacId,
            false,
            cancellationToken)
            ?? throw new KhongTimThayException("Không tìm thấy loại đối tác.");

        if (loaiDoiTac.TrangThai != TrangThaiHoatDong.HoatDong)
        {
            throw new QuyTacNghiepVuException("Loại đối tác đã ngừng hoạt động.");
        }
    }

    private static DoiTacKinhDoanhDto ChuyenDto(DoiTacKinhDoanhEntity entity)
        => new(
            entity.Id,
            entity.MaDoiTac,
            entity.TenDoiTac,
            entity.LoaiDoiTacId,
            entity.LoaiDoiTac.MaLoaiDoiTac,
            entity.LoaiDoiTac.TenLoaiDoiTac,
            entity.LaNhaCungCap,
            entity.MaSoThue,
            entity.DiaChi,
            entity.NguoiLienHe,
            entity.DienThoai,
            entity.Email,
            entity.SoTaiKhoan,
            entity.TenNganHang,
            (byte)entity.TrangThai,
            entity.CreatedAt,
            entity.UpdatedAt,
            RowVersionHelper.ChuyenThanhChuoi(entity.RowVersion));
}
