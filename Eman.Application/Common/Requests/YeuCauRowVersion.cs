using System.ComponentModel.DataAnnotations;

namespace Eman.Application.Common.Requests;

public sealed class YeuCauRowVersion
{
    [Required(ErrorMessage = "RowVersion là bắt buộc.")]
    public string RowVersion { get; init; } = string.Empty;
}
