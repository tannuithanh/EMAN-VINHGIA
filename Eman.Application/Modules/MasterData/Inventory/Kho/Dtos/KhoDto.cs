namespace Eman.Application.Modules.MasterData.Inventory.Kho.Dtos;

public sealed record KhoDto(
    Guid Id,
    string MaKho,
    string TenKho,
    bool HangTon,
    bool HangTru,
    string? MoTa,
    byte TrangThai,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion);
