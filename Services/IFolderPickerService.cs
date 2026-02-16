using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace VEOVideoDownloader.Services;

/// <summary>
///     Provides a service interface for handling folder selection dialogs.
/// </summary>
public interface IFolderPickerService
{
    Task<IStorageFolder?> PickFolderWithAccessAsync(string title = "Select Folder");
    Task<IStorageFolder?> GetDefaultDownloadsFolderAsync();
}