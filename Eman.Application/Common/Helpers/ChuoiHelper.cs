namespace Eman.Application.Common.Helpers;

public static class ChuoiHelper
{
    public static string ChuanHoaMa(string value)
        => value.Trim().ToUpperInvariant();

    public static string ChuanHoaBatBuoc(string value)
        => value.Trim();

    public static string? ChuanHoaTuyChon(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
