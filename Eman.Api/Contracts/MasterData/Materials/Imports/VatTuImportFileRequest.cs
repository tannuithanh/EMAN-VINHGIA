using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Contracts.MasterData.Materials.Imports;

public sealed class VatTuImportFileRequest
{
    [Required(ErrorMessage = "Vui lòng chọn file Excel cần import.")]
    public IFormFile File { get; init; } = null!;

    [MaxLength(50, ErrorMessage = "Mã nhân viên người import không được vượt quá 50 ký tự.")]
    public string? CreatedByMsnv { get; init; }
}
