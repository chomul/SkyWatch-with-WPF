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
public partial class MainViewModel : ObservableObject, IRecipient<CitySelectedMessage>, IRecipient<ToggleFavoriteMessage>, IRecipient<SettingsChangedMessage>
{
    private readonly IFavoritesService _favoritesService;
    private readonly IWeatherService _weatherService;
    private readonly ISettingsService _settingsService;

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
    public FavoritesViewModel FavoritesVM { get; }
    public SettingsViewModel SettingsVM { get; }

    public MainViewModel() : this(new FavoritesService(), new OpenWeatherService(), new SettingsService())
    {
    }

    public MainViewModel(IFavoritesService favoritesService, IWeatherService weatherService, ISettingsService settingsService)
    {
        _favoritesService = favoritesService;
        _weatherService = weatherService;
        _settingsService = settingsService;

        HomeVM = new HomeViewModel(_weatherService, _settingsService);
        _currentView = HomeVM;
        _selectedNavIndex = 0;

        // FavoritesVM은 MainVM의 컬렉션을 공유
        FavoritesVM = new FavoritesViewModel(FavoriteCities);

        // SettingsVM 생성
        SettingsVM = new SettingsViewModel(_settingsService);

        // 초기화 (설정 로드 → 즐겨찾기 로드 → 날씨 업데이트)
        _ = InitializeAsync();

        // 메시지 수신 등록
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    private async Task InitializeAsync()
    {
        // 1. 설정 로드
        await _settingsService.LoadSettingsAsync();
        SettingsVM.LoadCurrentSettings();

        // 2. 즐겨찾기 로드
        var favorites = await _favoritesService.LoadFavoritesAsync();
        FavoriteCities.Clear();
        foreach (var city in favorites)
        {
            FavoriteCities.Add(city);
        }
        await LoadFavoritesWeatherAsync();

        // 초기 홈 화면 도시가 즐겨찾기인지 확인
        CheckIfHomeIsFavorite();
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

        // 즐겨찾기 상태 확인 (비동기 로드 완료 후 체크해야 정확하지만, 
        // 여기서는 메시지의 위경도로 즉시 확인)
        CheckIfFavorite(city.Lat, city.Lon);
    }

    /// <summary>
    /// 즐겨찾기 토글 메시지 수신 시 호출 (HomeViewModel에서 발생)
    /// </summary>
    public void Receive(ToggleFavoriteMessage message)
    {
        // ToggleFavorite 커맨드 실행
        _ = ToggleFavorite(message.Value);
    }

    /// <summary>
    /// 설정 변경 메시지 수신 (언어/단위 변경 시 일출/일몰 텍스트 갱신 등을 위해)
    /// </summary>
    public void Receive(SettingsChangedMessage message)
    {
        var activeCity = FavoriteCities.FirstOrDefault(c => c.IsActive);
        if (activeCity != null)
        {
            // 언어 변경 시 낮 길이 텍스트 등을 갱신하기 위해 재호출
            _ = UpdateSunriseSunsetAsync(activeCity);
        }

        // 단위 변경 시 즐겨찾기 목록의 온도를 갱신해야 함 via API reload
        if (message.Value == "Unit")
        {
            _ = LoadFavoritesWeatherAsync();
        }
    }

    /// <summary>
    /// 즐겨찾기 추가/삭제 토글
    /// </summary>
    /// <param name="data">SearchResult, CurrentWeather, 또는 FavoriteCity</param>
    [RelayCommand]
    private async Task ToggleFavorite(object data)
    {
        if (data == null) return;

        string cityName = "";
        string countryCode = "";
        string flagEmoji = "";
        double lat = 0;
        double lon = 0;

        // 1. 데이터 타입에 따라 정보 추출
        if (data is SearchResult searchResult)
        {
            cityName = searchResult.CityName;
            countryCode = searchResult.CountryCode;
            flagEmoji = searchResult.FlagEmoji;
            lat = searchResult.Lat;
            lon = searchResult.Lon;
        }
        else if (data is CurrentWeather weather)
        {
            cityName = weather.CityName;
            countryCode = weather.CountryCode;
            // CurrentWeather에는 Flag가 없으므로 Helper를 통해 생성
            flagEmoji = SkyWatch.Helpers.CountryHelper.CountryCodeToFlag(countryCode);
            lat = weather.Lat;
            lon = weather.Lon;
        }
        else if (data is FavoriteCity favCity)
        {
            cityName = favCity.CityName;
            lat = favCity.Lat;
            lon = favCity.Lon;
        }

        // 2. 이미 존재하는지 확인 (위경도 오차 고려하여 이름으로 비교하거나, 대략적인 좌표 비교)
        // 여기서는 이름과 국가코드로 1차 비교 후, 없으면 좌표로 비교
        var existing = FavoriteCities.FirstOrDefault(c =>
            (c.CityName == cityName && c.CountryCode == countryCode) ||
            (Math.Abs(c.Lat - lat) < 0.01 && Math.Abs(c.Lon - lon) < 0.01));

        if (existing != null)
        {
            // 제거
            FavoriteCities.Remove(existing);
        }
        else
        {
            // 추가
            var newFavorite = new FavoriteCity
            {
                CityName = cityName,
                CountryCode = countryCode,
                FlagEmoji = flagEmoji, // CurrentWeather에서 온 경우 비어있을 수 있음
                Lat = lat,
                Lon = lon,
                IsActive = false
            };
            FavoriteCities.Add(newFavorite);

            // 추가된 도시 날씨 즉시 로드
            await UpdateCityWeatherAsync(newFavorite);
        }

        // 3. 변경사항 저장
        await _favoritesService.SaveFavoritesAsync(FavoriteCities.ToList());

        // 4. 상태 업데이트
        CheckIfHomeIsFavorite();
    }

    private void CheckIfHomeIsFavorite()
    {
        if (HomeVM.CurrentWeather == null) return;
        CheckIfFavorite(HomeVM.CurrentWeather.Lat, HomeVM.CurrentWeather.Lon);
    }

    private void CheckIfFavorite(double lat, double lon)
    {
        var isFav = FavoriteCities.Any(c => Math.Abs(c.Lat - lat) < 0.01 && Math.Abs(c.Lon - lon) < 0.01);
        HomeVM.IsFavorite = isFav;
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
            await UpdateSunriseSunsetAsync(activeCity);
    }

    /// <summary>
    /// 개별 도시 날씨 업데이트
    /// </summary>

    private async Task UpdateCityWeatherAsync(FavoriteCity city)
    {
        try
        {
            // 위경도 기반 조회
            var weatherInfo = await _weatherService.GetWeatherAsync(city.Lat, city.Lon);

            city.Temperature = weatherInfo.Current.Temperature;
            city.IconCode = weatherInfo.Current.IconCode;

            // Flag가 비어있다면(홈에서 추가한 경우) 채워주기 시도? 
            // (GeocodingService가 아니어서 Flag 변환 로직이 없음. 일단 유지)
        }
        catch
        {
            // API 에러 시 기존 값 유지
        }
    }

    /// <summary>
    /// 활성 도시의 일출/일몰 시간 업데이트
    /// </summary>
    private async Task UpdateSunriseSunsetAsync(FavoriteCity city)
    {
        try
        {
            // 위경도 기반 조회
            var weatherInfo = await _weatherService.GetWeatherAsync(city.Lat, city.Lon);
            var current = weatherInfo.Current;

            // 일출/일몰 및 낮 길이 계산
            var sunrise = current.Sunrise;
            var sunset = current.Sunset;
            var daylight = sunset - sunrise;

            SunriseTime = sunrise.ToString("HH:mm");
            SunsetTime = sunset.ToString("HH:mm");

            var format = LocalizationManager.Instance["Format_Daylight"]; // "Daylight {0}h {1}m"
            DaylightDuration = string.Format(format, daylight.Hours, daylight.Minutes);
        }
        catch
        {
            SunriseTime = "--:--";
            SunsetTime = "--:--";
            DaylightDuration = "";
        }
    }

    [RelayCommand]
    private async Task SelectFavoriteCity(FavoriteCity city)
    {
        if (city == null) return;

        // 1. 홈 화면으로 이동
        NavigateTo("Home");

        // 2. 날씨 로드
        await HomeVM.LoadWeatherAsync(city.Lat, city.Lon, city.CityName);

        // 3. 활성 상태 업데이트
        foreach (var c in FavoriteCities)
        {
            c.IsActive = (c == city);
        }

        // 4. 일출/일몰 업데이트
        await UpdateSunriseSunsetAsync(city);

        // 5. 홈 화면 즐겨찾기 상태 업데이트
        CheckIfHomeIsFavorite();
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
