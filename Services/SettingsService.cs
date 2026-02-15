using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using VEOVideoDownloader.Models;

namespace VEOVideoDownloader.Services;

public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;
    private AppSettings? _cachedSettings;

    public SettingsService()
    {
        //get app data folder path
        //this is cross-platform
        var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        //create the folder for your app.
        var appFolder = Path.Combine(appDataFolder, "VEOVideoDownloader");

        Directory.CreateDirectory(appFolder);
        _settingsFilePath = Path.Combine(appFolder, "settings.json");
    }
    
    public async Task<AppSettings> LoadSettingsAsync()
    {
        // want to return the cached settings it it's already loaded.
        if (_cachedSettings != null) return _cachedSettings;

        // if not: try to check if the settings file exists in the directory.
        //read the JSON file and deserialize then return the settings from the settings file.
        // create new settings file if deserialization fails.
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = await File.ReadAllTextAsync(_settingsFilePath);
                _cachedSettings = JsonSerializer.Deserialize<AppSettings>(json);
            }

            if (_cachedSettings != null)
            {
                return _cachedSettings;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Loading Settings: {ex.Message}");
        }

        _cachedSettings = new AppSettings();
        return _cachedSettings;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(settings, options);

            await File.WriteAllTextAsync(_settingsFilePath, json);
            _cachedSettings = settings;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
            throw;
        }
    }

    public async Task<string?> GetDownloadFolderPathAsync()
    {
        var settings = await LoadSettingsAsync();
        return settings.DownloadFolderPath;
    }

    public async Task SetDownloadFolderPathAsync(string path)
    {
        var settings = await LoadSettingsAsync();
        settings.DownloadFolderPath = path;
        await SaveSettingsAsync(settings);
    }
}