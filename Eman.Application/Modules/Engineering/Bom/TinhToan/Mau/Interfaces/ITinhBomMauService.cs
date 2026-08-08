using Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Dtos;

namespace Eman.Application.Modules.Engineering.Bom.TinhToan.Mau.Interfaces;

public interface ITinhBomMauService
{
    Task<KetQuaKiemThuBomMauDto> KiemThuAsync(
        string maSanPham,
        CancellationToken cancellationToken);
}
