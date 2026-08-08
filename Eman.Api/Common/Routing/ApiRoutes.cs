namespace Eman.Api.Common.Routing;

/// <summary>
/// Tiền tố route dùng chung theo miền dữ liệu của EMAN.
/// </summary>
internal static class ApiRoutes
{
    public const string MasterData = "api/master-data";

    public const string Transactions = "api/transactions";

    public const string Engineering = "api/engineering";

    public const string EngineeringBom = Engineering + "/bom";

    public const string EngineeringBomDungChung = EngineeringBom + "/dung-chung";

    public const string EngineeringBomMau = EngineeringBom + "/mau";

    public const string EngineeringBomVatTu = EngineeringBom + "/vat-tu";

    public const string EngineeringBomTinhToan = EngineeringBom + "/tinh-toan";

}
