using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using BioLabManager.Models;
using BioLabManager.Services;

namespace BioLabManager.ViewModels
{
	public partial class WelcomeViewModel : ObservableObject
	{
        private bool _isNewsLoadFailed;
        public bool IsNewsLoadFailed
        {
            get => _isNewsLoadFailed;
            set => SetProperty(ref _isNewsLoadFailed, value);
        }
        [ObservableProperty] private ObservableCollection<NewsItem> news = new();

		public WelcomeViewModel() => LoadNewsAsync();

        private async void LoadNewsAsync()
        {
            var fetched = await NewsService.FetchScienceNewsAsync();
            News = new ObservableCollection<NewsItem>(fetched.Articles.Take(4));
            IsNewsLoadFailed = fetched.IsError;
        }
    }
}
