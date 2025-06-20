using BioLabManager.Services;
using BioLabManager.ViewModels;
using System.Windows.Controls;

namespace BioLabManager.Views
{
	public partial class DashboardView : UserControl
	{
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(new DataTransferService());
        }

	}
}
