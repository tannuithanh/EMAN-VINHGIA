using Eman.Domain.Modules.MasterData.BusinessPartners.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

/// <summary>
/// Các DbSet thuộc Master Data - Đối tác kinh doanh.
/// </summary>
public sealed partial class EmanDbContext
{
    public DbSet<LoaiDoiTac> LoaiDoiTacs => Set<LoaiDoiTac>();

    public DbSet<DoiTacKinhDoanh> DoiTacKinhDoanhs => Set<DoiTacKinhDoanh>();

    public DbSet<DieuKienThanhToan> DieuKienThanhToans => Set<DieuKienThanhToan>();

    public DbSet<DieuKienGiaoHang> DieuKienGiaoHangs => Set<DieuKienGiaoHang>();

    public DbSet<BangGia> BangGias => Set<BangGia>();

    public DbSet<PhienBanBangGia> PhienBanBangGias => Set<PhienBanBangGia>();
}
