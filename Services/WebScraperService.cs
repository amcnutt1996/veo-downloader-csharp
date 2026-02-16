using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VEOVideoDownloader.Services;

public class WebScraperService : IWebScraperService
{
    private readonly HttpClient _httpClient;

    //Creates a new WebScraper service
    public WebScraperService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<string?> GetDownloadLinkAsync(string url)
    {
        if (!IsValidUrl(url)) return null;

        try
        {
            var primaryUrl = GetPrimaryUrl(url);
            Console.WriteLine($"PRIMARY URL -> {primaryUrl}");

            var apiResponse = await GetUrlResponseAsync(primaryUrl);
            if (string.IsNullOrEmpty(apiResponse))
            {
                Console.WriteLine("Failed to get API Response");
                return null;
            }

            Console.WriteLine($"PRIMARY URL RESPONSE: \n {apiResponse}");


            var downloadUrl = ExtractDownloadUrl(apiResponse);
            if (string.IsNullOrEmpty(downloadUrl) || downloadUrl.Contains("not found") ||
                downloadUrl.Contains("failed"))
            {
                Console.WriteLine("Failed to get Download URL");
                return null;
            }

            Console.WriteLine($"Download URL: {downloadUrl}");
            return downloadUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Getting Download Link: {ex.Message}");
            return null;
        }
    }

    public bool IsValidUrl(string url)
    {
        return url.Contains("https://") && url.Contains("matches");
    }


    private static string GetPrimaryUrl(string url)
    {
        var indexPosition = url.IndexOf("/matches/", StringComparison.Ordinal);
        var endPart = url.Substring(indexPosition);
        return "https://app.veo.co/api/app" + endPart + "videos/";
    }

    private static string ExtractDownloadUrl(string apiResponse)
    {
        Console.WriteLine("---Getting Download URL--- \n");

        var anchorIndex = apiResponse.IndexOf("panorama/transcode", StringComparison.Ordinal);
        if (anchorIndex == -1) return "Download link not found in response.";

        var startIndex = apiResponse.LastIndexOf("https://", anchorIndex, StringComparison.Ordinal);

        var lastIndex = apiResponse.IndexOf(".mp4", anchorIndex, StringComparison.Ordinal);

        if (startIndex == -1 || lastIndex == -1) return "Download link parsing failed.";

        var downloadUrl = apiResponse.Substring(startIndex, lastIndex - startIndex);
        downloadUrl += ".mp4";

        return downloadUrl;
    }


    private async Task<string?> GetUrlResponseAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            Console.WriteLine(response.IsSuccessStatusCode);
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request Error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Fetching URL: {ex.Message}");
            return null;
        }
    }
}