using System.Windows.Controls;
using BioLabManager.ViewModels;

namespace BioLabManager.Views
{
	public partial class StatisticsView : UserControl
	{
		public StatisticsView()
		{
			InitializeComponent();
			DataContext = new StatisticsViewModel();
		}
	}
}
