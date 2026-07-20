using Eman.Application.Contracts.HeThong;
using Eman.Application.Dtos.HeThong;

namespace Eman.Infrastructure.Services.HeThong;

public sealed class ThongTinHeThongService : IThongTinHeThongService
{
    public ThongTinHeThongDto LayThongTin()
        => new(
            TenHeThong: "EMAN",
            TrangThai: "Đang hoạt động",
            PhienBan: "1.0.0",
            ThoiGianMayChuUtc: DateTime.UtcNow);
}
