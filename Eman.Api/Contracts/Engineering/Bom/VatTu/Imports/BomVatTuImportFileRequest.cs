using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Eman.Api.Contracts.Engineering.Bom.VatTu.Imports;

public sealed class BomVatTuImportFileRequest
{
    [Required(ErrorMessage = "Vui lòng chọn file Excel B.O.M vật tư cần import.")]
    public IFormFile File { get; init; } = null!;

    [MaxLength(50, ErrorMessage = "Mã nhân viên người import không được vượt quá 50 ký tự.")]
    public string? CreatedByMsnv { get; init; }
}
