using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkyWatch.Models;
using SkyWatch.Services;

namespace SkyWatch.ViewModels;

/// <summary>
/// 홈 화면 ViewModel — 현재 날씨 + 시간별/주간 예보 표시
/// MockWeatherService에서 데이터를 로드하여 View에 바인딩합니다.
/// </summary>
public partial class HomeViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;

    // ── 현재 날씨 ──

    [ObservableProperty]
    private CurrentWeather? _currentWeather;

    // ── 예보 목록 ──

    [ObservableProperty]
    private ObservableCollection<HourlyForecast> _hourlyForecasts = [];

    [ObservableProperty]
    private ObservableCollection<DailyForecast> _dailyForecasts = [];

    // ── UI 상태 ──

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    // ── 온도 단위 ──

    [ObservableProperty]
    private string _unitLabel = ApiConfig.UnitLabel;

    [ObservableProperty]
    private string _windUnitLabel = ApiConfig.WindUnitLabel;

    public HomeViewModel() : this(new OpenWeatherService())
    {
    }

    public HomeViewModel(IWeatherService weatherService)
    {
        Title = "홈";
        _weatherService = weatherService;

        // 초기 데이터 로드
        _ = LoadWeatherAsync("Seoul");
    }

    /// <summary>
    /// 날씨 데이터를 로드합니다. 로딩/에러 상태를 자동 관리합니다.
    /// </summary>
    [RelayCommand]
    public async Task LoadWeatherAsync(string city)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var weatherInfo = await _weatherService.GetWeatherAsync(city);

            // 현재 날씨
            CurrentWeather = weatherInfo.Current;

            // 시간별 예보
            HourlyForecasts = new ObservableCollection<HourlyForecast>(weatherInfo.HourlyForecasts);

            // 주간 예보
            DailyForecasts = new ObservableCollection<DailyForecast>(weatherInfo.DailyForecasts);
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"날씨 데이터를 불러올 수 없습니다: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 위도/경도로 날씨 데이터를 로드합니다.
    /// cityName은 UI 표시용으로 사용됩니다.
    /// </summary>
    public async Task LoadWeatherAsync(double lat, double lon, string cityName)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var weatherInfo = await _weatherService.GetWeatherAsync(lat, lon);

            // API에서 가져온 도시 이름 대신, 검색된(또는 저장된) 도시 이름을 우선 사용할 수 있음
            // 여기서는 검색된 한글 이름을 유지하기 위해 덮어씁니다.
            if (!string.IsNullOrEmpty(cityName))
            {
                weatherInfo.Current.CityName = cityName;
            }

            // 현재 날씨
            CurrentWeather = weatherInfo.Current;

            // 시간별 예보
            HourlyForecasts = new ObservableCollection<HourlyForecast>(weatherInfo.HourlyForecasts);

            // 주간 예보
            DailyForecasts = new ObservableCollection<DailyForecast>(weatherInfo.DailyForecasts);
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"날씨 데이터를 불러올 수 없습니다: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 현재 도시의 날씨를 다시 로드합니다 (갱신 버튼용).
    /// </summary>
    [RelayCommand]
    private async Task RefreshWeatherAsync()
    {
        var city = CurrentWeather?.CityName ?? "Seoul";
        await LoadWeatherAsync(city);
    }

    /// <summary>
    /// °C ↔ °F 단위 전환 후 날씨 갱신
    /// </summary>
    [RelayCommand]
    private async Task ToggleUnitAsync()
    {
        ApiConfig.Units = ApiConfig.Units == "metric" ? "imperial" : "metric";
        UnitLabel = ApiConfig.UnitLabel;
        WindUnitLabel = ApiConfig.WindUnitLabel;

        var city = CurrentWeather?.CityName ?? "Seoul";
        await LoadWeatherAsync(city);
    }
}
