using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using BioLabManager.Views;
using BioLabManager.Services;

namespace BioLabManager.ViewModels;

public partial class LoginViewModel : ObservableObject, IShowMessage
{
	[ObservableProperty] private string username;
    [ObservableProperty] private string password;

	[RelayCommand]
	private async Task LoginAsync()
	{
		if (await AuthService.LoginAsync(Username, Password))
		{
			if (Application.Current.MainWindow is MainWindow main)
			{
				main.Content = new DashboardView();
			}
		}
		else
		{
			Show("Invalid name or password");
		}
	}

	[RelayCommand]
	private void GoToRegister()
	{
		if (Application.Current.MainWindow is MainWindow main)
		{
			main.Content = new RegisterView();
		}
	}

    public void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}
