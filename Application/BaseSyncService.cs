using HGT.EAM.Client.Configuration;
using HGT.EAM.Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HGT.EAM.Client.Application;

public abstract class BaseSyncService<TEntity, TQuery>
    where TEntity : class
    where TQuery : BaseQuery, new()
{
    protected readonly Infrastructure.Http.RestClient _client;
    protected readonly IAppDbContextFactory _dbFactory;
    protected readonly ILogger _logger;

    protected BaseSyncService(
        Infrastructure.Http.RestClient client,
        IAppDbContextFactory dbFactory,
        ILogger logger)
    {
        _client    = client;
        _dbFactory = dbFactory;
        _logger    = logger;
    }

    /// <summary>
    /// Abstract method that each concrete service must implement to fetch
    /// a specific page and parse its response.
    /// </summary>
    public abstract Task<(List<TEntity> Items, int TotalRecords, int TotalPages)>
        FetchAsync(TQuery q, CancellationToken ct = default);

    /// <summary>
    /// Abstract method that returns the corresponding DbSet for the entity.
    /// This allows generic persistence without depending on the property name.
    /// </summary>
    protected abstract DbSet<TEntity> GetDbSet(AppDbContext db);

    public async Task<int> PersistAsync(List<TEntity> items, CancellationToken ct = default)
    {
        await using var db = _dbFactory.CreateDbContext();
        if (db is null)
        {
            _logger.LogWarning("No hay conexión a base de datos configurada; se omite persistencia.");
            return 0;
        }

        var entityName = typeof(TEntity).Name;
        _logger.LogInformation("Iniciando persistencia de {Count} registros de {Entity}", items.Count, entityName);
        await db.Database.EnsureCreatedAsync(ct).ConfigureAwait(false);

        var dbSet = GetDbSet(db);

        const int batchSize = 500;
        var inserted = 0;
        for (var i = 0; i < items.Count; i += batchSize)
        {
            var batch = items.Skip(i).Take(batchSize).ToList();
            await dbSet.AddRangeAsync(batch, ct).ConfigureAwait(false);
            inserted += await db.SaveChangesAsync(ct).ConfigureAwait(false);
            db.ChangeTracker.Clear();
        }

        _logger.LogInformation("Persistencia de {Entity} finalizada. Filas insertadas: {Inserted}", entityName, inserted);
        return inserted;
    }

    public async Task<(int TotalRecords, int TotalPages, int Inserted)>
        FetchAndPersistAllAsync(TQuery q, CancellationToken ct = default)
    {
        var entityName    = typeof(TEntity).Name;
        int totalInserted = 0;
        int currentPage   = q.Page;
        int totalPages    = 0;
        int totalRecords  = 0;

        _logger.LogInformation("Iniciando sincronización masiva de {Entity} desde página {Page}", entityName, currentPage);

        do
        {
            var currentQuery = new TQuery
            {
                Credentials = q.Credentials,
                TypeFilter = q.TypeFilter,
                Month      = q.Month,
                Year       = q.Year,
                PageSize   = q.PageSize,
                Page       = currentPage
            };

            var result = await FetchAsync(currentQuery, ct).ConfigureAwait(false);

            if (totalPages == 0)
            {
                totalPages   = result.TotalPages;
                totalRecords = result.TotalRecords;
            }

            if (result.Items.Any())
            {
                totalInserted += await PersistAsync(result.Items, ct).ConfigureAwait(false);
                result.Items.Clear(); // Helps GC
            }

            _logger.LogInformation("Progreso {Entity}: {Current}/{Total} páginas procesadas", entityName, currentPage, totalPages);
            currentPage++;

        } while (currentPage <= totalPages && !ct.IsCancellationRequested);

        return (totalRecords, totalPages, totalInserted);
    }
}
