using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkyWatch.Messages;
using SkyWatch.Models;
using SkyWatch.Services;

namespace SkyWatch.ViewModels;

/// <summary>
/// 메인 ViewModel — 네비게이션 + 즐겨찾기 패널 관리
/// 즐겨찾기 도시의 실제 날씨를 API에서 로드합니다.
/// </summary>
public partial class MainViewModel : ObservableObject, IRecipient<CitySelectedMessage>
{
    private static readonly HttpClient _http = new();

    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private int _selectedNavIndex;

    // ── 즐겨찾기 ──
    public ObservableCollection<FavoriteCity> FavoriteCities { get; } = new();

    [ObservableProperty]
    private string _sunriseTime = "--:--";

    [ObservableProperty]
    private string _sunsetTime = "--:--";

    [ObservableProperty]
    private string _daylightDuration = "로딩 중...";

    public HomeViewModel HomeVM { get; } = new();
    public SearchViewModel SearchVM { get; } = new();
    public FavoritesViewModel FavoritesVM { get; } = new();
    public SettingsViewModel SettingsVM { get; } = new();

    public MainViewModel()
    {
        _currentView = HomeVM;
        _selectedNavIndex = 0;
        InitializeFavorites();
        _ = LoadFavoritesWeatherAsync();

        // 메시지 수신 등록
        WeakReferenceMessenger.Default.Register(this);
    }

    /// <summary>
    /// 도시 선택 메시지 수신 시 호출
    /// </summary>
    public void Receive(CitySelectedMessage message)
    {
        var city = message.Value;

        // 홈 화면으로 이동
        NavigateTo("Home");

        // 홈 화면에 날씨 로드 요청 (위경도 기반)
        _ = HomeVM.LoadWeatherAsync(city.Lat, city.Lon, city.CityName);
    }

    /// <summary>
    /// 즐겨찾기 도시 초기 목록 (기본값)
    /// </summary>
    private void InitializeFavorites()
    {
        FavoriteCities.Add(new FavoriteCity
        { CityName = "Seoul", CountryCode = "KR", FlagEmoji = "🇰🇷", IsActive = true });
        FavoriteCities.Add(new FavoriteCity
        { CityName = "New York", CountryCode = "US", FlagEmoji = "🇺🇸" });
        FavoriteCities.Add(new FavoriteCity
        { CityName = "Tokyo", CountryCode = "JP", FlagEmoji = "🇯🇵" });
        FavoriteCities.Add(new FavoriteCity
        { CityName = "London", CountryCode = "GB", FlagEmoji = "🇬🇧" });
    }

    /// <summary>
    /// 모든 즐겨찾기 도시의 실제 날씨를 API에서 병렬 로드
    /// </summary>
    private async Task LoadFavoritesWeatherAsync()
    {
        if (!ApiConfig.IsConfigured) return;

        var tasks = FavoriteCities.Select(city => UpdateCityWeatherAsync(city)).ToArray();
        await Task.WhenAll(tasks);

        // 활성 도시의 일출/일몰 업데이트
        var activeCity = FavoriteCities.FirstOrDefault(c => c.IsActive);
        if (activeCity != null)
            await UpdateSunriseSunsetAsync(activeCity.CityName);
    }

    /// <summary>
    /// 개별 도시 날씨 업데이트
    /// </summary>
    private async Task UpdateCityWeatherAsync(FavoriteCity city)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}/weather?q={city.CityName}&appid={ApiConfig.ApiKey}&units={ApiConfig.Units}&lang={ApiConfig.Lang}";
            var json = await _http.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            city.Temperature = root.GetProperty("main").GetProperty("temp").GetDouble();
            city.IconCode = root.GetProperty("weather")[0].GetProperty("icon").GetString() ?? "";
        }
        catch
        {
            // API 에러 시 기본값 유지
        }
    }

    /// <summary>
    /// 활성 도시의 일출/일몰 시간 업데이트
    /// </summary>
    private async Task UpdateSunriseSunsetAsync(string city)
    {
        try
        {
            var url = $"{ApiConfig.BaseUrl}/weather?q={city}&appid={ApiConfig.ApiKey}&units={ApiConfig.Units}";
            var json = await _http.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            var sys = doc.RootElement.GetProperty("sys");

            var sunrise = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunrise").GetInt64()).LocalDateTime;
            var sunset = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunset").GetInt64()).LocalDateTime;
            var daylight = sunset - sunrise;

            SunriseTime = sunrise.ToString("HH:mm");
            SunsetTime = sunset.ToString("HH:mm");
            DaylightDuration = $"낮 {daylight.Hours}시간 {daylight.Minutes}분";
        }
        catch
        {
            SunriseTime = "--:--";
            SunsetTime = "--:--";
            DaylightDuration = "정보 없음";
        }
    }

    [RelayCommand]
    private void NavigateTo(string viewName)
    {
        switch (viewName)
        {
            case "Home":
                CurrentView = HomeVM;
                SelectedNavIndex = 0;
                break;
            case "Search":
                CurrentView = SearchVM;
                SelectedNavIndex = 1;
                break;
            case "Favorites":
                CurrentView = FavoritesVM;
                SelectedNavIndex = 2;
                break;
            case "Settings":
                CurrentView = SettingsVM;
                SelectedNavIndex = 3;
                break;
        }
    }
}
