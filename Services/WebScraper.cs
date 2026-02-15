using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VEOVideoDownloader.Views;

namespace VEOVideoDownloader.Services;

public class WebScraper
{
    // Static HttpClientHandler instance to manage HTTP connections.
    
    // Static HttpClient instance to send HTTP requests.
    private static readonly HttpClient Client = new HttpClient();

    /// <summary>
    /// Retrieves a download link from the provided URL by performing a series of HTTP requests
    /// and processing the responses.
    /// </summary>
    /// <param name="url">The URL to process and retrieve the download link from.</param>
    public async Task GetDownloadLink(string url)
    {
        if (LinkChecker(url))
        {
            var firstUrl = GetPrimaryUrl(url);
            Console.WriteLine("First URL: " + firstUrl);

            // Fetch the content of the first URL (API/HTML)
            var firstResponse = await GetUrlReply(firstUrl);

            Console.WriteLine("RESPONSE: \n" + firstResponse + "\n");
            
            // Extract the video link
            var secondUrl = GetSecondUrl(firstResponse);
            Console.WriteLine("SECOND URL (Download Link): " + secondUrl);
            
        }
        else
        {
            MainWindow.ShowMessage("Error, invalid URL.");
        }
    }


    private static bool LinkChecker(string url)
    {
        return url.Contains("https://") && url.Contains("/matches/");
    }

    private static string GetPrimaryUrl(string url)
    {
        var indexPosition = url.IndexOf("/matches/", StringComparison.Ordinal);
        var endPart = url.Substring(indexPosition);
        return "https://app.veo.co/api/app" + endPart + "videos/";
    }
    
    private static string GetSecondUrl(string firstResponse)
    {
        Console.WriteLine("---Getting Second URL String--- \n");

        var anchorIndex = firstResponse.IndexOf("panorama/transcode", StringComparison.Ordinal);
        if (anchorIndex == -1) return "Download link not found in response.";

        var startIndex = firstResponse.LastIndexOf("https://", anchorIndex, StringComparison.Ordinal);
        var lastIndex = firstResponse.IndexOf(".mp4", anchorIndex, StringComparison.Ordinal);
        
        if (startIndex == -1 || lastIndex == -1) return "Download link parsing failed.";

        var secondUrl = firstResponse.Substring(startIndex, (lastIndex-startIndex));
        secondUrl += ".mp4";
        return secondUrl;
    }
    
    public static async Task<string> GetUrlReply(string url)
    {
        try
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}