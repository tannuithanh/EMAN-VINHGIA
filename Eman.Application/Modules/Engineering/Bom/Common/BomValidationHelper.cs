using Eman.Application.Common.Exceptions;

namespace Eman.Application.Modules.Engineering.Bom.Common;

internal static class BomValidationHelper
{
    public static void KiemTraDangHoatDong(bool isActive, string tenDanhMuc)
    {
        if (!isActive)
        {
            throw new QuyTacNghiepVuException($"{tenDanhMuc} đã ngừng hoạt động.");
        }
    }

    public static string ChuanHoaPhamViBom(string value)
    {
        var phamVi = value.Trim().ToUpperInvariant();
        if (phamVi is not ("BOM_MAU" or "BOM_THO"))
        {
            throw new QuyTacNghiepVuException("Phạm vi B.O.M chỉ nhận BOM_MAU hoặc BOM_THO.");
        }
        return phamVi;
    }

    public static void KiemTraKhoangDienTich(decimal? tu, decimal? den)
    {
        if (tu < 0 || den < 0)
        {
            throw new QuyTacNghiepVuException("Diện tích không được nhỏ hơn 0.");
        }
        if (tu.HasValue && den.HasValue && tu.Value >= den.Value)
        {
            throw new QuyTacNghiepVuException("Diện tích bắt đầu phải nhỏ hơn diện tích kết thúc.");
        }
    }
}
