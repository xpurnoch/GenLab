using System.Windows;
using System.Windows.Controls;

namespace BioLabManager.Helpers
{
	public static class PasswordHelper
	{
		public static readonly DependencyProperty BoundPassword =
			DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

		public static readonly DependencyProperty BindPassword =
			DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, OnBindPasswordChanged));

		private static readonly DependencyProperty UpdatingPassword =
			DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false));

		public static string GetBoundPassword(DependencyObject dp) =>
			(string)dp.GetValue(BoundPassword);

		public static void SetBoundPassword(DependencyObject dp, string value) =>
			dp.SetValue(BoundPassword, value);

		public static bool GetBindPassword(DependencyObject dp) =>
			(bool)dp.GetValue(BindPassword);

		public static void SetBindPassword(DependencyObject dp, bool value) =>
			dp.SetValue(BindPassword, value);

		private static bool GetUpdatingPassword(DependencyObject dp) =>
			(bool)dp.GetValue(UpdatingPassword);

		private static void SetUpdatingPassword(DependencyObject dp, bool value) =>
			dp.SetValue(UpdatingPassword, value);

		private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			if (dp is PasswordBox box && !GetUpdatingPassword(box))
			{
				box.PasswordChanged -= HandlePasswordChanged;
				box.Password = (string)e.NewValue;
				box.PasswordChanged += HandlePasswordChanged;
			}
		}

		private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			if (dp is PasswordBox box)
			{
				if ((bool)e.NewValue)
				{
					box.PasswordChanged += HandlePasswordChanged;
				}
				else
				{
					box.PasswordChanged -= HandlePasswordChanged;
				}
			}
		}

		private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
		{
			if (sender is PasswordBox box)
			{
				SetUpdatingPassword(box, true);
				SetBoundPassword(box, box.Password);
				SetUpdatingPassword(box, false);
			}
		}
	}
}
