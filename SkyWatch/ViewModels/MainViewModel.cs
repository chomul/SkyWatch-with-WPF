using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkyWatch.Models;

namespace SkyWatch.ViewModels;

/// <summary>
/// 메인 ViewModel — 네비게이션 + 즐겨찾기 패널 관리
/// </summary>
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private int _selectedNavIndex;

    // ── 즐겨찾기 ──
    public ObservableCollection<FavoriteCity> FavoriteCities { get; } = new();

    [ObservableProperty]
    private string _sunriseTime = "07:22";

    [ObservableProperty]
    private string _sunsetTime = "18:05";

    [ObservableProperty]
    private string _daylightDuration = "낮 10시간 43분";

    public HomeViewModel HomeVM { get; } = new();
    public SearchViewModel SearchVM { get; } = new();
    public FavoritesViewModel FavoritesVM { get; } = new();
    public SettingsViewModel SettingsVM { get; } = new();

    public MainViewModel()
    {
        _currentView = HomeVM;
        _selectedNavIndex = 0;
        InitializeFavorites();
    }

    private void InitializeFavorites()
    {
        FavoriteCities.Add(new FavoriteCity
        {
            CityName = "Seoul",
            CountryCode = "KR",
            FlagEmoji = "🇰🇷",
            Temperature = -2,
            IconCode = "04d",
            IsActive = true
        });
        FavoriteCities.Add(new FavoriteCity
        {
            CityName = "New York",
            CountryCode = "US",
            FlagEmoji = "🇺🇸",
            Temperature = 5,
            IconCode = "02d",
            IsActive = false
        });
        FavoriteCities.Add(new FavoriteCity
        {
            CityName = "Tokyo",
            CountryCode = "JP",
            FlagEmoji = "🇯🇵",
            Temperature = 8,
            IconCode = "01d",
            IsActive = false
        });
        FavoriteCities.Add(new FavoriteCity
        {
            CityName = "London",
            CountryCode = "GB",
            FlagEmoji = "🇬🇧",
            Temperature = 3,
            IconCode = "10d",
            IsActive = false
        });
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

