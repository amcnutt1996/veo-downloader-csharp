using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using VEOVideoDownloader.Services;
using VEOVideoDownloader.ViewModels;

namespace VEOVideoDownloader.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Create the service using this window's StorageProvider
        var folderPickerService = new FolderPickerService(StorageProvider);
        var settingsService = new SettingsService();
            
        // Create the ViewModel with the service
        DataContext = new MainWindowViewModel(folderPickerService, settingsService);
    }

    public void DownloadClick(object sender, RoutedEventArgs args)
    {
        // const string testLink = "https://app.veo.co/matches/20260212-training-february-12-v71d3ba7/";
        var scraper = new WebScraper();

        if (UrlTextBox.Text != null)
        {
            var downloadLink = scraper.GetDownloadLink(UrlTextBox.Text);    
        }
        else
        {
            ShowMessage("Please enter a VEO link.");
        }
    }
    
    public static async void ShowMessage(string message)
    {
        try
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Error", message, ButtonEnum.Ok);
            var result = await box.ShowAsync(); // Use await
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}