using System.Globalization;
using System.Text.Json;

namespace HGT.EAM.Client.Domain.Models;

public static class MapperHelper
{
    public static string GetSafeString(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "",
            _ => element.GetRawText()
        };
    }

    public static decimal? ParseDecimal(string? s, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        var cleaned = s.Trim().Replace(" ", "");
        var targetCulture = culture ?? CultureInfo.InvariantCulture;

        // Lógica de limpieza legacy si no hay cultura específica o para mayor robustez
        if (culture == null)
        {
            if (cleaned.Contains(',') && !cleaned.Contains('.'))
                cleaned = cleaned.Replace(',', '.');
            else if (cleaned.Contains('.') && cleaned.Contains(','))
                cleaned = cleaned.Replace(".", "").Replace(',', '.');
        }

        if (decimal.TryParse(cleaned, NumberStyles.Any, targetCulture, out var result))
            return result;

        return null;
    }

    public static long? ParseLong(string? s, CultureInfo? culture = null)
    {
        var d = ParseDecimal(s, culture);
        return d.HasValue ? (long)Math.Truncate(d.Value) : null;
    }

    public static DateTime? ParseDate(string? s, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        if (culture != null && DateTime.TryParse(s, culture, DateTimeStyles.AssumeLocal, out var dt))
            return dt;

        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
            return dt;

        if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dt))
            return dt;

        return null;
    }
}
