using SkyWatch.Models;

namespace SkyWatch.Services;

/// <summary>
/// 날씨 데이터를 제공하는 서비스 인터페이스
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// 도시 이름으로 날씨 정보를 가져옵니다.
    /// </summary>
    Task<WeatherInfo> GetWeatherAsync(string city);

    /// <summary>
    /// 위도/경도로 날씨 정보를 가져옵니다.
    /// </summary>
    Task<WeatherInfo> GetWeatherAsync(double lat, double lon);
}
