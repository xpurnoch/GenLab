using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using BioLabManager.Views;
using BioLabManager.Services;

namespace BioLabManager.ViewModels;

public partial class RegisterViewModel : ObservableObject, IShowMessage
{
	[ObservableProperty] private string username;
    [ObservableProperty] private string password;
    [ObservableProperty] private string labName;

    [RelayCommand]
	private async Task RegisterAsync()
	{
        if (new[] { Username, Password, LabName }.Any(string.IsNullOrWhiteSpace))
        {
			Show("Please fill in all fields before registering!");
			return;
		}

		if (await AuthService.RegisterAsync(Username, Password, LabName))
		{
			Show("Registered successfully!");
			if (Application.Current.MainWindow is MainWindow main)
				main.Content = new LoginView();
			
		}
		else
		{
			Show("Username already exists!");
		}
	}

    [RelayCommand]
    private void GoToLogin()
    {
        if (Application.Current.MainWindow is MainWindow main)
        {
			main.Content = new LoginView();
        }
    }

    public void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, icon);
    }
}
