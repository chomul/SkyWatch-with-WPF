using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SkyWatch.ViewModels;

/// <summary>
/// 메인 ViewModel — 네비게이션 관리
/// </summary>
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private int _selectedNavIndex;

    public HomeViewModel HomeVM { get; } = new();
    public SearchViewModel SearchVM { get; } = new();
    public FavoritesViewModel FavoritesVM { get; } = new();
    public SettingsViewModel SettingsVM { get; } = new();

    public MainViewModel()
    {
        _currentView = HomeVM;
        _selectedNavIndex = 0;
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
