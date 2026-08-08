namespace Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;

/// <summary>
/// Kết quả thực hiện import chính thức.
/// </summary>
public sealed class SanPhamImportResultDto
{
    public bool ThanhCong { get; init; }

    public string Message { get; init; } = string.Empty;

    public int TongSoDong { get; init; }

    public int SoDongDaImport { get; init; }

    public int SoDongBoQua { get; init; }

    public SanPhamImportPreviewDto? XemTruoc { get; init; }
}
