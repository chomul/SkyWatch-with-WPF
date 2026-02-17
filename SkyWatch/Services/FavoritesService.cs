using System.IO;
using System.Text.Json;
using SkyWatch.Models;

namespace SkyWatch.Services;

public class FavoritesService : IFavoritesService
{
    private readonly string _filePath;

    public FavoritesService()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SkyWatch");
        Directory.CreateDirectory(appDataPath);
        _filePath = Path.Combine(appDataPath, "favorites.json");
    }

    public async Task<List<FavoriteCity>> LoadFavoritesAsync()
    {
        if (!File.Exists(_filePath))
        {
            return GetDefaultFavorites();
        }

        try
        {
            using var stream = File.OpenRead(_filePath);
            var favorites = await JsonSerializer.DeserializeAsync<List<FavoriteCity>>(stream);
            return favorites ?? GetDefaultFavorites();
        }
        catch
        {
            // 파일이 깨졌거나 읽기 실패 시 기본값 반환
            return GetDefaultFavorites();
        }
    }

    public async Task SaveFavoritesAsync(List<FavoriteCity> favorites)
    {
        try
        {
            using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, favorites, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            // 로깅이 있다면 로그를 남기는 것이 좋음 (현재는 무시)
            System.Diagnostics.Debug.WriteLine($"Failed to save favorites: {ex.Message}");
        }
    }

    private List<FavoriteCity> GetDefaultFavorites()
    {
        return new List<FavoriteCity>();
    }
}
