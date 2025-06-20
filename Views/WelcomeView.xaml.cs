using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using BioLabManager.ViewModels;

namespace BioLabManager.Views
{
	public partial class WelcomeView : UserControl
	{
		public WelcomeView()
		{
			InitializeComponent();
			DataContext = new WelcomeViewModel();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = e.Uri.AbsoluteUri,
				UseShellExecute = true
			});
			e.Handled = true;
		}
	}
}
