using System;
using System.Net.Http;
using Avalonia.Controls;
using VEOVideoDownloader.Services;
using VEOVideoDownloader.ViewModels;

namespace VEOVideoDownloader.Views;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient;
    
    public MainWindow()
    {
        InitializeComponent();

        _httpClient = new HttpClient();

        // Create the service using this window's StorageProvider
        var folderPickerService = new FolderPickerService(StorageProvider);
        var settingsService = new SettingsService();
        var webScraperService = new WebScraperService(_httpClient);
        var downloadService = new DownloadService(_httpClient);
        var dialogService = new DialogService(this);

        // Connect the services to the view
        DataContext = new MainWindowViewModel(_httpClient, folderPickerService, settingsService, webScraperService, dialogService,
            downloadService);

        Closed += OnWindowClosed;
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        _httpClient.Dispose();
    }
}