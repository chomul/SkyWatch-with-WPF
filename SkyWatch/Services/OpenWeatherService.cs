using System.Net.Http;
using System.Text.Json;
using SkyWatch.Models;

namespace SkyWatch.Services;

/// <summary>
/// OpenWeatherMap Free 2.5 API를 사용하는 실제 날씨 서비스.
/// /weather (현재) + /forecast (5일/3시간) 두 API를 조합합니다.
/// </summary>
public class OpenWeatherService : IWeatherService
{
    private static readonly HttpClient _http = new();

    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
        var key = ApiConfig.ApiKey;
        var units = ApiConfig.Units;
        var lang = ApiConfig.Lang;

        // ── 1. 현재 날씨 ──
        var encodedCity = Uri.EscapeDataString(city);
        var currentUrl = $"{ApiConfig.BaseUrl}/weather?q={encodedCity}&appid={key}&units={units}&lang={lang}";
        var currentJson = await _http.GetStringAsync(currentUrl);
        var currentDoc = JsonDocument.Parse(currentJson);
        var currentRoot = currentDoc.RootElement;

        var current = ParseCurrentWeather(currentRoot);

        // ── 2. 5일/3시간 예보 ──
        var forecastUrl = $"{ApiConfig.BaseUrl}/forecast?q={encodedCity}&appid={key}&units={units}&lang={lang}";
        var forecastJson = await _http.GetStringAsync(forecastUrl);
        var forecastDoc = JsonDocument.Parse(forecastJson);
        var forecastRoot = forecastDoc.RootElement;

        var hourly = ParseHourlyForecasts(forecastRoot);
        var daily = ParseDailyForecasts(forecastRoot);

