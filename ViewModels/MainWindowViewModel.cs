using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VEOVideoDownloader.Services;

namespace VEOVideoDownloader.ViewModels;

//TODO: Add cancel button functionality to stop the download when the button is clicked. Disable all other buttons except stop button while downloading.

public partial class MainWindowViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private readonly IDialogService? _dialogService;
    private readonly IFolderPickerService? _folderPickerService;
    private readonly ISettingsService? _settingsService;
    private readonly IWebScraperService? _webScraperService;
    private readonly IDownloadService? _downloadService;

    private IStorageFolder? _selectedFolder;

    [ObservableProperty] private string? _downloadPath;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private string? _statusMessage;
    [ObservableProperty] private string? _videoUrl;
    [ObservableProperty] private double _downloadProgress;

    //Runtime constructor.
    public MainWindowViewModel(HttpClient httpClient, IFolderPickerService folderPickerService,
        ISettingsService settingsService,
        IWebScraperService webScraperService, IDialogService dialogService, IDownloadService downloadService)
    {
        _httpClient = httpClient;
        _folderPickerService = folderPickerService;
        _settingsService = settingsService;
        _webScraperService = webScraperService;
        _dialogService = dialogService;
        _downloadService = downloadService;

        _ = InitializeAsync();
    }

    //Design Constructor
    public MainWindowViewModel()
    {
        _httpClient = new HttpClient();
        _folderPickerService = null;
        _settingsService = null;
        _webScraperService = null;
        _dialogService = null;
        _downloadService = null;
        DownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        VideoUrl = "https://app.veo.co/matches/example";
    }

    private async Task InitializeAsync()
    {
        if (_folderPickerService == null) return;
        try
        {
            string? savedPath = null;
            if (_settingsService != null)
            {
                savedPath = await _settingsService.GetDownloadFolderPathAsync();
            }

            if (!string.IsNullOrEmpty(savedPath))
            {
                DownloadPath = savedPath;
                Console.WriteLine($"Loaded saved download path: {savedPath}");
            }

            var downloadsFolder = await _folderPickerService.GetDefaultDownloadsFolderAsync();
            if (downloadsFolder != null)
            {
                _selectedFolder = downloadsFolder;

                if (string.IsNullOrEmpty(savedPath))
                {
                    DownloadPath = downloadsFolder.Path.LocalPath;
                    Console.WriteLine($"Default download path set to: {DownloadPath}");
                }

                StatusMessage = "Ready to download";
            }
            else
            {
                var fallbackPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads");
                if (string.IsNullOrEmpty(savedPath))
                {
                    DownloadPath = fallbackPath;
                    Console.WriteLine($"Fallback download path set to {fallbackPath}");
                }

                StatusMessage = "Ready (folder selection required for download)";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing download path: {ex.Message}");

            var fallbackPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads");
        }
    }


    [RelayCommand]
    private async Task BrowseFolderAsync()
    {
        if (_folderPickerService == null || _settingsService == null) return;

        var selectedFolder = await _folderPickerService.PickFolderWithAccessAsync("Select Download Folder");

        if (selectedFolder != null)
        {
            _selectedFolder = selectedFolder;
            DownloadPath = selectedFolder.Path.LocalPath;
            Console.WriteLine("Download Path: " + DownloadPath);
            await _settingsService.SetDownloadFolderPathAsync(DownloadPath);
            Console.WriteLine("Saved Download Path to settings.json");

            StatusMessage = "Download folder updated";
        }
    }


    [RelayCommand]
    private async Task DownloadVideoAsync()
    {
        var downloadUrl = "";

        if (_webScraperService == null || _dialogService == null) return;

        //validate url provided.
        if (string.IsNullOrWhiteSpace(VideoUrl))
        {
            await _dialogService.ShowErrorAsync("Please enter a URL");
            return;
        }

        //validate format of url
        if (!_webScraperService.IsValidUrl(VideoUrl))
        {
            await _dialogService.ShowErrorAsync("Invalid URL. Must be a valid VEO match URL");
            return;
        }

        try
        {
            IsProcessing = true;
            StatusMessage = "Parsing link to get download link...";
            downloadUrl = await _webScraperService.GetDownloadLinkAsync(VideoUrl);

            if (string.IsNullOrEmpty(downloadUrl))
            {
                await _dialogService.ShowErrorAsync("Failed to extract valid download link from the provided URL");
                StatusMessage = "Failed to get download link";
                return;
            }

            StatusMessage = "Found Download Link!";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting video link from website: {ex.Message}");
        }

        if (_downloadService == null || _dialogService == null || _folderPickerService == null) return;

        try
        {
            if (_selectedFolder == null)
            {
                StatusMessage = "Getting Downloads folder access...";
                _selectedFolder = await _folderPickerService.GetDefaultDownloadsFolderAsync();
            }

            if (_selectedFolder == null)
            {
                StatusMessage = "Getting downloads folder access";
                _selectedFolder = await _folderPickerService.PickFolderWithAccessAsync("Select Download Folder");

                if (_selectedFolder == null)
                {
                    StatusMessage = "Download cancelled - no folder selected";
                    return;
                }

                DownloadPath = _selectedFolder.Path.LocalPath;
            }

            if (string.IsNullOrEmpty(downloadUrl)) return;
            try
            {
                var filename = $"VEO_VIDEO_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";

                StatusMessage = "Starting Download....";
                DownloadProgress = 0;

                var progress = new Progress<double>(percent =>
                {
                    DownloadProgress = percent * 100;
                    StatusMessage = $"Downloading... {DownloadProgress:F1}%";
                });

                await _downloadService.DownloadToFolderAsync(downloadUrl, _selectedFolder, filename, progress);

                var fullPath = Path.Combine(DownloadPath ?? "", filename);
                await _dialogService.ShowInfoAsync($"Download Completed!\n\nSaved to:\n {fullPath}");
            }
            catch (UnauthorizedAccessException)
            {
                StatusMessage = "Permission Denied. Please select download folder.";

                if (_dialogService != null)
                {
                    await _dialogService.ShowErrorAsync(
                        "Permission denied to downloads folder.\n\nPlease select a location manually");
                }

                _selectedFolder = await _folderPickerService.PickFolderWithAccessAsync("Select Download Folder");

                if (_selectedFolder != null)
                {
                    await DownloadVideoAsync();
                }
                else
                {
                    StatusMessage = "Download Cancelled.";
                }
            }

            IsProcessing = false;

        }
        catch (Exception ex)
        {
            StatusMessage = $"Download failed: {ex.Message}";
            if (_dialogService != null)
            {
                await _dialogService.ShowErrorAsync($"Download failed:\n{ex.Message}");
            }
        }

        IsProcessing = false;
    }
}