namespace HGT.EAM.Client.Configuration;

public sealed class ApiSettings
{
    public string? BaseUrl { get; set; }
    public Dictionary<string, ApiCredentials>? Credentials { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
}

public sealed class ApiCredentials
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Culture { get; set; }
}

public class BaseQuery
{
    public Configuration.ApiCredentials? Credentials { get; set; }
    public string? TypeFilter { get; set; } = "5";
    public int Month { get; set; } = DateTime.UtcNow.Month;
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;

    public Dictionary<string, string?> ToQueryDictionary()
    {
        return new Dictionary<string, string?>
        {
            ["typeFilter"] = TypeFilter,
            ["month"] = Month.ToString(),
            ["year"] = Year.ToString(),
            ["page"] = Page.ToString(),
            ["pagSize"] = PageSize.ToString() // Watch out for 'pagSize' because that is how the endpoint expects it
        };
    }
}

public sealed class ProvisionQuery : BaseQuery { }

public sealed class PurchaseOrderAuditQuery : BaseQuery { }

public static class ApiEndpoints
{
    public const string Provisions = "api/management-control/provisions";
    public const string PurchaseOrderAudit = "api/provisions/purchase/order/audit";

    public static string BuildUrl(this BaseQuery query, string baseUrl, string endpoint)
    {
        var root = baseUrl.TrimEnd('/');
        var typeFilter = Uri.EscapeDataString(query.TypeFilter ?? string.Empty);
        return $"{root}/{endpoint}?typeFilter={typeFilter}&month={query.Month}&year={query.Year}&page={query.Page}&pagSize={query.PageSize}";
    }
}