        return new WeatherInfo
        {
            Current = current,
            HourlyForecasts = hourly,
            DailyForecasts = daily
        };
    }

    public async Task<WeatherInfo> GetWeatherAsync(double lat, double lon)
    {
        var key = ApiConfig.ApiKey;
        var units = ApiConfig.Units;
        var lang = ApiConfig.Lang;

        // ── 1. 현재 날씨 ──
        var currentUrl = $"{ApiConfig.BaseUrl}/weather?lat={lat}&lon={lon}&appid={key}&units={units}&lang={lang}";
        var currentJson = await _http.GetStringAsync(currentUrl);
        var currentDoc = JsonDocument.Parse(currentJson);
        var currentRoot = currentDoc.RootElement;

        var current = ParseCurrentWeather(currentRoot);

        // ── 2. 5일/3시간 예보 ──
        var forecastUrl = $"{ApiConfig.BaseUrl}/forecast?lat={lat}&lon={lon}&appid={key}&units={units}&lang={lang}";
        var forecastJson = await _http.GetStringAsync(forecastUrl);
        var forecastDoc = JsonDocument.Parse(forecastJson);
        var forecastRoot = forecastDoc.RootElement;

        var hourly = ParseHourlyForecasts(forecastRoot);
        var daily = ParseDailyForecasts(forecastRoot);

        return new WeatherInfo
        {
            Current = current,
            HourlyForecasts = hourly,
            DailyForecasts = daily
        };
    }

    /// <summary>
    /// /weather 응답 → CurrentWeather 매핑
    /// </summary>
    private static CurrentWeather ParseCurrentWeather(JsonElement root)
    {
        var main = root.GetProperty("main");
        var weather = root.GetProperty("weather")[0];
        var wind = root.GetProperty("wind");
        var sys = root.GetProperty("sys");

        var sunrise = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunrise").GetInt64()).LocalDateTime;
        var sunset = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunset").GetInt64()).LocalDateTime;
        var visibility = root.GetProperty("visibility").GetDouble() / 1000.0; // m → km

        return new CurrentWeather
        {
            CityName = root.GetProperty("name").GetString() ?? "",
            CountryCode = root.GetProperty("sys").GetProperty("country").GetString() ?? "",
            Lat = root.GetProperty("coord").GetProperty("lat").GetDouble(),
            Lon = root.GetProperty("coord").GetProperty("lon").GetDouble(),
            Description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? "",
            IconCode = weather.GetProperty("icon").GetString() ?? "",
            Temperature = main.GetProperty("temp").GetDouble(),
            FeelsLike = main.GetProperty("feels_like").GetDouble(),
            TempMax = main.GetProperty("temp_max").GetDouble(),
            TempMin = main.GetProperty("temp_min").GetDouble(),
            Humidity = main.GetProperty("humidity").GetInt32(),
            WindSpeed = wind.GetProperty("speed").GetDouble(),
            WindDirection = DegreesToDirection(wind.GetProperty("deg").GetInt32()),
            Visibility = Math.Round(visibility, 1),
            UvIndex = 0,               // Free 2.5 API에는 UV 없음
            UvDescription = "N/A",
            Sunrise = sunrise,
            Sunset = sunset,
            UpdatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// /forecast → 가장 가까운 6개 시간별 예보
    /// </summary>
    private static List<HourlyForecast> ParseHourlyForecasts(JsonElement root)
    {
        var list = root.GetProperty("list");
        var results = new List<HourlyForecast>();
        var now = DateTime.Now;
        var isFirst = true;

        foreach (var item in list.EnumerateArray())
        {
            if (results.Count >= 6) break;

            var dt = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).LocalDateTime;
            var main = item.GetProperty("main");
            var weather = item.GetProperty("weather")[0];
            var pop = item.TryGetProperty("pop", out var popVal) ? popVal.GetDouble() : 0;

            results.Add(new HourlyForecast
            {
                Time = dt,
                Temperature = main.GetProperty("temp").GetDouble(),
                IconCode = weather.GetProperty("icon").GetString() ?? "",
                RainChance = (int)(pop * 100),
                IsNow = isFirst
            });
            isFirst = false;
        }

        return results;
    }

    /// <summary>
    /// /forecast → 날짜별 그룹핑 → 일별 최고/최저 계산 (최대 5일)
    /// </summary>
    private static List<DailyForecast> ParseDailyForecasts(JsonElement root)
    {
        var list = root.GetProperty("list");
        var dayNames = new[] { "일", "월", "화", "수", "목", "금", "토" };

        // 날짜별로 그룹핑
        var groups = new Dictionary<DateOnly, List<JsonElement>>();

        foreach (var item in list.EnumerateArray())
        {
            var dt = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).LocalDateTime;
            var dateKey = DateOnly.FromDateTime(dt);

            if (!groups.ContainsKey(dateKey))
                groups[dateKey] = new List<JsonElement>();
            groups[dateKey].Add(item);
        }

        var results = new List<DailyForecast>();
        var today = DateOnly.FromDateTime(DateTime.Now);
        var allTemps = new List<(double max, double min)>();

        // 각 날짜별 최고/최저 온도 계산
        foreach (var (date, items) in groups.OrderBy(g => g.Key))
        {
            var maxTemp = double.MinValue;
            var minTemp = double.MaxValue;
            var maxPop = 0.0;
            string icon = "";
            string desc = "";

            foreach (var item in items)
            {
                var main = item.GetProperty("main");
                var temp = main.GetProperty("temp").GetDouble();
                var tempMax = main.GetProperty("temp_max").GetDouble();
                var tempMin = main.GetProperty("temp_min").GetDouble();

                if (tempMax > maxTemp) maxTemp = tempMax;
                if (tempMin < minTemp) minTemp = tempMin;

                var pop = item.TryGetProperty("pop", out var p) ? p.GetDouble() : 0;
                if (pop > maxPop) maxPop = pop;

                // 대표 아이콘: 낮 시간대 (09~18시) 우선
                var dt = DateTimeOffset.FromUnixTimeSeconds(item.GetProperty("dt").GetInt64()).LocalDateTime;
                if (dt.Hour >= 9 && dt.Hour <= 18 || string.IsNullOrEmpty(icon))
                {
                    var w = item.GetProperty("weather")[0];
                    icon = w.GetProperty("icon").GetString() ?? "";
                    desc = w.GetProperty("description").GetString() ?? "";
                }
            }

            allTemps.Add((maxTemp, minTemp));

            var dayLabel = date == today ? "오늘"
                : date == today.AddDays(1) ? "내일"
                : dayNames[(int)date.DayOfWeek];

            results.Add(new DailyForecast
            {
                DayName = dayLabel,
                TempMax = Math.Round(maxTemp),
                TempMin = Math.Round(minTemp),
                IconCode = icon,
                Description = desc,
                RainChance = (int)(maxPop * 100),
                IsToday = date == today
            });
        }

        // TempBarRatio 계산 (전체 기간 대비 비율)
        if (allTemps.Count > 0)
        {
            var globalMax = allTemps.Max(t => t.max);
            var globalMin = allTemps.Min(t => t.min);
            var range = globalMax - globalMin;

            for (int i = 0; i < results.Count; i++)
            {
                results[i].TempBarRatio = range > 0
                    ? (allTemps[i].max - globalMin) / range
                    : 0.5;
            }
        }

        return results;
    }

    /// <summary>
    /// 풍향 각도(0~360) → 한국어 방위명
    /// </summary>
    private static string DegreesToDirection(int degrees)
    {
        var dirs = new[] { "북", "북동", "동", "남동", "남", "남서", "서", "북서" };
        var index = (int)Math.Round(degrees / 45.0) % 8;
        return dirs[index] + "풍";
    }
}
