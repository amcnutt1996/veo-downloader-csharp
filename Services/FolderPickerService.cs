using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.Collections.Generic;

namespace VEOVideoDownloader.Services;

public class FolderPickerService : IFolderPickerService
{
    private readonly IStorageProvider _storageProvider;

    public FolderPickerService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
    
    public async Task<IStorageFolder?> PickFolderWithAccessAsync(string title = "Select Folder")
    {
        if (_storageProvider == null) return null;

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

        if (result != null && result.Count > 0) return result[0];
        return null;
    }

    public async Task<IStorageFolder?> GetDefaultDownloadsFolderAsync()
    {
        if (_storageProvider == null) return null;

        var downloadsFolder = await _storageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);

        return downloadsFolder;
    }
}