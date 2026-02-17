namespace SkyWatch.Models;

/// <summary>
/// 즐겨찾기 패널에 표시되는 도시 정보.
/// </summary>
public class FavoriteCity
{
    /// <summary>도시 이름 (예: Seoul)</summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>국가 코드 (예: KR)</summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>국기 이모지 (예: 🇰🇷)</summary>
    public string FlagEmoji { get; set; } = string.Empty;

    /// <summary>현재 온도</summary>
    public double Temperature { get; set; }

    /// <summary>날씨 아이콘 코드 (예: "01d")</summary>
    public string IconCode { get; set; } = string.Empty;

    /// <summary>위도</summary>
    public double Lat { get; set; }

    /// <summary>경도</summary>
    public double Lon { get; set; }

    /// <summary>현재 선택(활성)된 도시 여부</summary>
    public bool IsActive { get; set; }
}
