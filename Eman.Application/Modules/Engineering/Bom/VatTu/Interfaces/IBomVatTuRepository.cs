using Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;
using Eman.Domain.Modules.Engineering.Bom.VatTu.Enums;
using Eman.Application.Modules.Engineering.Bom.VatTu.Models;

namespace Eman.Application.Modules.Engineering.Bom.VatTu.Interfaces;

public interface IBomVatTuRepository
{
    Task<(IReadOnlyList<BomVatTuPhienBan> Items, int TotalCount)> LayDanhSachPhienBanAsync(
        Guid? vatTuId,
        string? keyword,
        TrangThaiBomVatTuPhienBan? trangThai,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<BomVatTuPhienBan?> LayPhienBanTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);
    Task<BomVatTuChiTiet?> LayChiTietTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiSoPhienBanAsync(Guid vatTuId, int soPhienBan, Guid? loaiTruId, CancellationToken cancellationToken);
    Task<bool> CoPhienBanHieuLucAsync(Guid vatTuId, Guid? loaiTruId, CancellationToken cancellationToken);
    Task<bool> TonTaiThanhPhanAsync(Guid phienBanId, Guid vatTuThanhPhanId, Guid? loaiTruId, CancellationToken cancellationToken);
    Task<IReadOnlyList<QuanHeBomVatTu>> LayQuanHeBomHieuLucAsync(CancellationToken cancellationToken);
    Task ThemPhienBanAsync(BomVatTuPhienBan entity, CancellationToken cancellationToken);
    Task ThemChiTietAsync(BomVatTuChiTiet entity, CancellationToken cancellationToken);
    void XoaPhienBan(BomVatTuPhienBan entity);
    void XoaChiTiet(BomVatTuChiTiet entity);
}
