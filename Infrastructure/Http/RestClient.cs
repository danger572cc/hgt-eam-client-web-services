using System.Text;
using Microsoft.Extensions.Logging;

namespace HGT.EAM.Client.Infrastructure.Http;

public sealed class RestClient
{
    private readonly HttpClient _http;
    private readonly ILogger<RestClient> _logger;

    public RestClient(HttpClient http, ILogger<RestClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public Task<string> GetStringAsync(string relativeOrAbsoluteUrl, Configuration.ApiCredentials? credentials = null, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Get, relativeOrAbsoluteUrl, credentials, ct);

    public Task<string> GetStringAsync(string path, IReadOnlyDictionary<string, string?> query, Configuration.ApiCredentials? credentials = null, CancellationToken ct = default)
    {
        var url = BuildUrl(path, query);
        return GetStringAsync(url, credentials, ct);
    }

    private async Task<string> SendAsync(HttpMethod method, string relativeOrAbsoluteUrl, Configuration.ApiCredentials? credentials, CancellationToken ct)
    {
        var baseAddress = _http.BaseAddress?.ToString().TrimEnd('/') ?? string.Empty;
        var fullUrl = relativeOrAbsoluteUrl.StartsWith("http") 
            ? relativeOrAbsoluteUrl 
            : $"{baseAddress}/{relativeOrAbsoluteUrl.TrimStart('/')}";

        _logger.LogInformation("Enviando solicitud HTTP {Method} a {Url}", method, fullUrl);
        try
        {
            using var req = new HttpRequestMessage(method, relativeOrAbsoluteUrl);

            // Autenticación obligatoria
            if (credentials == null || string.IsNullOrWhiteSpace(credentials.Username) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                throw new InvalidOperationException("La autenticación en la API es obligatoria. No se encontraron credenciales válidas.");
            }

            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.Username}:{credentials.Password}"));
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);

            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

            var body = await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Error HTTP {StatusCode} al solicitar {Url}. Body: {Body}", resp.StatusCode, fullUrl, body);
                throw new HttpRequestException($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}");
            }

            _logger.LogInformation("Respuesta HTTP exitosa desde {Url} ({ContentLength} bytes)", fullUrl, body.Length);
            return body;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al realizar solicitud HTTP {Method} a {Url}", method, fullUrl);
            throw;
        }
    }

    private static string BuildUrl(string path, IReadOnlyDictionary<string, string?> query)
    {
        var p = path.TrimStart('/');
        if (query.Count == 0) return p;
        var sb = new StringBuilder();
        sb.Append(p).Append('?');
        var first = true;
        foreach (var kv in query)
        {
            if (!first) sb.Append('&');
            first = false;
            sb.Append(Uri.EscapeDataString(kv.Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(kv.Value ?? string.Empty));
        }
        return sb.ToString();
    }
}
