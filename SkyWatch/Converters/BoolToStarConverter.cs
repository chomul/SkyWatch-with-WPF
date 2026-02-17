using System.Globalization;
using System.Windows.Data;

namespace SkyWatch.Converters;

public class BoolToStarConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFavorite && isFavorite)
        {
            return "★"; // 채워진 별
        }
        return "☆"; // 빈 별
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
