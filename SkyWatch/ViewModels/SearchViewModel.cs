using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkyWatch.Messages;
using SkyWatch.Models;
using SkyWatch.Services;

namespace SkyWatch.ViewModels;

/// <summary>
/// 검색 화면 ViewModel — 도시 검색 + 최근 검색 관리
/// </summary>
public partial class SearchViewModel : ViewModelBase
{
    private readonly GeocodingService _searchService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private bool _isSearching;

    public ObservableCollection<SearchResult> SearchResults { get; } = new();
    public ObservableCollection<string> RecentSearches { get; } = new();

    public SearchViewModel() : this(new GeocodingService())
    {
    }

    public SearchViewModel(GeocodingService searchService)
    {
        Title = "도시 검색";
        _searchService = searchService;
    }

    // 엔터 키 입력 시 검색 실행
    [RelayCommand]
    private async Task SearchAsync()
    {
        await SearchCitiesAsync(SearchQuery);
    }

    private async Task SearchCitiesAsync(string query)
    {
        if (IsSearching)
        {
            // 이미 검색 중이면 추가 요청 무시
            return;
        }

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

            // 검색 결과가 있으면 최근 검색에 추가
            if (results.Count > 0)
                AddToRecentSearches(query.Trim());
        }
        finally
        {
            IsSearching = false;
        }
    }

    /// <summary>
    /// 최근 검색 목록에 추가 (중복 제거, 최대 8개)
    /// </summary>
    private void AddToRecentSearches(string query)
    {
        // 이미 있으면 제거 (맨 앞으로 이동시키기 위해)
        var existing = RecentSearches.FirstOrDefault(
            s => s.Equals(query, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
            RecentSearches.Remove(existing);

        // 맨 앞에 추가
        RecentSearches.Insert(0, query);

        // 최대 8개 유지
        while (RecentSearches.Count > 8)
            RecentSearches.RemoveAt(RecentSearches.Count - 1);
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
        // 선택 시 바로 검색 실행
        _ = SearchAsync();
    }

    /// <summary>
    /// 검색 결과 항목 선택 시 호출
    /// </summary>
    [RelayCommand]
    private void SelectCity(SearchResult city)
    {
        if (city == null) return;

        // 선택된 도시 정보를 메시지로 전송 (MainViewModel에서 수신)
        WeakReferenceMessenger.Default.Send(new CitySelectedMessage(city));
    }
}

