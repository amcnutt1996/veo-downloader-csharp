using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VEOVideoDownloader.Services;
using VEOVideoDownloader.Views;

namespace VEOVideoDownloader.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IFolderPickerService? _folderPickerService;
    private readonly ISettingsService? _settingsService;
    
    [ObservableProperty] private string? _downloadPath;

    public MainWindowViewModel(IFolderPickerService folderPickerService, ISettingsService settingsService)
    {
        _folderPickerService = folderPickerService;
        _settingsService = settingsService;

        _ = InitializeAsync();
    }

    public MainWindowViewModel()
    {
        _folderPickerService = null;
        _settingsService = null;
        DownloadPath = @"C:\Users\Example\Downloads";
    }

    private async Task InitializeAsync()
    {
        if (_settingsService == null)
        {
            return;
        }

        var savedPath = await _settingsService.GetDownloadFolderPathAsync();
        if (!string.IsNullOrEmpty(savedPath))
        {
            DownloadPath = savedPath;
        }
    }
    
    
    [RelayCommand]
    private async Task BrowseFolderAsync()
    {
        if (_folderPickerService == null || _settingsService == null)
        {
            return;
        }
        var selectedPath = await _folderPickerService.PickFolderAsync("Select Download Folder");
        if (selectedPath != null)
        {
            DownloadPath = selectedPath;
            Console.WriteLine("Download Path: " + DownloadPath);
            await _settingsService.SetDownloadFolderPathAsync(selectedPath);
            Console.WriteLine("Saved Download Path to settings.json");
        }
    }
}