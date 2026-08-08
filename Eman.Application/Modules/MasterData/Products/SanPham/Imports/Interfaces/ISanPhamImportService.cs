using Eman.Application.Modules.MasterData.Products.SanPham.Imports.Dtos;

namespace Eman.Application.Modules.MasterData.Products.SanPham.Imports.Interfaces;

/// <summary>
/// Xử lý tải mẫu, xem trước và import sản phẩm từ Excel.
/// </summary>
public interface ISanPhamImportService
{
    Task<SanPhamImportFileDto> TaoTemplateAsync(
        CancellationToken cancellationToken = default);

    Task<SanPhamImportPreviewDto> XemTruocAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        CancellationToken cancellationToken = default);

    Task<SanPhamImportResultDto> ImportAsync(
        Stream fileStream,
        string fileName,
        long fileSize,
        string? createdByMsnv,
        CancellationToken cancellationToken = default);
}
