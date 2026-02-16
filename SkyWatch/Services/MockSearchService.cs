using SkyWatch.Models;

namespace SkyWatch.Services;

/// <summary>
/// 도시 검색 목 서비스 — 더미 데이터 반환.
/// </summary>
public class MockSearchService
{
    public Task<List<SearchResult>> SearchCitiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult(new List<SearchResult>());

        var q = query.Trim().ToLowerInvariant();

        // 더미 검색 결과 DB
        var allCities = new List<SearchResult>
        {
            new() { CityName = "Seoul",    CountryName = "대한민국 · 서울특별시", FlagEmoji = "🇰🇷", Temperature = -2 },
            new() { CityName = "Busan",    CountryName = "대한민국 · 부산광역시", FlagEmoji = "🇰🇷", Temperature = 3 },
            new() { CityName = "Busan-si", CountryName = "대한민국 · 부산",      FlagEmoji = "🇰🇷", Temperature = 3 },
            new() { CityName = "Incheon",  CountryName = "대한민국 · 인천광역시", FlagEmoji = "🇰🇷", Temperature = -3 },
            new() { CityName = "Tokyo",    CountryName = "일본 · 도쿄도",        FlagEmoji = "🇯🇵", Temperature = 8 },
            new() { CityName = "New York", CountryName = "미국 · 뉴욕주",        FlagEmoji = "🇺🇸", Temperature = 5 },
            new() { CityName = "London",   CountryName = "영국 · 잉글랜드",      FlagEmoji = "🇬🇧", Temperature = 3 },
            new() { CityName = "Paris",    CountryName = "프랑스 · 일드프랑스",   FlagEmoji = "🇫🇷", Temperature = 6 },
            new() { CityName = "Beijing",  CountryName = "중국 · 베이징시",      FlagEmoji = "🇨🇳", Temperature = 1 },
        };

        var results = allCities
            .Where(c => c.CityName.ToLowerInvariant().Contains(q)
                     || c.CountryName.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // 첫 번째 결과를 TopResult로 표시
        if (results.Count > 0)
            results[0].IsTopResult = true;

        return Task.FromResult(results);
    }
}
