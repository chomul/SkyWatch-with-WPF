using System.Linq;

namespace SkyWatch.Helpers;

public static class CountryHelper
{
    /// <summary>
    /// 국가 코드 → 국기 이모지 변환 (ISO 3166-1 alpha-2)
    /// </summary>
    public static string CountryCodeToFlag(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != 2) return "🏳️";
        // 국가코드 문자를 Regional Indicator Symbol으로 변환
        return string.Concat(code.ToUpper().Select(c => char.ConvertFromUtf32(c + 0x1F1A5)));
    }

    /// <summary>
    /// 주요 국가 코드 → 한국어 국가명
    /// </summary>
    public static string CountryCodeToName(string code)
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
}
