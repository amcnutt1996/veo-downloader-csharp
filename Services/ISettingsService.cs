using System.Threading.Tasks;
using VEOVideoDownloader.Models;

namespace VEOVideoDownloader.Services;

public interface ISettingsService
{
    Task<AppSettings> LoadSettingsAsync();

    Task SaveSettingsAsync(AppSettings settings);

    Task<string?> GetDownloadFolderPathAsync();

    Task SetDownloadFolderPathAsync(string path);
}