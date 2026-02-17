using System.IO;
using System.Text.Json;

namespace SkyWatch.Services;

public interface ISettingsService
{
    Task LoadSettingsAsync();
    Task SaveSettingsAsync();
}

public class SettingsService : ISettingsService
{
    private readonly string _filePath;

    public SettingsService()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SkyWatch");
        Directory.CreateDirectory(appDataPath);
        _filePath = Path.Combine(appDataPath, "settings.json");
    }

    public async Task LoadSettingsAsync()
    {
        if (!File.Exists(_filePath)) return;

        try
        {
            using var stream = File.OpenRead(_filePath);
            var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream);

            if (settings != null)
            {
                ApiConfig.Units = settings.Units;
                ApiConfig.Lang = settings.Lang;
            }
        }
        catch
        {
            // 설정 로드 실패 시 기본값 유지
        }
    }

    public async Task SaveSettingsAsync()
    {
        var settings = new AppSettings
        {
            Units = ApiConfig.Units,
            Lang = ApiConfig.Lang
        };

        try
        {
            using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, settings, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    private class AppSettings
    {
        public string Units { get; set; } = "metric";
        public string Lang { get; set; } = "kr";
    }
}
