using System.Threading.Tasks;

namespace VEOVideoDownloader.Services;

public interface IWebScraperService
{
    Task<string?> GetDownloadLinkAsync(string url);

    bool IsValidUrl(string url);
}