namespace Eman.Application.Common.Helpers;

public static class ChuoiHelper
{
    public static string ChuanHoaMa(string value)
        => value.Trim().ToUpperInvariant();

    public static string ChuanHoaBatBuoc(string value)
        => value.Trim();

    public static string? ChuanHoaMaTuyChon(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : ChuanHoaMa(value);

    public static string? ChuanHoaTuyChon(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
