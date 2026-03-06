using System.Globalization;
using System.ComponentModel.DataAnnotations;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HGT.EAM.Client.Domain.Models;

public class PurchaseOrderAudit
{
    [Key]
    public long Id { get; set; }

    [JsonPropertyName("action_type")]         public string?   ActionType      { get; set; }
    [JsonPropertyName("changed_at")]          public DateTime? ChangedAt       { get; set; }
    [JsonPropertyName("changed_by")]          public string?   ChangedBy       { get; set; }
    [JsonPropertyName("column_text_display")] public string?   Field           { get; set; }
    [JsonPropertyName("new_value")]           public string?   NewValue        { get; set; }
    [JsonPropertyName("old_value")]           public string?   OldValue        { get; set; }
    [JsonPropertyName("order_dep")]           public string?   Department      { get; set; }
    [JsonPropertyName("order_number")]        public string?   OrderNumber     { get; set; }
    [JsonPropertyName("order_org")]           public string?   Organization    { get; set; }
    [JsonPropertyName("serial")]              public long?     Serial          { get; set; }
}

public class PurchaseOrderAuditConfiguration : IEntityTypeConfiguration<PurchaseOrderAudit>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderAudit> builder)
    {
        builder.ToTable("AuditoriaOC");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ActionType).HasColumnName("AccionRealizada").HasMaxLength(100);
        builder.Property(x => x.ChangedAt).HasColumnName("CambiadoEl");
        builder.Property(x => x.ChangedBy).HasColumnName("CambiadoPor").HasMaxLength(200);
        builder.Property(x => x.Field).HasColumnName("Campo").HasMaxLength(200);
        builder.Property(x => x.NewValue).HasColumnName("ValorNuevo").HasMaxLength(500);
        builder.Property(x => x.OldValue).HasColumnName("ValorAnterior").HasMaxLength(500);
        builder.Property(x => x.Department).HasColumnName("Departamento").HasMaxLength(100);
        builder.Property(x => x.OrderNumber).HasColumnName("OrdenCompra").HasMaxLength(100);
        builder.Property(x => x.Organization).HasColumnName("Organizacion").HasMaxLength(100);
        builder.Property(x => x.Serial).HasColumnName("Serial");

        builder.HasIndex(x => x.OrderNumber);
        builder.HasIndex(x => x.Organization);
        builder.HasIndex(x => x.ChangedAt);
    }
}

public static class PurchaseOrderAuditMapper
{
    /// <summary>
    /// Parsea el sobre de respuesta de /api/provisions/purchase/order/audit.
    /// Soporta la misma estructura que el endpoint de provisiones:
    /// { "data": { "totalPages": N, "totalRecords": N, "dataRecord": { "rows": [...] } } }
    /// También soporta array en raíz o array bajo claves comunes (items, rows, result).
    /// </summary>
    public static (List<PurchaseOrderAudit> Items, int TotalRecords, int TotalPages)
        FromApiEnvelope(string json, CultureInfo? culture = null)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        int totalPages   = 0;
        int totalRecords = 0;
        var items        = new List<PurchaseOrderAudit>();

        // Estructura tipo provisiones: { data: { totalPages, totalRecords, dataRecord: { rows } } }
        if (root.TryGetProperty("data", out var data))
        {
            if (data.TryGetProperty("totalPages",   out var tp) && tp.ValueKind == JsonValueKind.Number)
                totalPages = tp.GetInt32();
            if (data.TryGetProperty("totalRecords", out var tr) && tr.ValueKind == JsonValueKind.Number)
                totalRecords = tr.GetInt32();

            if (data.TryGetProperty("dataRecord", out var dr) &&
                dr.TryGetProperty("rows", out var rowsEl) &&
                rowsEl.ValueKind == JsonValueKind.Array)
            {
                items = DeserializeRows(rowsEl, culture);
            }
            else if (data.ValueKind == JsonValueKind.Array)
            {
                items = DeserializeRows(data, culture);
            }
        }
        // Respuesta plana: array en raíz
        else if (root.ValueKind == JsonValueKind.Array)
        {
            items        = DeserializeRows(root, culture);
            totalRecords = items.Count;
            totalPages   = 1;
        }
        // Array bajo claves comunes en raíz
        else
        {
            foreach (var key in new[] { "items", "rows", "result", "records" })
            {
                if (root.TryGetProperty(key, out var arr) && arr.ValueKind == JsonValueKind.Array)
                {
                    items = DeserializeRows(arr, culture);
                    break;
                }
            }

            if (totalPages   == 0) totalPages   = 1;
            if (totalRecords == 0) totalRecords = items.Count;
        }

        return (items, totalRecords, totalPages);
    }

    private static List<PurchaseOrderAudit> DeserializeRows(JsonElement arrayEl, CultureInfo? culture)
    {
        var raw = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(arrayEl.GetRawText())
                ?? new List<Dictionary<string, JsonElement>>();

        var list = raw.Select(item =>
            item.ToDictionary(
                kv => kv.Key,
                kv => MapperHelper.GetSafeString(kv.Value)
            )
        ).ToList();

        return list.Select(row => MapFromAliasDict(row, culture)).ToList();
    }

    private static PurchaseOrderAudit MapFromAliasDict(Dictionary<string, string> row, CultureInfo? culture)
    {
        string Get(string key) => row.TryGetValue(key, out var v) ? v : string.Empty;

        return new PurchaseOrderAudit
        {
            ActionType   = Get("action_type") ?? "",
            ChangedAt    = MapperHelper.ParseDate(Get("changed_at"), culture),
            ChangedBy    = Get("changed_by") ?? "",
            Field        = Get("column_text_display") ?? "",
            NewValue     = Get("new_value") ?? "",
            OldValue     = Get("old_value") ?? "",
            Department   = Get("order_dep") ?? "",
            OrderNumber  = Get("order_number") ?? "",
            Organization = Get("order_org") ?? "",
            Serial       = MapperHelper.ParseLong(Get("serial"), culture)
        };
    }
}
