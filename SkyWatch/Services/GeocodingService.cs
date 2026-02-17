using System.Net.Http;
using System.Text.Json;
using SkyWatch.Models;

namespace SkyWatch.Services;

/// <summary>
/// OpenWeatherMap Geocoding API를 사용하는 도시 검색 서비스.
/// MockSearchService를 대체합니다.
/// </summary>
public class GeocodingService
{
    private static readonly HttpClient _http = new();

    /// <summary>
    /// 국가 코드 → 국기 이모지 변환 (ISO 3166-1 alpha-2)
    /// </summary>
    private static string CountryCodeToFlag(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != 2) return "🏳️";
        // 국가코드 문자를 Regional Indicator Symbol으로 변환
        return string.Concat(code.ToUpper().Select(c => char.ConvertFromUtf32(c + 0x1F1A5)));
    }

    /// <summary>
    /// 주요 국가 코드 → 한국어 국가명
    /// </summary>
    private static string CountryCodeToName(string code)
    {
        return code switch
        {
            "KR" => "대한민국",
            "JP" => "일본",
            "US" => "미국",
            "GB" => "영국",
            "FR" => "프랑스",
            "DE" => "독일",
            "CN" => "중국",
            "TW" => "대만",
            "HK" => "홍콩",
            "SG" => "싱가포르",
            "TH" => "태국",
            "VN" => "베트남",
            "PH" => "필리핀",
            "ID" => "인도네시아",
            "MY" => "말레이시아",
            "IN" => "인도",
            "AU" => "호주",
            "CA" => "캐나다",
            "IT" => "이탈리아",
            "ES" => "스페인",
            "RU" => "러시아",
            "BR" => "브라질",
            "MX" => "멕시코",
            "NZ" => "뉴질랜드",
            "SE" => "스웨덴",
            "NO" => "노르웨이",
            "FI" => "핀란드",
            "DK" => "덴마크",
            "NL" => "네덜란드",
            "CH" => "스위스",
            "AT" => "오스트리아",
            "BE" => "벨기에",
            "PT" => "포르투갈",
            "PL" => "폴란드",
            "CZ" => "체코",
            "TR" => "튀르키예",
            "EG" => "이집트",
            "AE" => "아랍에미리트",
            "SA" => "사우디아라비아",
            _ => code
        };
    }

    /// <summary>
    /// 도시명으로 검색 → SearchResult 리스트 반환
    /// </summary>
    public async Task<List<SearchResult>> SearchCitiesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<SearchResult>();

        var key = ApiConfig.ApiKey;

        // Geocoding API 호출
        var geoUrl = $"{ApiConfig.GeoUrl}/direct?q={Uri.EscapeDataString(query)}&limit=5&appid={key}";
        var geoJson = await _http.GetStringAsync(geoUrl);
        var geoDoc = JsonDocument.Parse(geoJson);
        var geoRoot = geoDoc.RootElement;

        if (geoRoot.ValueKind != JsonValueKind.Array || geoRoot.GetArrayLength() == 0)
            return new List<SearchResult>();

        // 각 도시의 현재 온도를 병렬로 가져오기
        var tasks = new List<Task<SearchResult?>>();

        foreach (var city in geoRoot.EnumerateArray())
        {
            tasks.Add(ParseCityWithTempAsync(city));
        }

        var results = (await Task.WhenAll(tasks))
            .Where(r => r != null)
            .Cast<SearchResult>()
            .ToList();

        // 첫 번째 결과를 TopResult로
        if (results.Count > 0)
            results[0].IsTopResult = true;

        return results;
    }

    /// <summary>
    /// Geocoding 결과 하나를 파싱하고, 현재 온도도 가져옴
    /// </summary>
    private static async Task<SearchResult?> ParseCityWithTempAsync(JsonElement city)
    {
        try
        {
            var name = city.GetProperty("name").GetString() ?? "";
            var country = city.GetProperty("country").GetString() ?? "";
            var lat = city.GetProperty("lat").GetDouble();
            var lon = city.GetProperty("lon").GetDouble();

            // 한국어 이름이 있으면 사용
            var localName = name;
            if (city.TryGetProperty("local_names", out var localNames)
                && localNames.TryGetProperty("ko", out var koName))
            {
                localName = koName.GetString() ?? name;
            }

            // 주(state) 정보
            var state = city.TryGetProperty("state", out var stateVal)
                ? stateVal.GetString() ?? "" : "";

            var countryName = CountryCodeToName(country);
            var displayCountry = string.IsNullOrEmpty(state)
                ? countryName
                : $"{countryName} · {state}";

            // 현재 온도 조회
            var temp = await GetTemperatureAsync(lat, lon);

            return new SearchResult
            {
                CityName = localName,
                CountryName = displayCountry,
                FlagEmoji = CountryCodeToFlag(country),
                Temperature = temp,
                Lat = lat,
                Lon = lon
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 위도/경도로 현재 온도만 빠르게 조회
    /// </summary>
    private static async Task<double> GetTemperatureAsync(double lat, double lon)
    {
        try
        {
            var key = ApiConfig.ApiKey;
            var url = $"{ApiConfig.BaseUrl}/weather?lat={lat}&lon={lon}&appid={key}&units={ApiConfig.Units}";
            var json = await _http.GetStringAsync(url);
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
        }
        catch
        {
            return 0;
        }
    }
}
