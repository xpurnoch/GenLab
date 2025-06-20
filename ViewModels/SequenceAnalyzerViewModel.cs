using BioLabManager.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace BioLabManager.ViewModels
{
	public partial class AnalyzerViewModel : ObservableObject
	{
		[ObservableProperty] private string inputSequence;
		[ObservableProperty] private string rnaSequence;
		[ObservableProperty] private string proteinSequence;
		[ObservableProperty] private string predictedOrganism;
		[ObservableProperty] private ISeries[] baseCompositionSeries;
		[ObservableProperty] private SolidColorPaint legendTextPaint = new(SKColors.White);
		[ObservableProperty] private ObservableCollection<DnaVisualItem> dnaVisualItems = new();

		public AnalyzerViewModel() => InputSequence = string.Empty;

		[RelayCommand]
        private async Task AnalyzeAsync()
        {
            if (string.IsNullOrWhiteSpace(InputSequence))
            {
                DnaVisualItems.Clear();
                BaseCompositionSeries = null;
                return;
            }

            string sequence = InputSequence.ToUpper().Replace(" ", "").Trim();
            var result = await Task.Run(() =>
            {
                int a = sequence.Count(c => c == 'A');
                int t = sequence.Count(c => c == 'T');
                int c = sequence.Count(c => c == 'C');
                int g = sequence.Count(c => c == 'G');

                var pieSeries = new ISeries[]
                {
					new PieSeries<int> { Values = new[] { a }, Name = "A", Fill = new SolidColorPaint(SKColors.Gold) },
					new PieSeries<int> { Values = new[] { t }, Name = "T", Fill = new SolidColorPaint(SKColors.LightGreen) },
					new PieSeries<int> { Values = new[] { c }, Name = "C", Fill = new SolidColorPaint(SKColors.HotPink) },
					new PieSeries<int> { Values = new[] { g }, Name = "G", Fill = new SolidColorPaint(SKColors.DeepSkyBlue) }
                };

                var visualItems = sequence.Select(n => new DnaVisualItem
                {
                    Base = n.ToString(),
                    Color = n switch
                    {
                        'A' => Brushes.Gold,
                        'T' => Brushes.LightGreen,
                        'C' => Brushes.HotPink,
                        'G' => Brushes.DeepSkyBlue,
                        _ => Brushes.Gray
                    }
                }).ToList();

                return (pieSeries, visualItems);
            });

            BaseCompositionSeries = result.pieSeries;
            LegendTextPaint = new SolidColorPaint(SKColors.White);
            DnaVisualItems = new ObservableCollection<DnaVisualItem>(result.visualItems);
        }

        [RelayCommand]
		private void PredictOrganismFromSequence()
		{
			if (string.IsNullOrWhiteSpace(InputSequence))
			{
				PredictedOrganism = string.Empty;
				return;
			}
			PredictedOrganism = DNAClassifier.PredictOrganism(InputSequence);
		}

		[RelayCommand]
		private void TranslateToProteinFromSequence()
		{
			if (string.IsNullOrWhiteSpace(InputSequence))
			{
				RnaSequence = string.Empty;
				ProteinSequence = string.Empty;
				return;
			}
			RnaSequence = SequenceService.DnaToMrnaComplement(InputSequence);
			ProteinSequence = SequenceService.TranslateRnaToProtein(RnaSequence);
		}

		[RelayCommand]
		private void CopyRna()
		{
			if (!string.IsNullOrWhiteSpace(RnaSequence))
			{
                System.Windows.Clipboard.SetDataObject(RnaSequence);
			}
		}

		[RelayCommand]
		private void CopyProtein()
		{
			if (!string.IsNullOrWhiteSpace(ProteinSequence))
			{
				System.Windows.Clipboard.SetDataObject(ProteinSequence);
			}
		}
    }
}
