using SkyWatch.Models;

namespace SkyWatch.Services;

/// <summary>
/// 더미 데이터를 반환하는 Mock 날씨 서비스.
/// UI 개발 및 테스트용 — 목업 HTML과 동일한 Seoul 데이터를 제공합니다.
/// Stage 3에서 실제 OpenWeatherMap API 서비스로 교체됩니다.
/// </summary>
public class MockWeatherService : IWeatherService
{
    public async Task<WeatherInfo> GetWeatherAsync(string city)
    {
        // 네트워크 호출을 시뮬레이션 (UI 로딩 상태 테스트용)
        await Task.Delay(500);

        var now = DateTime.Now;

        return new WeatherInfo
        {
            Current = new CurrentWeather
            {
                CityName = "Seoul",
                CountryCode = "KR",
                Description = "대체로 맑음",
                IconCode = "02d",
                Temperature = 23,
                FeelsLike = 21,
                TempMax = 27,
                TempMin = 16,
                Humidity = 62,
                WindSpeed = 4.2,
                WindDirection = "북동풍 ↗",
                Visibility = 10,
                UvIndex = 5,
                UvDescription = "보통",
                Sunrise = now.Date.AddHours(6).AddMinutes(12),
                Sunset = now.Date.AddHours(19).AddMinutes(38),
                UpdatedAt = now
            },

            HourlyForecasts = new List<HourlyForecast>
            {
                new() { Time = now, Temperature = 23, IconCode = "02d", RainChance = 0, IsNow = true },
                new() { Time = now.Date.AddHours(18), Temperature = 21, IconCode = "03d", RainChance = 0 },
                new() { Time = now.Date.AddHours(21), Temperature = 18, IconCode = "01n", RainChance = 10 },
                new() { Time = now.Date.AddDays(1).AddHours(0), Temperature = 16, IconCode = "10n", RainChance = 45 },
                new() { Time = now.Date.AddDays(1).AddHours(3), Temperature = 15, IconCode = "10n", RainChance = 60 },
                new() { Time = now.Date.AddDays(1).AddHours(6), Temperature = 16, IconCode = "03d", RainChance = 20 },
                new() { Time = now.Date.AddDays(1).AddHours(9), Temperature = 19, IconCode = "02d", RainChance = 5 },
            },

            DailyForecasts = new List<DailyForecast>
            {
                new() { DayName = "오늘", TempMax = 27, TempMin = 16, IconCode = "02d", Description = "대체로 맑음", RainChance = 10, IsToday = true, TempBarRatio = 0.70 },
                new() { DayName = "내일", TempMax = 22, TempMin = 14, IconCode = "10d", Description = "비", RainChance = 70, TempBarRatio = 0.45 },
                new() { DayName = "수",   TempMax = 24, TempMin = 15, IconCode = "03d", Description = "구름 많음", RainChance = 25, TempBarRatio = 0.55 },
                new() { DayName = "목",   TempMax = 29, TempMin = 17, IconCode = "01d", Description = "맑음", RainChance = 0, TempBarRatio = 0.80 },
                new() { DayName = "금",   TempMax = 30, TempMin = 18, IconCode = "01d", Description = "맑음", RainChance = 0, TempBarRatio = 0.85 },
                new() { DayName = "토",   TempMax = 26, TempMin = 16, IconCode = "03d", Description = "구름 조금", RainChance = 15, TempBarRatio = 0.65 },
                new() { DayName = "일",   TempMax = 27, TempMin = 15, IconCode = "02d", Description = "대체로 맑음", RainChance = 5, TempBarRatio = 0.72 },
            }
        };
    }
}
