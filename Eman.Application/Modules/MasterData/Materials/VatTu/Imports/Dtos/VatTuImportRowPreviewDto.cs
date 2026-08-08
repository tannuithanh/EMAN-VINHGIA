namespace Eman.Application.Modules.MasterData.Materials.VatTu.Imports.Dtos;

public sealed class VatTuImportRowPreviewDto
{
    public int Dong { get; init; }
    public string? MaVatTu { get; init; }
    public string? TenVatTu { get; init; }
    public string? MaDonViTinh { get; init; }
    public string? MaNhomVatTu { get; init; }
    public byte? PhamViSuDung { get; init; }
    public byte? PhuongThucCungUng { get; init; }
    public string? MaCoSoMuaVatTu { get; init; }
    public string? MaNhaCungCapMacDinh { get; init; }
    public string? MaThueVat { get; init; }
    public string? MaKhoLuuTru { get; init; }
    public IReadOnlyList<string> MaPhanXuongs { get; init; } = Array.Empty<string>();
    public bool HopLe => Loi.Count == 0;
    public IReadOnlyList<string> Loi { get; init; } = Array.Empty<string>();
}
