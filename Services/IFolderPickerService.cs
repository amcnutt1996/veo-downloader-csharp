using System.Threading.Tasks;

namespace VEOVideoDownloader.Services;

/// <summary>
/// Provides a service interface for handling folder selection dialogs.
/// </summary>
public interface IFolderPickerService
{
    Task<string?> PickFolderAsync(string title = "Select Folder");
}