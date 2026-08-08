using Eman.Domain.Modules.Engineering.Bom.Mau.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

public sealed partial class EmanDbContext
{
    public DbSet<BomMauBuoc> BomMauBuocs => Set<BomMauBuoc>();
    public DbSet<BuocNhomTheoMau> BuocNhomTheoMaus => Set<BuocNhomTheoMau>();
    public DbSet<BomMauDinhMucNhomM> BomMauDinhMucNhomMs => Set<BomMauDinhMucNhomM>();
    public DbSet<BomMauHeSoDeTai> BomMauHeSoDeTais => Set<BomMauHeSoDeTai>();
    public DbSet<BomMauHeSoMau> BomMauHeSoMaus => Set<BomMauHeSoMau>();
    public DbSet<BomMaHangPhen> BomMaHangPhens => Set<BomMaHangPhen>();
    public DbSet<ChauInsert> ChauInserts => Set<ChauInsert>();
    public DbSet<BomMaHangChauInsert> BomMaHangChauInserts => Set<BomMaHangChauInsert>();
}
