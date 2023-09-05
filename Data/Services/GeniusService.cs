using System.Net.Http.Headers;
using AngleSharp;
using Flurl;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace THGen.Data.Services;

public class GeniusService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeniusService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _httpClient.BaseAddress = new Uri("https://api.genius.com");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["GeniusAccessToken"]);
        _httpClient.DefaultRequestHeaders.Add("Cookie", _configuration["GeniusCookie"]);
    }

    public async Task<GeniusSearch> GetSearchResultsAsync(string query)
    {
        var q = "search"
            .SetQueryParam("q", query);
        var res = await _httpClient.GetStringAsync(q.ToUri());
        return GeniusSearch.FromJson(res);
    }

    public async Task<string> GetScrapedLyricsAsync(string url)
    {
        var page = await _httpClient.GetStringAsync(url);
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var doc = await context.OpenAsync(req => req.Content(page));
        var lyricsContainerElement = doc.QuerySelector("[data-lyrics-container='true']");
        if (lyricsContainerElement != null)
        {
            // Loop through all anchor (link) elements within the container
            foreach (var anchorElement in lyricsContainerElement.QuerySelectorAll("a"))
            {
                // Create a new span element
                var spanElement = doc.CreateElement("span");
                spanElement.InnerHtml = anchorElement.InnerHtml; // Set the text content

                // Replace the anchor element with the span element
                anchorElement.Replace(spanElement);
            }

            // Extract the modified HTML content
            var modifiedHtml = lyricsContainerElement.OuterHtml;
            return modifiedHtml;
        }
        else
        {
            return "Error";
        }

    }
}