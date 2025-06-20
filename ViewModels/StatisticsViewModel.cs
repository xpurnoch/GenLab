using CommunityToolkit.Mvvm.ComponentModel;
using BioLabManager.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using Microsoft.EntityFrameworkCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using BioLabManager.Models;

namespace BioLabManager.ViewModels
{
	public partial class StatisticsViewModel : ObservableObject
	{
		[ObservableProperty] private ISeries[] typeSeries;
		[ObservableProperty] private ISeries[] statusSeries;
		[ObservableProperty] private ISeries[] labSeries;
		[ObservableProperty] private ISeries[] timeSeries;
		[ObservableProperty] private Axis[] labXAxis;
		[ObservableProperty] private Axis[] labYAxis;
		[ObservableProperty] private string[] labLabels;

        public StatisticsViewModel() => _ = LoadStatisticsAsync();
		private async Task LoadStatisticsAsync()
        {
			await using var db = new BioLabDbContext();
            var currentUser = AuthService.CurrentUser;

            if (currentUser == null)
            {
				ClearCharts();
                return;
            }

            var samples = await db.Samples
                .Include(s => s.Lab)
                .Where(s => s.UserId == currentUser.Id)
                .ToListAsync();

            TypeSeries = CreatePieSeries(samples, s => s.SampleType);
            StatusSeries = CreatePieSeries(samples, s => s.Status);
            var labGroups = samples
				.GroupBy(s => s.Lab?.Name ?? "Unknown")
				.OrderBy(g => g.Key)
				.ToList();

			LabSeries = new ISeries[]
			{
				new ColumnSeries<int>
				{
					Name = "Samples",
					Values = labGroups.Select(g => g.Count()).ToArray(),
					Fill = new SolidColorPaint(SKColors.DeepSkyBlue)
				}
			};

            LabXAxis = new Axis[]
            {
                new Axis
                {
                    Labels = labGroups.Select(g => g.Key).ToArray(),
                    LabelsRotation = 15,
                    TextSize = 14,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    TicksPaint = new SolidColorPaint(SKColors.White),
                    NamePaint = new SolidColorPaint(SKColors.White),
                }
            };

            LabYAxis = new Axis[]
            {
                new Axis
                {
                    TextSize = 14,
                    LabelsPaint = new SolidColorPaint(SKColors.White),
                    TicksPaint = new SolidColorPaint(SKColors.White),
                    NamePaint = new SolidColorPaint(SKColors.White),
                    MinStep = 1, 
                    MaxLimit = 10, 
                    MinLimit = 0  
                }
            };
        }

        private void ClearCharts()
        {
            TypeSeries = Array.Empty<ISeries>();
            StatusSeries = Array.Empty<ISeries>();
            LabSeries = Array.Empty<ISeries>();
            LabXAxis = Array.Empty<Axis>();
            LabYAxis = Array.Empty<Axis>();
        }

        private static ISeries[] CreatePieSeries<TKey>(IEnumerable<Sample> samples, Func<Sample, TKey> keySelector)
        {
            return samples
                .GroupBy(keySelector)
                .Select(g => new PieSeries<ObservableValue>
                {
                    Name = g.Key?.ToString() ?? "Unknown",
                    Values = new[] { new ObservableValue(g.Count()) }
                })
                .ToArray();
        }
    }
}
