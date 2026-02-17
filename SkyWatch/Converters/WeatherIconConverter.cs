using System.Globalization;
using System.Windows.Data;

namespace SkyWatch.Converters;

/// <summary>
/// OpenWeatherMap 아이콘 코드를 이모지로 변환하는 Converter.
/// XAML에서 {Binding IconCode, Converter={StaticResource WeatherIconConverter}} 형태로 사용합니다.
/// 
/// 아이콘 코드 형식: "XXy" (XX = 날씨 번호, y = d(낮)/n(밤))
/// 참고: https://openweathermap.org/weather-conditions
/// </summary>
public class WeatherIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string iconCode || string.IsNullOrEmpty(iconCode))
            return "❓";

        // 숫자 부분만 추출 (예: "02d" → "02")
        var code = iconCode.Length >= 2 ? iconCode[..2] : iconCode;
        var isNight = iconCode.EndsWith('n');

        return code switch
        {
            "01" => isNight ? "🌙\uFE0E" : "☀️\uFE0E",    // 맑음
            "02" => isNight ? "🌙\uFE0E" : "🌤\uFE0E",     // 구름 조금
            "03" => "⛅\uFE0E",                       // 구름 많음
            "04" => "☁️\uFE0E",                       // 흐림
            "09" => "🌧\uFE0E",                       // 소나기
            "10" => "🌧\uFE0E",                       // 비
            "11" => "⛈\uFE0E",                        // 뇌우
            "13" => "❄️\uFE0E",                       // 눈
            "50" => "🌫\uFE0E",                       // 안개
            _ => "🌤\uFE0E"                           // 기본값
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("WeatherIconConverter는 단방향 전용입니다.");
    }
}
