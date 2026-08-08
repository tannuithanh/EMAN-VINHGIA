using Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Dtos;
using Entity = Eman.Domain.Modules.Engineering.Bom.DungChung.Entities.QuyTacNhomM;

namespace Eman.Application.Modules.Engineering.Bom.DungChung.QuyTacNhomM.Interfaces;

public interface IQuyTacNhomMRepository
{
    Task<(IReadOnlyList<Entity> Items, int TotalCount)> LayDanhSachAsync(BoLocQuyTacNhomMRequest request, CancellationToken cancellationToken);
    Task<Entity?> LayTheoIdAsync(long id, bool theoDoi, CancellationToken cancellationToken);
    Task<bool> TonTaiTrungAsync(long hinhDangId, long nhomMId, long? loaiTruId, CancellationToken cancellationToken);
    Task<bool> TonTaiKhoangChongLanAsync(
        string phamViBom,
        long hinhDangId,
        decimal dienTichTu,
        decimal? dienTichDen,
        bool baoGomTu,
        bool baoGomDen,
        long? loaiTruId,
        CancellationToken cancellationToken);
    Task ThemAsync(Entity entity, CancellationToken cancellationToken);
    void Xoa(Entity entity);
}
