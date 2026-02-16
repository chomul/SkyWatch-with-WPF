using System.Globalization;
using System.Windows.Data;

namespace SkyWatch.Converters;

/// <summary>
/// TempBarRatio (0.0~1.0) × MaxWidth(60px) → 실제 픽셀 너비로 변환.
/// ConverterParameter로 최대 너비를 전달합니다.
/// 예: Converter={StaticResource RatioToWidthConverter}, ConverterParameter=60
/// </summary>
public class RatioToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double ratio && parameter is string maxWidthStr
            && double.TryParse(maxWidthStr, CultureInfo.InvariantCulture, out var maxWidth))
        {
            return ratio * maxWidth;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
