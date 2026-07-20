using Eman.Application.Common.Exceptions;

namespace Eman.Application.Common.Helpers;

public static class RowVersionHelper
{
    public static string ChuyenThanhChuoi(byte[] rowVersion)
        => Convert.ToBase64String(rowVersion);

    public static void KiemTra(string rowVersionGuiLen, byte[] rowVersionHienTai)
    {
        byte[] rowVersion;

        try
        {
            rowVersion = Convert.FromBase64String(rowVersionGuiLen);
        }
        catch (FormatException)
        {
            throw new QuyTacNghiepVuException("RowVersion không đúng định dạng Base64.");
        }

        if (!rowVersion.SequenceEqual(rowVersionHienTai))
        {
            throw new XungDotDuLieuException(
                "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại dữ liệu trước khi thao tác.");
        }
    }
}
