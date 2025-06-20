using BioLabManager.ViewModels;
using System.Windows.Controls;

namespace BioLabManager.Views
{
	public partial class BoxVisualizerView : UserControl
	{
		public BoxVisualizerView()
		{
			InitializeComponent();
			if (DataContext is BoxVisualizerViewModel vm)
			{
				_ = vm.LoadBoxesAsync();
			}
		}
	}
}