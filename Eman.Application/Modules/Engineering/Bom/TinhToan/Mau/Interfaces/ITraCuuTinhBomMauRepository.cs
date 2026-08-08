using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Models;

namespace Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Interfaces;

public interface ITraCuuTinhBomMauRepository
{
    Task<HeVaDeTaiTraCuuBomMau?> LayHeVaDeTaiAsync(
        string maHe,
        CancellationToken cancellationToken);

    Task<MauSacTraCuuBomMau?> LayMauSacAsync(
        long heSanPhamId,
        long deTaiId,
        string maMau,
        CancellationToken cancellationToken);

    Task<MaHangTraCuuBomMau?> LayMaHangAsync(
        string maHang,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<QuyTacNhomMTraCuuBomMau>> LayCacQuyTacNhomMAsync(
        long hinhDangBomMauId,
        CancellationToken cancellationToken);

    Task<GoiDuLieuBuocTraCuuBomMau> LayGoiDuLieuBuocAsync(
        long heSanPhamId,
        long deTaiId,
        long mauSacId,
        long nhomMId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ChauInsertTraCuuBomMau>> LayChauInsertsAsync(
        long maHangId,
        CancellationToken cancellationToken);

    Task<PhenTraCuuBomMau?> LayPhenAsync(
        long maHangId,
        CancellationToken cancellationToken);
}
