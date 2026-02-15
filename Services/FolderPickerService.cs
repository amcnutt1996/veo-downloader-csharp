using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace VEOVideoDownloader.Services;

public class FolderPickerService : IFolderPickerService
{
    private readonly IStorageProvider _storageProvider;

    public FolderPickerService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<string?> PickFolderAsync(string title = "Select Folder")
    {
        if (_storageProvider == null)
        {
            return null;
        }

        var options = new FolderPickerOpenOptions()
        {
            Title = title,
            AllowMultiple = false
        };
        var suggestedFolder = await _storageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);

        if (suggestedFolder != null)
        {
            options.SuggestedStartLocation = suggestedFolder;
        }

        var result = await _storageProvider.OpenFolderPickerAsync(options);
        if (result != null && result.Count > 0)
        {
            return result[0].Path.LocalPath;
        }
        return null;
    }
}