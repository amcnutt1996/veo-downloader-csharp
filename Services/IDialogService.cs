using System.Threading.Tasks;

namespace VEOVideoDownloader.Services;

public interface IDialogService
{
    Task ShowErrorAsync(string message);

    Task ShowInfoAsync(string message);

    Task ShowWarningAsync(string message);

    Task<bool> ShowConfirmationAsync(string message);
}