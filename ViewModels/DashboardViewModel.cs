using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BioLabManager.Views;
using BioLabManager.Services;
using System.Windows;
using System.Windows.Media;

namespace BioLabManager.ViewModels
{
	public partial class DashboardViewModel : ObservableObject
	{
        private readonly IDataTransferService _dataTransfer;
        public DashboardViewModel(IDataTransferService dataTransfer)
        {
            _dataTransfer = dataTransfer;
        }

        [ObservableProperty] private object currentView = new WelcomeView();
        [ObservableProperty] private string username = AuthService.CurrentUser?.Username;
        [ObservableProperty] private bool isOnline = true;

        [RelayCommand] public void NavigateToSamples() => CurrentView = new SampleView();
		[RelayCommand] public void NavigateToWelcome() => CurrentView = new WelcomeView();
		[RelayCommand] public void NavigateToStatistics() => CurrentView = new StatisticsView();
		[RelayCommand] public void NavigateToAnalyzer() => CurrentView = new SequenceAnalyzerView();
		[RelayCommand] public void NavigateToMatcher() => CurrentView = new SequenceMatcherView();
		[RelayCommand] public void NavigateToBoxVisualizer() => CurrentView = new BoxVisualizerView();

		public string StatusText => IsOnline ? "Online" : "Offline";
		public Brush StatusColor => IsOnline ? Brushes.LightGreen : Brushes.IndianRed;

		[RelayCommand]
		private void ToggleStatus()
		{
			IsOnline = !IsOnline;
			OnPropertyChanged(nameof(StatusText));
			OnPropertyChanged(nameof(StatusColor));
		}

		[RelayCommand]
		private void Logout()
		{
			if (Application.Current.MainWindow is MainWindow main)
			{
				AuthService.Logout();
				main.Content = new LoginView();
			}
		}

        [RelayCommand]
        public async Task ExportData() => await _dataTransfer.ExportAsync();

        [RelayCommand]
        public async Task ImportData() => await _dataTransfer.ImportAsync();
    }
}
