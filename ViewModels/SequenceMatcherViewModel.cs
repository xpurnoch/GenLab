using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BioLabManager.Models;
using BioLabManager.Services;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace BioLabManager.ViewModels;

public partial class SequenceMatcherViewModel : ObservableObject
{
	[ObservableProperty] private string inputSequence = string.Empty;
	[ObservableProperty] private string bestMatchIdentifier;
	[ObservableProperty] private int bestDistance;
	[ObservableProperty] private ObservableCollection<DnaVisualItem> inputVisual = new();
	[ObservableProperty] private ObservableCollection<DnaVisualItem> matchVisual = new();

	[RelayCommand]
	private async Task MatchSequenceAsync()
	{
		if (string.IsNullOrWhiteSpace(InputSequence))
		{
			BestMatchIdentifier = string.Empty;
			BestDistance = 0;
			InputVisual.Clear();
			MatchVisual.Clear();
			return;
		}
		var input = InputSequence.Trim().ToUpper();
		await Task.Run(() =>
		{
			var bestMatch = FindBestMatch(input, out int bestDistance);
			App.Current.Dispatcher.Invoke(() =>
			{
				BestMatchIdentifier = bestMatch?.Identifier ?? "None";
				BestDistance = bestDistance;
				InputVisual = VisualizeSequence(input);
				MatchVisual = VisualizeSequence(bestMatch?.Sequence ?? "");
			});
		});
	}

	private ObservableCollection<DnaVisualItem> VisualizeSequence(string sequence)
	{
		var visuals = new ObservableCollection<DnaVisualItem>();
		foreach (char c in sequence)
		{
			visuals.Add(new DnaVisualItem
			{
				Base = c.ToString(),
				Color = c switch
				{
					'A' => Brushes.Gold,
					'T' => Brushes.LightGreen,
					'C' => Brushes.HotPink,
					'G' => Brushes.DeepSkyBlue,
					_ => Brushes.Gray
				}
			});
		}
		return visuals;
	}

	private static int LevenshteinDistance(string a, string b)
	{
		int[,] dp = new int[a.Length + 1, b.Length + 1];
		for (int i = 0; i <= a.Length; i++)
			for (int j = 0; j <= b.Length; j++)
			{
				if (i == 0) dp[i, j] = j;
				else if (j == 0) dp[i, j] = i;
				else
				{
					int cost = a[i - 1] == b[j - 1] ? 0 : 1;
					dp[i, j] = Math.Min(Math.Min(
						dp[i - 1, j] + 1,
						dp[i, j - 1] + 1),
						dp[i - 1, j - 1] + cost);
				}
			}
		return dp[a.Length, b.Length];
	}

	private Sample FindBestMatch(string input, out int bestDistance)
	{
		bestDistance = int.MaxValue;
		Sample bestMatch = null;

		using var db = new BioLabDbContext();
        var currentUser = AuthService.CurrentUser;
        if (currentUser == null)
        {
            bestDistance = 0;
            return null;
        }

        var samples = db.Samples
            .Where(s => s.UserId == currentUser.Id)
            .ToList();

        foreach (var sample in samples)
		{
			if (string.IsNullOrWhiteSpace(sample.Sequence)) continue;

			var distance = LevenshteinDistance(input, sample.Sequence.ToUpper());
			if (distance < bestDistance)
			{
				bestDistance = distance;
				bestMatch = sample;
			}
		}
		return bestMatch;
	}

}
