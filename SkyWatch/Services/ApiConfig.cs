using System.IO;
using System.Text.Json;

namespace SkyWatch.Services;

/// <summary>
/// OpenWeatherMap API 설정을 appsettings.json에서 읽어옵니다.
/// </summary>
public static class ApiConfig
{
    /// <summary>API 키</summary>
    public static string ApiKey { get; private set; } = string.Empty;

    /// <summary>날씨 API 기본 URL</summary>
    public const string BaseUrl = "https://api.openweathermap.org/data/2.5";

    /// <summary>Geocoding API 기본 URL</summary>
    public const string GeoUrl = "https://api.openweathermap.org/geo/1.0";

    /// <summary>단위 (metric = 섭씨, imperial = 화씨)</summary>
    public static string Units { get; set; } = "metric";

    /// <summary>현재 단위 라벨 (°C 또는 °F)</summary>
    public static string UnitLabel => Units == "metric" ? "°C" : "°F";

    /// <summary>풍속 단위 라벨 (m/s 또는 mph)</summary>
    public static string WindUnitLabel => Units == "metric" ? "m/s" : "mph";

    /// <summary>언어 (kr = 한국어 날씨 설명)</summary>
    public static string Lang { get; set; } = "kr";

    /// <summary>
    /// appsettings.json에서 API 키를 로드합니다.
    /// 앱 시작 시 한 번 호출하세요.
    /// </summary>
    public static void Load()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        // 개발 시 프로젝트 루트의 appsettings.json도 탐색
        if (!File.Exists(path))
        {
            var devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "appsettings.json");
            if (File.Exists(devPath))
                path = devPath;
        }

        if (!File.Exists(path))
        {
            System.Diagnostics.Debug.WriteLine("⚠️ appsettings.json을 찾을 수 없습니다. API 키를 설정해주세요.");
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("OpenWeatherMap", out var owm)
                && owm.TryGetProperty("ApiKey", out var key))
            {
                ApiKey = key.GetString() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"⚠️ appsettings.json 파싱 에러: {ex.Message}");
        }
    }

    /// <summary>API 키가 설정되었는지 확인</summary>
    public static bool IsConfigured =>
        !string.IsNullOrEmpty(ApiKey) && ApiKey != "여기에_API_키를_입력하세요";
}
