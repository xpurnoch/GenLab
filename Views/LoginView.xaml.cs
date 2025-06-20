using System.Windows.Controls;
using BioLabManager.ViewModels;

namespace BioLabManager.Views;

public partial class LoginView : UserControl
{
	public LoginView()
	{
		InitializeComponent();
		DataContext = new LoginViewModel();
	}
}
