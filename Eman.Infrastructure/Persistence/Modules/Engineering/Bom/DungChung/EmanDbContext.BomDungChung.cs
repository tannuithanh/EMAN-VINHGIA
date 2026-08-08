using Eman.Domain.Modules.Engineering.Bom.DungChung.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

public sealed partial class EmanDbContext
{
    public DbSet<HeSanPham> HeSanPhams => Set<HeSanPham>();
    public DbSet<DeTai> DeTais => Set<DeTai>();
    public DbSet<MauSac> MauSacs => Set<MauSac>();
    public DbSet<HinhDang> HinhDangs => Set<HinhDang>();
    public DbSet<MaHang> MaHangs => Set<MaHang>();
    public DbSet<NhomM> NhomMs => Set<NhomM>();
    public DbSet<QuyTacNhomM> QuyTacNhomMs => Set<QuyTacNhomM>();
}
