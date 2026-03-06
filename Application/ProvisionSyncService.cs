using HGT.EAM.Client.Configuration;
using HGT.EAM.Client.Domain.Models;
using HGT.EAM.Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HGT.EAM.Client.Application;

public sealed class ProvisionSyncService : BaseSyncService<Provision, ProvisionQuery>
{
    public ProvisionSyncService(
        Infrastructure.Http.RestClient client,
        IAppDbContextFactory dbFactory,
        ILogger<ProvisionSyncService> logger)
        : base(client, dbFactory, logger)
    {
    }

    protected override DbSet<Provision> GetDbSet(AppDbContext db) => db.Provisions;

    public override async Task<(List<Provision> Items, int TotalRecords, int TotalPages)> FetchAsync(
        ProvisionQuery q, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Iniciando fetch de provisiones con filtros: TypeFilter={TypeFilter}, Month={Month}, Year={Year}, Page={Page}, PageSize={PageSize}",
            q.TypeFilter, q.Month, q.Year, q.Page, q.PageSize);

        var query = q.ToQueryDictionary();
        var culture = !string.IsNullOrWhiteSpace(q.Credentials?.Culture) 
                    ? new System.Globalization.CultureInfo(q.Credentials.Culture) 
                    : null;

        var json = await _client.GetStringAsync(ApiEndpoints.Provisions, query, q.Credentials, ct).ConfigureAwait(false);
        var result = ProvisionMapper.FromApiEnvelope(json, culture);

        _logger.LogInformation("Json: \n {json}", json);

        _logger.LogInformation("Fetch completado: TotalRecords={TotalRecords}, TotalPages={TotalPages}, Items={Items}",
            result.TotalRecords, result.TotalPages, result.Items.Count);

        return result;
    }
}

