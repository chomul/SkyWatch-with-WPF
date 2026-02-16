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
    /// <param name="city">도시 이름 (예: "Seoul")</param>
    /// <returns>현재 날씨 + 시간별/일별 예보</returns>
    Task<WeatherInfo> GetWeatherAsync(string city);
}
