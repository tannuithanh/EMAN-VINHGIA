namespace Eman.Application.Modules.Engineering.Bom.VatTu.Imports.Dtos;

public sealed class BomVatTuImportFileDto
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string FileName { get; init; } = "03-Import-BOM-Vat-Tu-EMAN.xlsx";
}
