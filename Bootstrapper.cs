using HGT.EAM.Client.Configuration;
using HGT.EAM.Client.Infrastructure.Data;
using HGT.EAM.Client.Infrastructure.Http;
using HGT.EAM.Client.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace HGT.EAM.Client;

public static class Bootstrapper
{
    public static ServiceProvider Build(IConfiguration config)
    {
        var api = config.GetSection("ApiSettings").Get<ApiSettings>()
                  ?? throw new InvalidOperationException("Missing ApiSettings section in appsettings.json");

        var retry = config.GetSection("HttpRetry").Get<HttpRetryOptions>() ?? new HttpRetryOptions();

        var services = new ServiceCollection();

        // Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .CreateLogger();

        services.AddLogging(builder => builder.ClearProviders().AddSerilog());

        services.AddSingleton(api);
        services.AddSingleton(retry);

        services.AddHttpClient<RestClient>(http =>
            {
                http.BaseAddress = new Uri(api.BaseUrl!.TrimEnd('/') + "/");
                http.Timeout = TimeSpan.FromSeconds(api.TimeoutSeconds > 0 ? api.TimeoutSeconds : 30);

                http.DefaultRequestHeaders.Accept.Clear();
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(_ =>
            {
                var retries = Math.Max(0, retry.MaxRetries);
                if (retries == 0)
                    return Policy.NoOpAsync<HttpResponseMessage>();

                var baseDelay = TimeSpan.FromMilliseconds(Math.Max(0, retry.BaseDelayMilliseconds));

                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(r => r.StatusCode == (HttpStatusCode)429)
                    .WaitAndRetryAsync(
                        retryCount: retries,
                        sleepDurationProvider: attempt =>
                        {
                            var exp = Math.Min(10, attempt - 1);
                            var delay = TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, exp));
                            return delay > TimeSpan.FromSeconds(30) ? TimeSpan.FromSeconds(30) : delay;
                        });
            });

        var connStr = config.GetConnectionString("DefaultConnection");
        services.AddSingleton<IAppDbContextFactory>(new AppDbContextFactory(connStr));

        services.AddSingleton<ProvisionSyncService>();
        services.AddSingleton<PurchaseOrderAuditSyncService>();

        return services.BuildServiceProvider();
    }
}
