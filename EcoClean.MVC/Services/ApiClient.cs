using System.Net.Http.Headers;
using System.Text.Json;

namespace EcoClean.MVC.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;
    private readonly ILogger<ApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ApiClient(HttpClient http, IHttpContextAccessor ctx, ILogger<ApiClient> logger)
    {
        _http   = http;
        _ctx    = ctx;
        _logger = logger;
    }

    private string? GetToken() => _ctx.HttpContext?.Request.Cookies["ecoclean_token"];

    private HttpRequestMessage BuildRequest(HttpMethod method, string path, HttpContent? content = null)
    {
        var req = new HttpRequestMessage(method, path);
        var token = GetToken();
        if (!string.IsNullOrEmpty(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (content != null)
            req.Content = content;
        return req;
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        try
        {
            var req = BuildRequest(HttpMethod.Get, path);
            var res = await _http.SendAsync(req);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("GET {Path} => {Status}: {Body}", path, (int)res.StatusCode, body);
                return default;
            }

            return JsonSerializer.Deserialize<T>(body, _jsonOpts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET {Path} failed", path);
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string path, object body)
    {
        try
        {
            var json    = JsonSerializer.Serialize(body, _jsonOpts);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var req     = BuildRequest(HttpMethod.Post, path, content);
            var res     = await _http.SendAsync(req);
            var resBody = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("POST {Path} => {Status}: {Body}", path, (int)res.StatusCode, resBody);
                return default;
            }

            return JsonSerializer.Deserialize<T>(resBody, _jsonOpts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST {Path} failed", path);
            return default;
        }
    }

    public async Task<T?> PostRawAsync<T>(string path, string jsonBody)
    {
        try
        {
            var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            var req     = BuildRequest(HttpMethod.Post, path, content);
            var res     = await _http.SendAsync(req);
            var resBody = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("POST(raw) {Path} => {Status}: {Body}", path, (int)res.StatusCode, resBody);
                return default;
            }

            return JsonSerializer.Deserialize<T>(resBody, _jsonOpts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST(raw) {Path} failed", path);
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string path)
    {
        try
        {
            var req = BuildRequest(HttpMethod.Delete, path);
            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode)
                _logger.LogWarning("DELETE {Path} => {Status}", path, (int)res.StatusCode);
            return res.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE {Path} failed", path);
            return false;
        }
    }

    public async Task<T?> PutAsync<T>(string path, object body)
    {
        try
        {
            var json    = JsonSerializer.Serialize(body, _jsonOpts);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var req     = BuildRequest(HttpMethod.Put, path, content);
            var res     = await _http.SendAsync(req);
            var resBody = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("PUT {Path} => {Status}: {Body}", path, (int)res.StatusCode, resBody);
                return default;
            }

            return JsonSerializer.Deserialize<T>(resBody, _jsonOpts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT {Path} failed", path);
            return default;
        }
    }
}
