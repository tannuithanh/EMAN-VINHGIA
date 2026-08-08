namespace Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;

public sealed class VatTuImportFileDto
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string FileName { get; init; } = "02-Import-Vat-Tu-EMAN.xlsx";
}
