//TODO: Implement window title sniffing for Windows (Linux uses the unofficial tidal-hifi app with the web API)

using System.Net.Http.Headers;

namespace THGen.Data.Services;

public class TidalHifiCurrentService
{
    private readonly HttpClient _httpClient;

    public TidalHifiCurrentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(1);
        _httpClient.BaseAddress = new Uri("http://localhost:47836");
        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true
        };
    }

    public async Task<TidalHifiCurrent?> GetCurrentAsync() =>
        await _httpClient.GetFromJsonAsync<TidalHifiCurrent>("current");
}