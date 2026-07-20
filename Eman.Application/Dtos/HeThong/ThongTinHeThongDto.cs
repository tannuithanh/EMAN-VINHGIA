namespace Eman.Application.Dtos.HeThong;

/// <summary>
/// Thông tin kiểm tra trạng thái chạy của dịch vụ EMAN.
/// </summary>
public sealed record ThongTinHeThongDto(
    string TenHeThong,
    string TrangThai,
    string PhienBan,
    DateTime ThoiGianMayChuUtc);
