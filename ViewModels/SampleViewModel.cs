using BioLabManager.Models;
using BioLabManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BioLabManager.ViewModels
{
	public partial class SampleViewModel : ObservableObject, IShowMessage
	{
		[ObservableProperty] private ObservableCollection<Sample> samples;
		[ObservableProperty] private ObservableCollection<Sample> filteredSamples;
		[ObservableProperty] private string filterText;
		[ObservableProperty] private string newLabName;
		[ObservableProperty] private Sample newSample;
		[ObservableProperty] private List<string> statusOptions = new() { "Active", "Not Active" };
		[ObservableProperty] private List<string> typeOptions = new() { "DNA", "RNA", "Protein", "Other" };
		[ObservableProperty] private string sortBy = "None";
		[ObservableProperty] private bool isSortDescending = false;

        public SampleViewModel()
		{
			_ = LoadSamplesAsync();
			NewSample = CreateNewSample();

			WeakReferenceMessenger.Default.Register<Messages.SamplesImportedMessage>(this, async (r, m) =>
			{
				await RefreshSamplesAsync();
			});

			ApplyFilter();
		}

        [RelayCommand]
        private async Task AddSampleCommandAsync() => await AddSampleAsync();
        public IRelayCommand<Sample> DeleteSampleCommand => new AsyncRelayCommand<Sample>(DeleteSampleAsync);
        public IRelayCommand<string> SortCommand => new RelayCommand<string>(SortSamplesBy);

        private async Task LoadSamplesAsync()
		{
			await using var db = new BioLabDbContext();
			var loadedSamples = AuthService.CurrentUser != null
				? await db.Samples
					.Include(s => s.Lab)
					.Include(s => s.User)
					.Where(s => s.UserId == AuthService.CurrentUser.Id)
					.ToListAsync()
				: new List<Sample>();

			Samples = new ObservableCollection<Sample>(loadedSamples);
			ApplyFilter();
		}

		private async Task AddSampleAsync()
		{
			if (!IsNewSampleValid())
			{
				Show("Please fill in all required fields.");
				ResetNewSample();
				return;
			}

			if (!int.TryParse(NewSample.Identifier?.Trim(), out int numberOnly) ||
				!int.TryParse(NewSample.StorageLocation?.Trim(), out int storageNumber))
			{
				Show("Identifier and Storage must be a numeric value.");
				ResetNewSample();
				return;
			}

			string sequence = NewSample.Sequence?.Trim().ToUpper() ?? "";
			await using var db = new BioLabDbContext();

            var lab = await LabService.GetOrCreateLabAsync(db, NewLabName);
            FormatAndAssignSampleMetadata(lab, numberOnly, storageNumber, sequence);

            db.Samples.Add(NewSample);
			await db.SaveChangesAsync();

			var added = await db.Samples
				.Include(s => s.Lab)
				.FirstOrDefaultAsync(s => s.Identifier == NewSample.Identifier && s.LabId == lab.Id);

			if (added != null)
			{
				Samples.Add(added);
			}

			ResetNewSample();
			ApplyFilter();
		}

		private async Task DeleteSampleAsync(Sample sample)
		{
			if (sample == null) 
				return;
			await using var db = new BioLabDbContext();
			var s = await db.Samples.FindAsync(sample.Id);
			if (s != null)
			{
				db.Samples.Remove(s);
				await db.SaveChangesAsync();
			}

			Samples.Remove(sample);
			ApplyFilter();
		}

		private void SortSamplesBy(string sort)
		{
			SortBy = sort;
			IsSortDescending = SortBy == sort ? !IsSortDescending : false;
			ApplySorting();
		}

		private void ApplySorting()
		{
			IEnumerable<Sample> sorted = SortBy switch
			{
				"Identifier" => Sort(Samples, s => s.Identifier),
				"Type" => Sort(Samples, s => s.SampleType),
				"Status" => Sort(Samples, s => s.Status),
				"Storage" => Sort(Samples, s => s.StorageLocation),
				"Lab" => Sort(Samples, s => s.Lab?.Name),
				"Date" => Sort(Samples, s => s.CollectedAt),
				_ => Samples
			};

			FilteredSamples = new ObservableCollection<Sample>(sorted);
			OnPropertyChanged(nameof(FilteredSamples));
		}

		private IEnumerable<Sample> Sort<TKey>(IEnumerable<Sample> source, Func<Sample, TKey> keySelector)
		{
			return IsSortDescending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
		}

		private void ApplyFilter()
		{
			if (string.IsNullOrWhiteSpace(FilterText))
			{
				FilteredSamples = new ObservableCollection<Sample>(Samples);
			}
			else
			{
				var lower = FilterText.ToLower();
				var filtered = Samples.Where(s =>
					s.SampleType?.ToLower().Contains(lower) == true ||
					s.Identifier?.ToLower().Contains(lower) == true ||
					s.Status?.ToLower().Contains(lower) == true ||
					s.StorageLocation?.ToLower().Contains(lower) == true ||
					s.Lab?.Name?.ToLower().Contains(lower) == true ||
					s.Sequence?.ToLower().Contains(lower) == true
				);

				FilteredSamples = new ObservableCollection<Sample>(filtered);
			}
			OnPropertyChanged(nameof(FilteredSamples));
		}

		public async Task RefreshSamplesAsync()
		{
			await using var db = new BioLabDbContext();
			var samplesFromDb = await db.Samples
				.Include(s => s.Lab)
				.Include(s => s.User)
				.Where(s => s.UserId == AuthService.CurrentUser.Id)
				.ToListAsync();

			Samples.Clear();
			foreach (var sample in samplesFromDb)
				Samples.Add(sample);

			ApplyFilter();
		}

        private bool IsNewSampleValid()
        {
            var requiredFields = new[]
            {
				NewSample.Identifier,
				NewSample.SampleType,
				NewSample.Status,
				NewSample.StorageLocation,
				NewSample.Sequence,
				NewLabName
			};
            return requiredFields.All(s => !string.IsNullOrWhiteSpace(s));
        }

        private void ResetNewSample()
		{
			NewSample = CreateNewSample();
			NewLabName = string.Empty;
			OnPropertyChanged(nameof(NewSample));
			OnPropertyChanged(nameof(NewLabName));
		}

		private Sample CreateNewSample() => new()
		{
			Identifier = "",
			SampleType = null,
			Status = null,
			StorageLocation = "",
			Sequence = "",
			CollectedAt = DateTime.Now
		};

        private void FormatAndAssignSampleMetadata(Lab lab, int idNum, int boxNum, string sequence)
        {
            NewSample.Identifier = $"SMP-{idNum:D3}";
            NewSample.StorageLocation = $"BOX-{boxNum:D2}";
            NewSample.Sequence = sequence;
            NewSample.LabId = lab.Id;
            NewSample.UserId = AuthService.CurrentUser.Id;
        }

        partial void OnFilterTextChanged(string value) => ApplyFilter();

		[RelayCommand]
		private void CopySequence(string? sequence)
		{
			if (!string.IsNullOrWhiteSpace(sequence))
				Clipboard.SetDataObject(sequence);
			
		}

        public void Show(string message, string title = "Info", MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }
    }
}
