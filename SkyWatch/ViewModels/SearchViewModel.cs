using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkyWatch.Models;
using SkyWatch.Services;

namespace SkyWatch.ViewModels;

/// <summary>
/// 검색 화면 ViewModel — 도시 검색 + 최근 검색 관리
/// </summary>
public partial class SearchViewModel : ViewModelBase
{
    private readonly MockSearchService _searchService = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    public ObservableCollection<SearchResult> SearchResults { get; } = new();
    public ObservableCollection<string> RecentSearches { get; } = new();

    public SearchViewModel()
    {
        Title = "도시 검색";

        // 더미 최근 검색
        RecentSearches.Add("Seoul");
        RecentSearches.Add("Tokyo");
        RecentSearches.Add("New York");
    }

    // SearchQuery 변경 시 자동 검색
    partial void OnSearchQueryChanged(string value)
    {
        _ = SearchCitiesAsync(value);
    }

    private async Task SearchCitiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            SearchResults.Clear();
            return;
        }

        try
        {
            IsSearching = true;
            var results = await _searchService.SearchCitiesAsync(query);

            SearchResults.Clear();
            foreach (var r in results)
                SearchResults.Add(r);
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private void ClearQuery()
    {
        SearchQuery = string.Empty;
    }

    [RelayCommand]
    private void SelectRecentSearch(string city)
    {
        SearchQuery = city;
    }
}

