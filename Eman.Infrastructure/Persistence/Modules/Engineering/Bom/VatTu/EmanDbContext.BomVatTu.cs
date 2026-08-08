using Eman.Domain.Modules.Engineering.Bom.VatTu.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eman.Infrastructure.Persistence;

public sealed partial class EmanDbContext
{
    public DbSet<BomVatTuPhienBan> BomVatTuPhienBans => Set<BomVatTuPhienBan>();
    public DbSet<BomVatTuChiTiet> BomVatTuChiTiets => Set<BomVatTuChiTiet>();
}
