using HGT.EAM.Client.Configuration;
using HGT.EAM.Client.Domain.Models;
using HGT.EAM.Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HGT.EAM.Client.Application;

public sealed class PurchaseOrderAuditSyncService : BaseSyncService<PurchaseOrderAudit, PurchaseOrderAuditQuery>
{
    public PurchaseOrderAuditSyncService(
        Infrastructure.Http.RestClient client,
        IAppDbContextFactory dbFactory,
        ILogger<PurchaseOrderAuditSyncService> logger)
        : base(client, dbFactory, logger)
    {
    }

    protected override DbSet<PurchaseOrderAudit> GetDbSet(AppDbContext db) => db.PurchaseOrderAudits;

    public override async Task<(List<PurchaseOrderAudit> Items, int TotalRecords, int TotalPages)> FetchAsync(
        PurchaseOrderAuditQuery q, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Iniciando fetch de auditoría OC: TypeFilter={TypeFilter}, Month={Month}, Year={Year}, Page={Page}, PageSize={PageSize}",
            q.TypeFilter, q.Month, q.Year, q.Page, q.PageSize);

        var query = q.ToQueryDictionary();
        var culture = !string.IsNullOrWhiteSpace(q.Credentials?.Culture) 
                    ? new System.Globalization.CultureInfo(q.Credentials.Culture) 
                    : null;

        var json = await _client.GetStringAsync(ApiEndpoints.PurchaseOrderAudit, query, q.Credentials, ct).ConfigureAwait(false);
        var result = PurchaseOrderAuditMapper.FromApiEnvelope(json, culture);

        _logger.LogInformation(
            "Fetch completado: TotalRecords={TotalRecords}, TotalPages={TotalPages}, Items={Items}",
            result.TotalRecords, result.TotalPages, result.Items.Count);

        return result;
    }
}
