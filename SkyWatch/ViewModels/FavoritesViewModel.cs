using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkyWatch.Messages;
using SkyWatch.Models;

namespace SkyWatch.ViewModels;

/// <summary>
/// 즐겨찾기 화면 ViewModel — 저장된 도시 관리
/// </summary>
public partial class FavoritesViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<FavoriteCity> _favoriteCities;

    public FavoritesViewModel(ObservableCollection<FavoriteCity> favorites)
    {
        Title = "즐겨찾기";
        _favoriteCities = favorites;
    }

    [ObservableProperty]
    private FavoriteCity? _selectedCity;

    partial void OnSelectedCityChanged(FavoriteCity? value)
    {
        if (value == null) return;

        // 선택 상태 동기화 (IsActive 업데이트)
        foreach (var city in FavoriteCities)
        {
            city.IsActive = (city == value);
        }

        // 메시지 전송
        var searchResult = new SearchResult
        {
            CityName = value.CityName,
            CountryName = value.CountryCode,
            CountryCode = value.CountryCode,
            FlagEmoji = value.FlagEmoji,
            Lat = value.Lat,
            Lon = value.Lon
        };

        WeakReferenceMessenger.Default.Send(new CitySelectedMessage(searchResult));
    }

    [RelayCommand]
    private void SelectFavorite(FavoriteCity city)
    {
        SelectedCity = city;
    }

    [RelayCommand]
    private void DeleteFavorite(FavoriteCity city)
    {
        if (city == null) return;
        WeakReferenceMessenger.Default.Send(new ToggleFavoriteMessage(city));
    }
}
