using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace VEOVideoDownloader.Services;

public interface IDownloadService
{
    Task DownloadToFolderAsync(string url, IStorageFolder folder, string filename, IProgress<double>? progress = null);

    // Task CancelDownloadAsync();
}