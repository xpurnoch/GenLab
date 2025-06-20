using BioLabManager.ViewModels;
using System.Windows.Controls;

namespace BioLabManager.Views
{
	public partial class RegisterView : UserControl
	{
		public RegisterView()
		{
			InitializeComponent();
			DataContext = new RegisterViewModel();
		}
	}
}
