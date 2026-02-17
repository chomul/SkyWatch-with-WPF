using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SkyWatch.Services;

public class LocalizationManager : INotifyPropertyChanged
{
    private static LocalizationManager? _instance;
    public static LocalizationManager Instance => _instance ??= new LocalizationManager();

    private Dictionary<string, string> _strings = new();

    private LocalizationManager()
    {
        SetLanguage("kr"); // 기본값
    }

    public void SetLanguage(string lang)
    {
        if (lang == "en")
        {
            _strings = new Dictionary<string, string>
            {
                // Navigation
                ["Nav_Home"] = "Home",
                ["Nav_Search"] = "Search",
                ["Nav_Favorites"] = "Favorites",
                ["Nav_Settings"] = "Settings",

                // Home
                ["Home_Sunrise"] = "Sunrise",
                ["Home_Sunset"] = "Sunset",
                ["Home_Daylight"] = "Daylight",
                ["Home_Humidity"] = "Humidity",
                ["Home_Wind"] = "Wind",
                ["Home_UV"] = "UV Index",
                ["Home_Visibility"] = "Visibility",
                ["Home_FeelsLike"] = "Feels Like",
                ["Home_Hourly"] = "Hourly Forecast",
                ["Home_Daily"] = "Daily Forecast",
                ["Home_Loading"] = "Loading...",
                ["Home_Today"] = "Today",
                ["Home_BelowAvg"] = "Below Average",
                ["Home_VeryGood"] = "Very Good",
                ["Home_Now"] = "Now",
                ["Home_Refresh"] = "Refresh",

                // Search
                ["Search_Title"] = "City Search",
                ["Search_Placeholder"] = "Enter city name...",
                ["Search_Results"] = "Search Results",
                ["Search_Recent"] = "Recent Searches",
                ["Search_NoResult"] = "No results found.",
                ["Search_Guide"] = "Enter a city name to see search results.",

                // Favorites
                ["Fav_Title"] = "Favorite Cities",
                ["Fav_Empty"] = "No favorite cities yet.",
                ["Fav_SunInfo"] = "Sunrise · Sunset",

                // Formats
                ["Format_Daylight"] = "Daylight {0}h {1}m",

                // Settings
                ["Settings_Title"] = "Settings",
                ["Settings_Unit"] = "Unit Settings",
                ["Settings_Celsius"] = "Celsius (°C)",
                ["Settings_Fahrenheit"] = "Fahrenheit (°F)",
                ["Settings_LangHeader"] = "Language",
                ["Settings_Info"] = "Information",
                ["Settings_App"] = "App Name:",
                ["Settings_Ver"] = "Version:",
            };
        }
        else // default to kr
        {
            _strings = new Dictionary<string, string>
            {
                // Navigation
                ["Nav_Home"] = "홈",
                ["Nav_Search"] = "검색",
                ["Nav_Favorites"] = "즐겨찾기",
                ["Nav_Settings"] = "설정",

                // Home
                ["Home_Sunrise"] = "일출",
                ["Home_Sunset"] = "일몰",
                ["Home_Daylight"] = "낮 길이",
                ["Home_Humidity"] = "습도",
                ["Home_Wind"] = "바람",
                ["Home_UV"] = "자외선",
                ["Home_Visibility"] = "가시거리",
                ["Home_FeelsLike"] = "체감",
                ["Home_Hourly"] = "시간별 예보",
                ["Home_Daily"] = "주간 예보",
                ["Home_Loading"] = "로딩 중...",
                ["Home_Today"] = "오늘",
                ["Home_BelowAvg"] = "평균 이하",
                ["Home_VeryGood"] = "매우 좋음",
                ["Home_Now"] = "지금",
                ["Home_Refresh"] = "갱신",

                // Search
                ["Search_Title"] = "도시 검색",
                ["Search_Placeholder"] = "도시 이름을 입력하세요...",
                ["Search_Results"] = "검색 결과",
                ["Search_Recent"] = "최근 검색",
                ["Search_NoResult"] = "검색 결과가 없습니다.",
                ["Search_Guide"] = "도시 이름을 입력하면 검색 결과가 표시됩니다.",

                // Favorites
                ["Fav_Title"] = "즐겨찾기 목록",
                ["Fav_Empty"] = "등록된 즐겨찾기가 없습니다.",
                ["Fav_SunInfo"] = "일출 · 일몰",

                // Formats
                ["Format_Daylight"] = "낮 {0}시간 {1}분",

                // Settings
                ["Settings_Title"] = "설정",
                ["Settings_Unit"] = "단위 설정",
                ["Settings_Celsius"] = "섭씨 (°C)",
                ["Settings_Fahrenheit"] = "화씨 (°F)",
                ["Settings_LangHeader"] = "언어 설정",
                ["Settings_Info"] = "정보",
                ["Settings_App"] = "이름:",
                ["Settings_Ver"] = "버전:",
            };
        }

        OnPropertyChanged(string.Empty); // 모든 속성 변경 알림
    }

    public string this[string key] => _strings.TryGetValue(key, out var value) ? value : key;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
