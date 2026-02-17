namespace SkyWatch.Models;

/// <summary>
/// 도시 검색 결과 모델.
/// </summary>
public class SearchResult
{
    /// <summary>도시 이름 (예: Busan)</summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>국가/지역명 (예: 대한민국 · 부산광역시)</summary>
    public string CountryName { get; set; } = string.Empty;

    /// <summary>국기 이모지 (예: 🇰🇷)</summary>
    public string FlagEmoji { get; set; } = string.Empty;

    /// <summary>현재 온도</summary>
    public double Temperature { get; set; }

    /// <summary>위도</summary>
    public double Lat { get; set; }

    /// <summary>경도</summary>
    public double Lon { get; set; }

    /// <summary>최상위 결과 여부 (하이라이트용)</summary>
    public bool IsTopResult { get; set; }
}
