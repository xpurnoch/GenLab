using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using BioLabManager.ViewModels;

namespace BioLabManager.Views
{
	public partial class SequenceAnalyzerView : UserControl
	{
		public SequenceAnalyzerView()
		{
			InitializeComponent();
			DataContext = new AnalyzerViewModel();
		}

		private static readonly Regex _atcgRegex = new Regex("^[AaTtCcGg]+$");

		private void InputSequence_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (!_atcgRegex.IsMatch(e.Text))
			{
				e.Handled = true;
				return;
			}

			if (sender is TextBox textBox)
			{
				int caret = textBox.CaretIndex;
				string before = textBox.Text.Substring(0, caret);
				string after = textBox.Text.Substring(caret);
				textBox.Text = before + e.Text.ToUpper() + after;
				textBox.CaretIndex = caret + 1;
				e.Handled = true;
			}
		}

	}
}