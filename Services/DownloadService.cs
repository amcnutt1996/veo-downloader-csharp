using System;
using System.IO;
using System.IO.Enumeration;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace VEOVideoDownloader.Services;

public class DownloadService : IDownloadService
{
    private readonly HttpClient _httpClient;

    public DownloadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task DownloadToFolderAsync(string url, IStorageFolder folder, string filename, IProgress<double>? progress = null)
    {
        try
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;

            var storageFile = await folder.CreateFileAsync(filename);

            if (storageFile == null) throw new IOException($"Failed to create file {filename}");

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = await storageFile.OpenWriteAsync();


            if (totalBytes.HasValue && progress != null)
            {
                await CopyToAsyncWithProgress(contentStream, fileStream, totalBytes.Value, progress);
            }
            else
            {
                await contentStream.CopyToAsync(fileStream);
            }

            Console.WriteLine($"Download Complete: {filename}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Download failed: {filename}, Error: {ex.Message}");
        }
    }

    private static async Task CopyToAsyncWithProgress(Stream source, Stream destination, long totalBytes,
        IProgress<double> progress)
    {
        var buffer = new byte[81920];
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await source.ReadAsync(buffer)) > 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;

            var progressPercentage = (double)totalRead / totalBytes;
            progress.Report(progressPercentage);

        }
        
        
    }
}