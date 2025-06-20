using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace BioLabManager.Helpers
{
	public class EmptyToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
			string.IsNullOrWhiteSpace(value?.ToString()) ? Visibility.Visible : Visibility.Collapsed;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
			throw new NotImplementedException();
		
	}

}
