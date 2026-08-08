namespace Eman.Application.Modules.MasterData.Materials.VatTu.Dtos;

public sealed record VatTuDto
{
    public Guid Id { get; init; }
    public string MaVatTu { get; init; } = string.Empty;
    public string TenVatTu { get; init; } = string.Empty;
    public string? TenTiengAnh { get; init; }

    public Guid DonViTinhId { get; init; }
    public string MaDonViTinh { get; init; } = string.Empty;
    public string TenDonViTinh { get; init; } = string.Empty;
    public string? KyHieuDonViTinh { get; init; }

    public string? QuyCachDongGoi { get; init; }
    public byte? PhamViSuDung { get; init; }
    public string? TenPhamViSuDung { get; init; }
    public IReadOnlyList<VatTuPhanXuongDto> PhanXuongs { get; init; } = [];

    public Guid NhomVatTuId { get; init; }
    public string MaNhomVatTu { get; init; } = string.Empty;
    public string TenNhomVatTu { get; init; } = string.Empty;

    public string? MucDichSuDung { get; init; }
    public byte PhuongThucCungUng { get; init; }
    public string TenPhuongThucCungUng { get; init; } = string.Empty;

    public Guid? CoSoMuaVatTuId { get; init; }
    public string? MaCoSoMuaVatTu { get; init; }
    public string? TenCoSoMuaVatTu { get; init; }

    public Guid? NhaCungCapMacDinhId { get; init; }
    public string? MaNhaCungCapMacDinh { get; init; }
    public string? TenNhaCungCapMacDinh { get; init; }

    public int? NgayMuaHang { get; init; }
    public int HanSuDungNgay { get; init; }
    public decimal? Moq { get; init; }

    public Guid? ThueVatId { get; init; }
    public string? MaThueVat { get; init; }
    public string? TenThueVat { get; init; }
    public decimal? ThueSuat { get; init; }

    public decimal? TonToiThieu { get; init; }
    public Guid? KhoLuuTruId { get; init; }
    public string? MaKhoLuuTru { get; init; }
    public string? TenKhoLuuTru { get; init; }

    public byte TrangThai { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedByMsnv { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedByMsnv { get; init; }
    public string RowVersion { get; init; } = string.Empty;
}
