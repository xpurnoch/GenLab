using BioLabManager.Views;
using System.Windows;

namespace BioLabManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
		Content = new LoginView();
	}
}