namespace Eman.Application.Modules.MasterData.Materials.VatTu.Exports.Dtos;

public sealed class VatTuExportFileDto
{
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string FileName { get; init; } = string.Empty;
}
