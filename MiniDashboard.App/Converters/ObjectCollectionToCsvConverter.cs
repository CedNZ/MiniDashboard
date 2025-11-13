using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace MiniDashboard.App.Converters
{
    public class ObjectCollectionToCsvConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable items && parameter is string propertyName)
            {
                var parts = new List<string>();

                foreach (var item in items)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    var prop = item.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                    if (prop != null)
                    {
                        var val = prop.GetValue(item);
                        if (val != null)
                        {
                            parts.Add(val.ToString()!);
                        }
                    }
                }

                return string.Join(", ", parts);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
