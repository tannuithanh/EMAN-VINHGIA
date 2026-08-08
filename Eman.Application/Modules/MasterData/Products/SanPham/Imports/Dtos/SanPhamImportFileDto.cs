namespace Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;

/// <summary>
/// Nội dung file mẫu import sản phẩm.
/// </summary>
public sealed class SanPhamImportFileDto
{
    public byte[] Content { get; init; } = Array.Empty<byte>();

    public string ContentType { get; init; } =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public string FileName { get; init; } = "01-Import-San-Pham-EMAN.xlsx";
}
