using BioLabManager.ViewModels;
using System.Windows.Controls;

namespace BioLabManager.Views
{
    public partial class SampleView : UserControl
    {
        public SampleView()
        {
            InitializeComponent();
			DataContext = new SampleViewModel();
		}
	}
}
