namespace SkyWatch.Models;

/// <summary>
/// 현재 날씨 정보
/// </summary>
public class CurrentWeather
{
    /// <summary>도시 이름</summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>국가 코드 (KR, US 등)</summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>날씨 설명 (대체로 맑음, 비 등)</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>날씨 아이콘 코드 (OpenWeatherMap 형식: 01d, 10n 등)</summary>
    public string IconCode { get; set; } = string.Empty;

    /// <summary>현재 온도 (°C)</summary>
    public double Temperature { get; set; }

    /// <summary>체감 온도 (°C)</summary>
    public double FeelsLike { get; set; }

    /// <summary>최고 온도 (°C)</summary>
    public double TempMax { get; set; }

    /// <summary>최저 온도 (°C)</summary>
    public double TempMin { get; set; }

    /// <summary>습도 (%)</summary>
    public int Humidity { get; set; }

    /// <summary>풍속 (m/s)</summary>
    public double WindSpeed { get; set; }

    /// <summary>풍향 설명 (북동풍 등)</summary>
    public string WindDirection { get; set; } = string.Empty;

    /// <summary>가시거리 (km)</summary>
    public double Visibility { get; set; }

    /// <summary>UV 지수</summary>
    public int UvIndex { get; set; }

    /// <summary>UV 등급 설명 (보통, 높음 등)</summary>
    public string UvDescription { get; set; } = string.Empty;

    /// <summary>일출 시간</summary>
    public DateTime Sunrise { get; set; }

    /// <summary>일몰 시간</summary>
    public DateTime Sunset { get; set; }

    /// <summary>마지막 갱신 시간</summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 시간별 예보
/// </summary>
public class HourlyForecast
{
    /// <summary>예보 시간</summary>
    public DateTime Time { get; set; }

    /// <summary>온도 (°C)</summary>
    public double Temperature { get; set; }

    /// <summary>날씨 아이콘 코드</summary>
    public string IconCode { get; set; } = string.Empty;

    /// <summary>강수 확률 (0~100)</summary>
    public int RainChance { get; set; }

    /// <summary>현재 시간 여부</summary>
    public bool IsNow { get; set; }
}

/// <summary>
/// 일별 예보
/// </summary>
public class DailyForecast
{
    /// <summary>요일 이름 (오늘, 내일, 수, 목 등)</summary>
    public string DayName { get; set; } = string.Empty;

    /// <summary>최고 온도 (°C)</summary>
    public double TempMax { get; set; }

    /// <summary>최저 온도 (°C)</summary>
    public double TempMin { get; set; }

    /// <summary>날씨 아이콘 코드</summary>
    public string IconCode { get; set; } = string.Empty;

    /// <summary>날씨 설명</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>강수 확률 (0~100)</summary>
    public int RainChance { get; set; }

    /// <summary>오늘 여부</summary>
    public bool IsToday { get; set; }

    /// <summary>온도 바 비율 (0.0 ~ 1.0) — UI 표시용</summary>
    public double TempBarRatio { get; set; }
}

/// <summary>
/// 전체 날씨 정보를 통합하는 루트 모델
/// </summary>
public class WeatherInfo
{
    /// <summary>현재 날씨</summary>
    public CurrentWeather Current { get; set; } = new();

    /// <summary>시간별 예보 목록</summary>
    public List<HourlyForecast> HourlyForecasts { get; set; } = [];

    /// <summary>일별 예보 목록</summary>
    public List<DailyForecast> DailyForecasts { get; set; } = [];
}
