using Eman.Application.Modules.MasterData.Materials.NhomVatTu.Interfaces;
using Eman.Domain.Common.Enums;
using Eman.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NhomVatTuEntity = Eman.Domain.Modules.MasterData.Materials.Entities.NhomVatTu;

namespace Eman.Infrastructure.Repositories.MasterData.Materials.NhomVatTu;

public sealed class NhomVatTuRepository(EmanDbContext dbContext) : INhomVatTuRepository
{
    public async Task<(IReadOnlyList<NhomVatTuEntity> Items, int TotalCount)> LayDanhSachAsync(
        string? keyword, TrangThaiHoatDong? trangThai, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.NhomVatTus.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var tuKhoa = keyword.Trim();
            query = query.Where(item => item.MaNhomVatTu.Contains(tuKhoa)
                || item.TenNhomVatTu.Contains(tuKhoa)
                || (item.MoTa != null && item.MoTa.Contains(tuKhoa)));
        }
        if (trangThai.HasValue) query = query.Where(item => item.TrangThai == trangThai.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(item => item.MaNhomVatTu)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }
    public Task<NhomVatTuEntity?> LayTheoIdAsync(Guid id, bool theoDoi, CancellationToken cancellationToken)
        => (theoDoi ? dbContext.NhomVatTus.AsQueryable() : dbContext.NhomVatTus.AsNoTracking())
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
    public Task<bool> TonTaiMaAsync(string maNhomVatTu, Guid? loaiTruId, CancellationToken cancellationToken)
        => dbContext.NhomVatTus.AnyAsync(item => item.MaNhomVatTu == maNhomVatTu
            && (!loaiTruId.HasValue || item.Id != loaiTruId.Value), cancellationToken);
    public Task<bool> DangDuocSuDungAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.VatTus.AnyAsync(item => item.NhomVatTuId == id, cancellationToken);
    public Task ThemAsync(NhomVatTuEntity entity, CancellationToken cancellationToken)
        => dbContext.NhomVatTus.AddAsync(entity, cancellationToken).AsTask();
    public void Xoa(NhomVatTuEntity entity) => dbContext.NhomVatTus.Remove(entity);
}
