using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using VideoDownloader.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace VideoDownloader.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void DownloadClick(object sender, RoutedEventArgs args)
    {
        // const string testLink = "https://app.veo.co/matches/20260212-training-february-12-v71d3ba7/";
        var scraper = new WebScraper();
        var url = UrlTextBox.Text;

        var downloadLink = await scraper.GetDownloadLink(url);
    }
    
    
    public void StopDownloadClk(object sender, RoutedEventArgs args)
    {
        //do stuff here when button clicked
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