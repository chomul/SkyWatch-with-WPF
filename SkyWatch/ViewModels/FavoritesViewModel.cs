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

    [RelayCommand]
    private void SelectFavorite(FavoriteCity city)
    {
        if (city == null) return;

        // SearchResult로 변환하여 메시지 전송
        var searchResult = new SearchResult
        {
            CityName = city.CityName,
            CountryName = city.CountryCode, // FavoriteCity에는 CountryName이 없으므로 Code 사용
            CountryCode = city.CountryCode,
            FlagEmoji = city.FlagEmoji,
            Lat = city.Lat,
            Lon = city.Lon
        };

        WeakReferenceMessenger.Default.Send(new CitySelectedMessage(searchResult));
    }

    [RelayCommand]
    private void DeleteFavorite(FavoriteCity city)
    {
        if (city == null) return;
        WeakReferenceMessenger.Default.Send(new ToggleFavoriteMessage(city));
    }
}
